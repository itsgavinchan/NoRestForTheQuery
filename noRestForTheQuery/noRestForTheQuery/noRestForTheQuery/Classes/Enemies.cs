using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {

    class Professor : GameObject {
        int /*markerAmt,*/ markerSpeed, shootCooldown, elapsedTime;
        public SearchCone search;
        public int attackPower;
        public List<Marker> markers = new List<Marker>();
        public Professor(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed, FinalGame.professorSprite.Width, FinalGame.professorSprite.Height) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
                isAlive = false;
                //markerAmt = 10;
                markerSpeed = 10;
                this.attackPower = attackPower;
                //Search Cone will rotate around the professor, so the professor's origin is provided in the constructor
                search = new SearchCone( new Vector2( origin.X-FinalGame.searchConeSprite.Width, origin.Y-FinalGame.searchConeSprite.Height ), this.origin );
                shootCooldown = 200;
                elapsedTime = 0;
        }

        // Update the Position And/Or Velocity
        public void update( Student1 student, GameTime gameTime ) {
            
            if ( isAlive && position.X >= FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - 100) { position.X -= speed; }
            else { position.X = FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - 100; }
            //if ( isAlive && position.X < FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - 100 ) { position.X += speed; }
            search.searchFor( student );
            if( search.foundSomeone ){
                elapsedTime -= gameTime.ElapsedGameTime.Milliseconds;
                if( elapsedTime < 0 ){
                    shoot( student.position.X, student.position.Y );
                    elapsedTime = shootCooldown;
                }
            }
            search.position.X = origin.X-FinalGame.searchConeSprite.Width;
            search.position.Y = origin.Y-FinalGame.searchConeSprite.Height;
            
            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position+this.origin, 0.0f));
        }

        public void reset() {
            isAlive = false;
            markers.Clear();
            //markerAmt = 10;
            position = new Vector2(FinalGame.WINDOW_WIDTH, (FinalGame.WINDOW_HEIGHT - FinalGame.professorSprite.Height) / 2);
        }

        public void shoot( double studentPosX, double studentPosY) {
            //if (markerAmt > 0) {
                markers.Add(new Marker( attackPower, 
                                        new Vector2(position.X + FinalGame.professorSprite.Width / 2 - FinalGame.markerSprite.Width / 2,   //Position
                                                    position.Y + FinalGame.professorSprite.Height / 2 - FinalGame.markerSprite.Height),
                                        new Vector2(FinalGame.markerSprite.Width / 2,                   //Origin
                                                    FinalGame.markerSprite.Height / 2), 
                                        Vector2.Zero,                                                   //Velocity
                                        markerSpeed,                                                    //Speed
                                        (float)(Math.Atan2(studentPosY, studentPosX))));                //Rotation
                markers.Last().colorArr = new Color[ FinalGame.markerSprite.Width*FinalGame.markerSprite.Height ];
                FinalGame.markerSprite.GetData<Color>( markers.Last().colorArr );
            //    markerAmt--;
            //}
        }
    }

    class Assignment : DamagableObject {
        protected bool chasing;
        protected float hover, searchRadius;

        public Assignment(Vector2 position, Vector2 origin, Vector2 velocity, float speed, int width, int height, bool chaseState, float searchRadius):
            base(position, origin, velocity, 0, width, height) {
                this.chasing = chaseState;
                this.searchRadius = searchRadius;
                this.hover = 0.0F;
        }

        public void update(float x, float y) {
            if (!chasing) {
                velocity.Y = (float)Math.Sin(hover -= .05F);
                if (hover < -2 * Math.PI) { hover = 0; } //To prevent the hover value from getting too large
                if (isOtherObjectClose(x, y, searchRadius)) { chasing = true; }
            }
            else {
                rotation = (float)Math.Atan2(y - (position.Y + origin.Y), x - (position.X + origin.X));
                velocity.X += (float)Math.Cos(rotation) * (.05F * speed);
                velocity.Y += (float)Math.Sin(rotation) * (.07F * speed);
                rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            }

            velocity.X = MathHelper.Clamp( velocity.X, -2.5F*speed, 2.5F*speed );
            velocity.Y = MathHelper.Clamp( velocity.Y, -2.5F*speed, 2.5F*speed );
            position.X += velocity.X;
            position.Y += velocity.Y;
            this.transform =
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position + this.origin, 0.0f));
        }
    }

    class Homework : Assignment {
        public Homework(Vector2 position, Vector2 origin, Vector2 velocity ) :
            base(position, origin, velocity, 0, FinalGame.homeworkSprite.Width, FinalGame.homeworkSprite.Height, false, 400) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 50 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 10 * FinalGame.gameLevel;

            // Assign Values to Local Members
            speed = (float)(FinalGame.rand.Next(2, 3) + FinalGame.rand.NextDouble());
        }
    }
    class Exam : Assignment {
        public int durability;
        public Exam(Vector2 position, Vector2 origin, Vector2 velocity ) :
            base(position, origin, velocity, 0, FinalGame.examSprite.Width, FinalGame.examSprite.Height, false, 550) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 100 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 20 * FinalGame.gameLevel;

            // Assign Values to Local Members
            durability = FinalGame.gameLevel * 10;
            speed = (float)(FinalGame.rand.Next(5, 6) + FinalGame.rand.NextDouble());
        }

    }
    //class Final : DamagableObject {
    //    public Final(Vector2 position, Vector2 origin, Vector2 velocity, float speed ) :
    //        base(position, origin, velocity, speed, FinalGame.finalSprite.Width, FinalGame.finalSprite.Height) {
    //        // Values Already Assigned To: 
    //        //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
    //        //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
    //        // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

    //        fullHealth = 200 * FinalGame.gameLevel;
    //        currentHealth = fullHealth;
    //        attackPower = 50 * FinalGame.gameLevel;

    //        // Assign Values to Local Members

    //    }
    //    public void update( float x, float y ) { }
    //}
}
