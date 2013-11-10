using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {
    class Student1 : DamagableObject {
        double sanity;
        int pencilSpeed;
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

            // Assign Values to Local Members: sanity, budget, amtPencil, pencils, notebook, pencilSpeed
            pencilSpeed = 15;
            pencils = new List<Pencil>();
            notebook = new Notebook(position, new Vector2(FinalGame.notebookSprite.Width / 2, FinalGame.notebookSprite.Height / 2), Vector2.Zero, speed);
            amtPencil = 50;
            sanity = 1.00;
            budget = 200;

        }

        public void reset() {
            position.X = FinalGame.WINDOW_WIDTH / 2 - FinalGame.studentSprite.Width / 2;
            position.Y = 200;
            velocity = Vector2.Zero;
            pencils.Clear();
            notebook.isAlive = true;
            notebook.numOfNotebook = 3;
            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10;
            amtPencil = 50;
            sanity = 1.00;
            budget = 200;
        }

        // Override Check Boundaries
        public bool checkBoundaries() {
            if (position.X <= FinalGame.screenOffset) { return true; }
            if (position.X >= (FinalGame.WINDOW_WIDTH + FinalGame.screenOffset)) { return true; }
            if (position.Y <= 0) { return true; }
            if (position.Y >= FinalGame.WINDOW_HEIGHT) { return true; }
            return false;
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

        public void shoot(float rotation) {
            if (amtPencil > 0) {
                pencils.Add(new Pencil(attackPower, new Vector2(position.X + FinalGame.studentSprite.Width / 2, position.Y + FinalGame.studentSprite.Height / 2),
                    new Vector2(FinalGame.pencilSprite.Width / 2, FinalGame.pencilSprite.Height / 2), pencilSpeed, rotation)); // ten is the speed of pencil
                amtPencil--;
            }
        }

        // Update the Position And/Or Velocity
        public void update() {
            if (position.X <= FinalGame.screenOffset) { position.X = FinalGame.screenOffset; }
            else if (position.X + FinalGame.studentSprite.Width >= FinalGame.WINDOW_WIDTH + FinalGame.screenOffset) { position.X = FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - FinalGame.studentSprite.Width; }
            else { position.X += velocity.X; }
            position.Y += velocity.Y; 
            velocity.Y += FinalGame.GRAVITY;
            notebook.update(position + origin);
        }


    }
}
