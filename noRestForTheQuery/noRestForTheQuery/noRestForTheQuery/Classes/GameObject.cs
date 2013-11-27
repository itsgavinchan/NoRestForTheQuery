using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {
    public class GameObject {
        public bool isAlive = true;
        public Vector2 position, origin, velocity;
        public Color[] colorArr;
        public Matrix transform;
        public float rotation, rotSpeed, speed, width, height;
        public GameObject( Vector2 position, Vector2 origin, Vector2 velocity, float speed, float width, float height ) {
            this.position = position;
            this.velocity = velocity;
            this.origin = origin;
            this.speed = speed;
            this.rotation = 0;
            this.width = width;
            this.height = height;
        }

        public bool checkBoundaries( int width, int height ) {
            if (position.X <= FinalGame.screenOffset - width) { return true; }
            if (position.X >= (FinalGame.WINDOW_WIDTH + FinalGame.screenOffset) + width) { return true; }
            if (position.Y <= 0) { return true; }
            if (position.Y >= FinalGame.WINDOW_HEIGHT + height) { return true; }
            return false;
        }

        protected bool isOtherObjectClose( float playerX, float playerY, float radius ){
            float distance = (float)Math.Sqrt( Math.Pow(position.X-playerX, 2) - Math.Pow(position.Y-playerY, 2) );
            if( distance < radius ){ return true; }
            return false;
        }
    }

    class Platform : GameObject {
        public Rectangle rectangle;
        public Platform(Vector2 position, Vector2 velocity, float speed)
            : base(position, 
                   new Vector2((FinalGame.defaultBlockWidth / 2), (FinalGame.defaultBlockHeight / 2)), 
                   velocity, speed, FinalGame.defaultBlockWidth, FinalGame.defaultBlockHeight) 
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, FinalGame.defaultBlockWidth, FinalGame.defaultBlockHeight);
        }
    }

    class DamagableObject : GameObject {
        public bool hit;
        public int currentHealth, fullHealth, attackPower;
        public DamagableObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed, float width, float height)
            : base(position, origin, velocity, speed, width, height) { }

        public bool checkDeath() {
            if (currentHealth <= 0) { isAlive = false; }
            return isAlive;
        }

        public void incrementHealth(int restore) { if (isAlive) currentHealth += restore; }
        public void decrementHealth(int damage) { if (isAlive) currentHealth -= damage; checkDeath(); }
        public void incrementAttack(int boost) { if (isAlive) attackPower += boost; }

        public Rectangle boundingRectangle(Rectangle rectangle, Matrix transform) {
            //Rectangle's four corners
            Vector2 topLeft  = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 topRight = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 botLeft  = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 botRight = new Vector2(rectangle.Right, rectangle.Bottom);

            //Transform corners into world coordinates
            Vector2.Transform(ref topLeft, ref transform, out topLeft);
            Vector2.Transform(ref topRight, ref transform, out topRight);
            Vector2.Transform(ref botLeft, ref transform, out botLeft);
            Vector2.Transform(ref botRight, ref transform, out botRight);

            //Find the coordinates of the boundary rectangle's topLeft (min) and botRight (max)
            Vector2 boundaryTopLeft = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(botLeft, botRight));
            Vector2 boundaryBotRight = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(botLeft, botRight));

            // Return that as a rectangle
            return new Rectangle((int)boundaryTopLeft.X,                         //Boundary start X
                                 (int)boundaryTopLeft.Y,                         //Boundary start Y
                                 (int)(boundaryBotRight.X - boundaryTopLeft.X),  //Boundary width
                                 (int)(boundaryBotRight.Y - boundaryTopLeft.Y)); //Boundary height
        }
        public void handleCollision( GameObject other, int otherWidth, int otherHeight, int objWidth, int objHeight ){
            Rectangle otherBoundary, objBoundary;
            
            objBoundary = boundingRectangle( new Rectangle( 0, 0, objWidth, objHeight ), this.transform );
            otherBoundary = boundingRectangle( new Rectangle( 0, 0, otherWidth, otherHeight ), other.transform );

            if( otherBoundary.Intersects( objBoundary ) ){
                if( isHit( other, otherWidth, otherHeight, objWidth, objHeight ) ){
                    hit = true;
                    return;
                }
                else{
                    hit = false;
                }
            }
            
        }
        public bool isHit(GameObject other, int otherWidth, int otherHeight, int objWidth, int objHeight) {
    
            //Map pixels of the other object to this object
            Matrix transformAtoB = other.transform * Matrix.Invert( this.transform );

            //Increments in the other object in terms of this object
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAtoB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAtoB);

            //Top left corner of the other object
            Vector2 rowStartInB = Vector2.Transform(Vector2.Zero, transformAtoB);
            
            //Temp storage for traversals in the row
            Vector2 posInB;

            //For every row in the other object
            for (int yA = 0; yA < otherHeight; ++yA) {
                posInB = rowStartInB;

                //Check each pixel
                for (int xA = 0; xA < otherWidth; ++xA) {

                    //Attempt to get the corresponding point in this object
                    int yB = (int)Math.Round(posInB.Y);
                    int xB = (int)Math.Round(posInB.X);

                    //If the points are in the constraints of this object
                    if (0 <= yB && yB < objHeight && 0 <= xB && xB < objWidth) {

                        //Compare the colors. If both are not transparent, they've hit each other!
                        if (other.colorArr[xA + yA * otherWidth].A != 0 && this.colorArr[xB + yB * objWidth].A != 0) {
                            return true;
                        }
                    }
                    //As we advance to the next pixel in A, advance to the next pixel in B
                    posInB += stepX;
                }
                //Next row 
                rowStartInB += stepY;
            }
            //No collision
            return false;
        }
    }

    


}
