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
        public float rotation, rotSpeed, speed;
        public GameObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed) {
            this.position = position;
            this.velocity = velocity;
            this.origin = origin;
            this.speed = speed;
            this.rotation = 0;
        }

        public bool checkBoundaries( int width, int height ) {
            if (position.X <= -width) { return true; }
            if (position.X >= FinalGame.WINDOW_WIDTH + width) { return true; }
            if (position.Y <= -height) { return true; }
            if (position.Y >= FinalGame.WINDOW_HEIGHT + height) { return true; }
            return false;
        }
    }

    class Platform : GameObject {
        public int blockSize;
        public Rectangle rectangle;
        public Platform(Vector2 position, Vector2 velocity, int blockType, float speed)
            : base(position, 
                   new Vector2((FinalGame.defaultBlockSize / 2) * blockType, (FinalGame.defaultBlockSize / 2) * blockType), 
                   velocity, speed) 
        {
            blockSize = blockType;
            rectangle = new Rectangle((int)position.X, (int)position.Y, FinalGame.defaultBlockSize * blockType, FinalGame.defaultBlockSize * blockType);
        }
    }

    class DamagableObject : GameObject {
        protected int currentHealth, fullHealth, attackPower;
        public DamagableObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed)
            : base(position, origin, velocity, speed) { }

        public bool checkDeath() {
            if (currentHealth <= 0) { isAlive = false; }
            return isAlive;
        }

        public void incrementHealth(int restore) { if (isAlive) currentHealth += restore; }
        public void decrementHealth(int damage) { if (isAlive) currentHealth -= damage; checkDeath(); }
        public void incrementAttack(int boost) { if (isAlive) attackPower += boost; }

    }

    class Student1 : DamagableObject {
        double sanity;
        public int budget, amtPencil;
        public bool onGround, jumping, colliding;
        public List<Pencil> pencils;
        public Notebook notebook;

        public Student1(Vector2 position, Vector2 origin, Vector2 velocity, float speed)
            : base(position, origin, velocity, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10;

            // Assign Values to Local Members: sanity, budget, amtPencil, pencils, notebook
            pencils = new List<Pencil>();
            notebook = new Notebook( position , new Vector2( FinalGame.notebookSprite.Width/2, FinalGame.notebookSprite.Height/2 ), Vector2.Zero, speed );
            amtPencil = 50;
            sanity = 1.00;
            budget = 200;

        }

        public void reset() {
            position.X = FinalGame.WINDOW_WIDTH / 2 - FinalGame.studentSprite.Width / 2;
            position.Y = 200;
            velocity = Vector2.Zero;
            pencils.Clear();
            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10; 
            amtPencil = 50;
            sanity = 1.00;
            budget = 200;
        }

        // Weekend Option Effects
        public void studyEffect() { attackPower += FinalGame.gameLevel * 10; }
        public void sleepEffect() { fullHealth += FinalGame.SLEEPINCREMENT; }
        public void foodEffect() { currentHealth = fullHealth; }
        public void allowanceEffect() { budget += FinalGame.rand.Next(50, 70); }
        public void socialEffect() {
            double increaseAmt = FinalGame.rand.NextDouble() * 0.5;
            if (increaseAmt <= 0.2) { increaseAmt = 0.2; }
            else if (increaseAmt >= 0.4) { increaseAmt = 0.4; }
            sanity += increaseAmt;
            if (sanity > 1.0) { sanity = 1.0; }
        }

        // Map Options / Actions
        public void jump() {
            velocity.Y = 0; //Guarantees a full jump
            velocity.Y -= 15;
            jumping = true;
            onGround = false;
        }

        public void shoot( float rotation ) {
            if (amtPencil > 0) {
                pencils.Add(new Pencil(attackPower, new Vector2(position.X + FinalGame.studentSprite.Width/2, position.Y + FinalGame.studentSprite.Height/2),
                    new Vector2(FinalGame.pencilSprite.Width / 2, FinalGame.pencilSprite.Height/ 2), 10, rotation)); // ten is the speed of pencil
                amtPencil--;
            }
        }

        // Update the Position And/Or Velocity
        public void update() {
            position.X += velocity.X;
            position.Y += velocity.Y;
            notebook.update(position + origin);
        }

        
    }

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
            speed = (float) ( FinalGame.rand.Next(0, 3) + FinalGame.rand.NextDouble());

        }
        // Update the Position And/Or Velocity
        public void update( ) {
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
    class Missile : GameObject {
        protected int attackPower;

        public Missile(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed) {
            // Assign Values To: attackPower
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            
            // Assign Values to Local Members
            this.attackPower = attackPower;

        }


    }
    class Pencil : Missile {
        
        public Pencil(int attackPower, Vector2 position, Vector2 origin, float speed, float rotation) :
            base(attackPower, position, origin, Vector2.Zero, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      Missle - attackPower
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed

            this.rotation = rotation;
            velocity.X = (float)Math.Cos(rotation) * speed;
            velocity.Y = (float)Math.Sin(rotation) * speed;

            // Assign Values to Local Members
        }
        // Update the Position And/Or Velocity
        public void update() {
            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }
    class Marker : Missile {
        public Marker(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(attackPower, position, origin, velocity, speed)
        {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      Missle - attackPower
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed


            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update() {

        }
    }
    class PopQuiz : Missile {
        public PopQuiz(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(attackPower, position, origin, velocity, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      Missle - attackPower
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed


            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update() {

        }
    }
    class Notebook : GameObject {
        public int numOfNotebook;

        public Notebook(Vector2 position, Vector2 origin, Vector2 velocity, float speed) :
            base(position, origin, velocity, speed) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
                this.rotation = (float) FinalGame.rand.NextDouble() * MathHelper.TwoPi;
                this.rotSpeed = 0.04F;
            // Assign Values to Local Members
            numOfNotebook = 3;
        }
        // Update the Position And/Or Velocity
        public void update( Vector2 studentOrigin ) {
            position = studentOrigin;
            rotation += rotSpeed;
        }
        void isDamaged() {
            numOfNotebook--;
            if (numOfNotebook <= 0) { isAlive = false; }
        }
    }

}
