using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {

    class Missile : GameObject {
        public int attackPower;
        public Missile(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed, int width, int height) :
            base(position, origin, velocity, speed, width, height) {
            // Assign Values To: attackPower
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed


            // Assign Values to Local Members
            this.attackPower = attackPower;
        }
    }
    class Pencil : Missile {

        public Pencil(int attackPower, Vector2 position, Vector2 origin, float speed, float rotation ) :
            base(attackPower, position, origin, Vector2.Zero, speed, FinalGame.pencilSprite.Width, FinalGame.pencilSprite.Height) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      Missle - attackPower
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            this.rotation = rotation;
            velocity.X = (float)Math.Cos(rotation) * speed;
            velocity.Y = (float)Math.Sin(rotation) * speed;

            // Assign Values to Local Members
        }

        public void update() {
            position.X += velocity.X;
            position.Y += velocity.Y;
            velocity.Y += FinalGame.GRAVITY * .15F;
            rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            //Update transform
            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position, 0.0f));
        }
    } 
    class Marker : Missile {
        public Marker(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed, float rotation ) :
            base(attackPower, position, origin, velocity, speed, FinalGame.markerSprite.Width, FinalGame.markerSprite.Height) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      Missle - attackPower
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
            //this.rotation = rotation;
            //velocity.X = (float)Math.Cos(rotation) * speed;
            //velocity.Y = (float)Math.Sin(rotation) * speed;

            // Assign Values to Local Members
            this.rotation = rotation;
        }

        public void update( float x, float y ) {
            
            //Recalculate the angle needed to hit the student & update velocities accordingly
            rotation = (float)Math.Atan2( y - position.Y, x - position.X );
            velocity.X -= (.05F*speed);
            velocity.Y += (float)Math.Sin(rotation) * (.05F*speed);

            //Update positions
            position.X += velocity.X;
            position.Y += velocity.Y;

            //Recalculate the marker's orientation depending on its velocity
            rotation = (float)Math.Atan2(velocity.Y, velocity.X);

            //Update transform
            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position, 0.0f));
        }
    }


    class Notebook : GameObject {
        public int numOfNotebook, maxBooks;

        public Notebook(Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed, FinalGame.notebookSprite.Width, FinalGame.notebookSprite.Height) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
            this.rotation = (float)FinalGame.rand.NextDouble() * MathHelper.TwoPi;
            this.rotSpeed = 0.04F;
            // Assign Values to Local Members
            numOfNotebook = 3;
            maxBooks = 3;
        }
        // Update the Position And/Or Velocity
        public void update(Vector2 studentOrigin) {
            position = studentOrigin;
            rotation += rotSpeed;
        }
        public void isDamaged() {
            numOfNotebook--;
            if (numOfNotebook <= 0) { isAlive = false; }
        }

        public void reset() {
            numOfNotebook = 3;
            isAlive = true;
        }
    }



    //class PopQuizzes : Missile {
    //    public Marker(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
    //        base(attackPower, position, origin, velocity, speed) {
    //        // Values Already Assigned To: 
    //        //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
    //        //      Missle - attackPower
    //        // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed


    //        // Assign Values to Local Members

    //    }
    //    // Update the Position And/Or Velocity
    //    public void update() {

    //    }
    //}
}
