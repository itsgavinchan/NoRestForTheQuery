using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery
{
    public class GameObject
    {
        public bool isAlive = true;
        public Vector2 position, origin, velocity;
        public Color[] colorArr;
        public float rotation, rotSpeed;
        public GameObject(Vector2 position, Vector2 origin, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
            this.origin = origin;
        }
    }

    class Platform : GameObject
    {
        public int blockSize;
        public Rectangle rectangle;
        public Platform(Vector2 position, Vector2 velocity, int blockType)
            : base(position, 
                   new Vector2((Game1.defaultBlockSize / 2) * blockType, (Game1.defaultBlockSize / 2) * blockType), 
                   velocity) 
        {
            blockSize = blockType;
            rectangle = new Rectangle((int)position.X, (int)position.Y, Game1.defaultBlockSize * blockType, Game1.defaultBlockSize * blockType);
        }
    }

    class DamagableObject : GameObject
    {
        protected int currentHealth, fullHealth, attackPower;
        public DamagableObject(Vector2 position, Vector2 origin, Vector2 velocity)
            : base(position, origin, velocity) { }

        public bool checkDeath()
        {
            if (currentHealth <= 0) { isAlive = false; }
            return isAlive;
        }

        public void incrementHealth(int restore) { if (isAlive) currentHealth += restore; }
        public void decrementHealth(int damage) { if (isAlive) currentHealth -= damage; checkDeath(); }
        public void incrementAttack(int boost) { if (isAlive) attackPower += boost; }

    }

    class Student1 : DamagableObject
    {
        double sanity;
        int attackMultiplier, sleepIncrement, budget;
        public bool onGround, jumping, colliding;

        public Student1(Vector2 position, Vector2 origin, Vector2 velocity)
            : base(position, origin, velocity)
        {
            // Assign Values To:
            // currentHealth, fullHealth, attackPower
            // Still Missing rotation, rotSpeed, colorArr
            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10;

            // Assign Values to Local Members
            sleepIncrement = 50;
            sanity = 1.00;
            budget = 200;
        }

        // Weekend Option Effects
        public void studyEffect() { attackPower += Game1.gameLevel * 10; }
        public void sleepEffect() { fullHealth += sleepIncrement; }
        public void foodEffect() { currentHealth = fullHealth; }
        public void allowanceEffect() { budget += Game1.rand.Next(50, 70); }
        public void socialEffect()
        {
            double increaseAmt = Game1.rand.NextDouble() * 0.5;
            if (increaseAmt <= 0.2) { increaseAmt = 0.2; }
            else if (increaseAmt >= 0.4) { increaseAmt = 0.4; }
            sanity += increaseAmt;
            if (sanity > 1.0) { sanity = 1.0; }
        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }

    class Homework : DamagableObject
    {
        public Homework(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity)
        {
            // Assign Values To:
            // currentHealth, fullHealth, attackPower
            // Still Missing rotation, rotSpeed, colorArr
            fullHealth = 50 * Game1.gameLevel;
            currentHealth = fullHealth;
            attackPower = 10 * Game1.gameLevel;
            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class Midterm : DamagableObject
    {
        public Midterm(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity)
        {
            // Assign Values To:
            // currentHealth, fullHealth, attackPower
            // Still Missing rotation, rotSpeed, colorArr
            fullHealth = 100 * Game1.gameLevel;
            currentHealth = fullHealth;
            attackPower = 25 * Game1.gameLevel;
            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }

    }
    class Final : DamagableObject
    {
        public Final(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity)
        {
            // Assign Values To:
            // currentHealth, fullHealth, attackPower
            // Still Missing rotation, rotSpeed, colorArr
            fullHealth = 200 * Game1.gameLevel;
            currentHealth = fullHealth;
            attackPower = 50 * Game1.gameLevel;
            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class Professor : GameObject
    {
        public Professor(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity) { }

        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class Missile : GameObject
    {
        protected int attackPower;

        public Missile(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity)
        {
            // Assign Values To: attackPower
            // Still Missing rotation, rotSpeed, colorArr
            this.attackPower = attackPower;
            // Assign Values to Local Members


        }
    }
    class Pencil : Missile
    {
        public Pencil(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity) :
            base(attackPower, position, origin, velocity)
        {
            // Assign Values To: attackPower
            // Still Missing rotation, rotSpeed, colorArr

            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class Marker : Missile
    {
        public Marker(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity) :
            base(attackPower, position, origin, velocity)
        {
            // Assign Values To: attackPower
            // Still Missing rotation, rotSpeed, colorArr

            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class PopQuiz : Missile
    {
        public PopQuiz(int attackPower, Vector2 position, Vector2 origin, Vector2 velocity) :
            base(attackPower, position, origin, velocity)
        {
            // Assign Values To:
            // attackPower
            // Still Missing rotation, rotSpeed, colorArr

            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
    }
    class Notebook : GameObject
    {
        int numOfNotebook;

        public Notebook(Vector2 position, Vector2 origin, Vector2 velocity) :
            base(position, origin, velocity)
        {
            // Assign Values To:
            // numOfNotebook
            // Still Missing rotation, rotSpeed, colorArr
            numOfNotebook = 3;
            // Assign Values to Local Members

        }
        // Update the Position And/Or Velocity
        public void update()
        {

        }
        void isDamaged()
        {
            numOfNotebook--;
            if (numOfNotebook <= 0) { isAlive = false; }
        }
    }

}
