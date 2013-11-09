using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {

    class Homework : DamagableObject {
        public Homework(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity, 0) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 50 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 10 * FinalGame.gameLevel;

            // Assign Values to Local Members
            speed = (float)(FinalGame.rand.Next(0, 3) + FinalGame.rand.NextDouble());

        }
        // Update the Position And/Or Velocity
        public void update() {
            position.X -= speed;
        }
    }
    class Midterm : DamagableObject {
        public Midterm(Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 100 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 25 * FinalGame.gameLevel;

            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update() {

        }

    }
    class Final : DamagableObject {
        public Final(Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 200 * FinalGame.gameLevel;
            currentHealth = fullHealth;
            attackPower = 50 * FinalGame.gameLevel;

            // Assign Values to Local Members

        }

        // Update the Position And/Or Velocity
        public void update() {

        }
    }
    class Professor : GameObject {
        public Professor(Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed) { }

        // Update the Position And/Or Velocity
        public void update() {

        }
    }
}
