﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace noRestForTheQuery {

    class Professor : GameObject {
        int /*markerAmt,*/ markerSpeed, shootCooldown, elapsedTime;
        public SearchCone search;
        public int attackPower;
        public float hover;
        public List<Marker> markers = new List<Marker>();
        public Professor(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed, FinalGame.professorSprite.Width, FinalGame.professorSprite.Height) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
                isAlive = false;
                markerSpeed = 10;
                this.attackPower = attackPower;

                //Search Cone will rotate around the professor, so the professor's origin is provided in the constructor
                search = new SearchCone( new Vector2( position.X+origin.X-FinalGame.searchConeSprite.Width, 
                                                      position.Y+origin.Y-FinalGame.searchConeSprite.Height/2 ),
                                         new Vector2( FinalGame.searchConeSprite.Width, FinalGame.searchConeSprite.Height/2 ) );
                
                shootCooldown = 800;
                elapsedTime = shootCooldown/2;
        }

        // Update the Position And/Or Velocity
        public void update( Student1 student, GameTime gameTime ) {

            //Add a hover to make it look nicer
            velocity.Y = (float)Math.Cos(hover -= .05F);
            if (hover < -2 * Math.PI) { hover = 0; } //To prevent the hover value from getting too large
            position.Y += velocity.Y;
            
            if ( isAlive && position.X >= FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - 100) { position.X -= speed; }
            else { position.X = FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - 100; }


            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position+this.origin, 0.0f));

            
            search.update( position.X+width/2, position.Y+origin.Y/2, student); 
            if( isAlive && search.foundSomeone ){ 
                elapsedTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime < 0 ) {
                    if( student.position.X + FinalGame.studentSprite.Width*3 < position.X ) shoot( student.position.X, student.position.Y );
                    elapsedTime = shootCooldown;
                }
            }
            else{ elapsedTime = shootCooldown/2; }
            
        }

        public void reset() {
            isAlive = false;
            markers.Clear();
            position = new Vector2(FinalGame.WINDOW_WIDTH, (FinalGame.WINDOW_HEIGHT - FinalGame.professorSprite.Height) / 2);
        }

        public void shoot( double studentPosX, double studentPosY) {
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
        }
    }

    class Assignment : DamagableObject {
        protected bool chasing;
        protected float hover, searchRadius;

        public Assignment(Vector2 position, Vector2 origin, Vector2 velocity, float speed, int width, int height, bool chaseState, float searchRadius):
            base(position, origin, velocity, 0, width, height) {
                this.chasing = chaseState;
                this.searchRadius = searchRadius;
                this.hover = (float)FinalGame.rand.NextDouble();
        }

        public void update(float x, float y) {
            if (!chasing) {
                velocity.Y = 4 * (float)Math.Sin(hover -= .05F);
                velocity.X = (float)Math.Cos(hover);
                if (hover < -2 * Math.PI) { hover = 0; } //To prevent the hover value from getting too large
                if (isOtherObjectClose(x, y, searchRadius)) { chasing = true; }
            }
            else {
                rotation = (float)Math.Atan2(y - (position.Y + origin.Y), x - (position.X + origin.X));
                velocity.X += (float)Math.Cos(rotation) * (.09F * speed);
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
            base(position, origin, velocity, 0, FinalGame.homeworkSprite.Width, FinalGame.homeworkSprite.Height, false, 250) {
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

            fullHealth = 30 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 20 * FinalGame.gameLevel;

            // Assign Values to Local Members
            durability = FinalGame.gameLevel * 10;
            speed = (float)(FinalGame.rand.Next(4, 5) + FinalGame.rand.NextDouble());
        }

    }
}
