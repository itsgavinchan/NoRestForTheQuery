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
        public float rotation, rotSpeed, speed;
        public int width, height;
        public GameObject( Vector2 position, Vector2 origin, Vector2 velocity, float speed, int width, int height ) {
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
        public DamagableObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed, int width, int height)
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

        public void handleCollision( GameObject other ){
            Rectangle otherBoundary, objBoundary;
            
            objBoundary = boundingRectangle( new Rectangle( 0, 0, (int)width, (int)height ), this.transform );
            otherBoundary = boundingRectangle( new Rectangle( 0, 0, (int)other.width, (int)other.height ), other.transform );

            if( otherBoundary.Intersects( objBoundary ) && !hit ){
                if( isHit( other ) ){
                    hit = true;
                    return;
                }
                else{
                    hit = false;
                }
            }
            
        }

        public bool isHit(GameObject other) {
    
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
            for (int yA = 0; yA < other.height; ++yA) {
                posInB = rowStartInB;

                //Check each pixel
                for (int xA = 0; xA < other.width; ++xA) {

                    //Attempt to get the corresponding point in this object
                    int yB = (int)Math.Round(posInB.Y);
                    int xB = (int)Math.Round(posInB.X);

                    //If the points are in the constraints of this object
                    if (0 <= yB && yB < height && 0 <= xB && xB < width) {

                        //Compare the colors. If both are not transparent, they've hit each other!
                        if (other.colorArr[xA + yA * other.width].A != 0 && this.colorArr[xB + yB * width].A != 0) {
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

    class SearchCone : GameObject {
        public bool foundSomeone;
        public float angle, searchDirection, searchBoundary;
        public SearchCone( Vector2 position, Vector2 origin )
            : base( position, origin, Vector2.Zero, 0, FinalGame.searchConeSprite.Width, FinalGame.searchConeSprite.Height ){
            foundSomeone = false;
            searchBoundary = (float)Math.PI/6;
            angle = 0;
            searchDirection = -1;
            this.colorArr = new Color[ FinalGame.searchConeSprite.Width * FinalGame.searchConeSprite.Height ];
            FinalGame.searchConeSprite.GetData<Color>( colorArr );
        }

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

        public void searchFor(GameObject other) {
            Rectangle otherBoundary, objBoundary;
            
            objBoundary = boundingRectangle( new Rectangle( 0, 0, width, height ), this.transform );
            otherBoundary = boundingRectangle( new Rectangle( 0, 0, other.width, other.height ), other.transform );

            //Check only if the other object is in close vicinity
            if( otherBoundary.Intersects( objBoundary ) ){
                //Map pixels of the other object to the cone
                Matrix transformAtoB = other.transform * Matrix.Invert( this.transform );

                //Increments in the other object in terms of the cone
                Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAtoB);
                Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAtoB);

                //Top left corner of the other object
                Vector2 rowStartInB = Vector2.Transform(Vector2.Zero, transformAtoB);
            
                //Temp storage for traversals in the row
                Vector2 posInB;

                //For every row in the other object
                for (int yA = 0; yA < other.height; ++yA) {
                    posInB = rowStartInB;

                    //Check each pixel
                    for (int xA = 0; xA < other.width; ++xA) {

                        //Attempt to get the corresponding point in the cone
                        int yB = (int)Math.Round(posInB.Y);
                        int xB = (int)Math.Round(posInB.X);

                        //If the points are in the constraints of the cone
                        if (0 <= yB && yB < height && 0 <= xB && xB < width) {

                            //Compare the colors. If both are not transparent, the other object is sighted!
                            if (other.colorArr[xA + yA * (int)other.width].A != 0 && this.colorArr[xB + yB * (int)width].A != 0) {
                                foundSomeone = true;
                                return;
                            }
                        }
                        //As we advance to the next pixel in A, advance to the next pixel in B
                        posInB += stepX;
                    }
                    //Next row 
                    rowStartInB += stepY;
                }
                //Found nothing
                foundSomeone = false;
            }
            else{
                foundSomeone = false;
            }
        }

        public void update( float anchorPosX, float anchorPosY, GameObject target ){
            searchFor( target );
            //Default behavior
            if( !foundSomeone ){
                if( angle >= searchBoundary ){ searchDirection = -1; }
                else if( angle < -searchBoundary ){ searchDirection = 1; }
                angle += searchDirection*.01F;
                rotation = angle;
            }
            else{
                angle = (float)Math.Atan2( (position.Y)-(target.position.Y+target.origin.Y) , (position.X)-(target.position.X+target.origin.X)  );
                rotation = angle;
            }

            position.X = anchorPosX;
            position.Y = anchorPosY;
            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position, 0.0f));
        }
    }

}
