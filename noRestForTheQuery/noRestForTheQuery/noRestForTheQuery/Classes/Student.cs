using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {
    class Student1 : DamagableObject {
        public AnimatedSprite sprite;
        public double sanity, experience = 0;
        int pencilSpeed;
        public int budget, amtPencil;
        public bool onGround, jumping, colliding;
        public List<Pencil> pencils;
        public Notebook notebook;

        public Student1( ref AnimatedSprite sprite, Vector2 position, Vector2 origin, Vector2 velocity, float speed)
            : base(position, origin, velocity, speed, FinalGame.studentWidth, FinalGame.studentHeight) {
            // Values Already Assigned To: 
            //      GameObject - bool isAlive, Vector2 position, Vector2 origin, Vector2 velocity, float speed
            //      DamagableObject - int currentHealth, int fullHealth, int attackPower;
            // Empty Values To Be Assigned: Color[] colorArr, float rotation, float rotSpeed
            
            this.sprite = sprite;
            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10;

            // Assign Values to Local Members: sanity, budget, amtPencil, pencils, notebook, pencilSpeed
            pencilSpeed = 15;
            pencils = new List<Pencil>();
            notebook = new Notebook(position, new Vector2(FinalGame.notebookSprite.Width / 2, FinalGame.notebookSprite.Height / 2), Vector2.Zero, speed);
            amtPencil = 150;
            sanity = 1.00;
            budget = 200;
        }

        public void gainExperience() { 
            experience += 1.0 / ( FinalGame.gameLevel * 10.0 );
            if (experience > 1.0) {
                attackPower += FinalGame.gameLevel*2;
                experience = 0;
            }
        }

        public void reset() {
            isAlive = true;
            position.X = FinalGame.WINDOW_WIDTH / 2 - FinalGame.studentSprite.Width / 2;
            position.Y = 200;
            velocity = Vector2.Zero;
            pencils.Clear();
            notebook.reset();
            fullHealth = 100;
            currentHealth = fullHealth;
            attackPower = 10;
            amtPencil = 150;
            sanity = 1.00;
            budget = 200;
            experience = 0;
        }

        // Override Check Boundaries
        public bool checkBoundaries() {
            if (position.X <= FinalGame.screenOffset - FinalGame.studentSprite.Width) { return true; }
            if (position.X >= (FinalGame.WINDOW_WIDTH + FinalGame.screenOffset)) { return true; }
            return false;
        }

        // Weekend Option Effects
        public void studyEffect() { attackPower += FinalGame.gameLevel * 5; }
        public void sleepEffect() { fullHealth += FinalGame.SLEEPINCREMENT; }
        public void foodEffect() { currentHealth = fullHealth; }
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
            velocity.Y -= 17;
            jumping = true;
            onGround = false;
        }

        public void shoot(float rotation) {
            if (amtPencil > 0) {
                pencils.Add(new Pencil(attackPower, new Vector2(position.X + FinalGame.studentSprite.Width / 2, position.Y + FinalGame.studentSprite.Height / 2),
                    new Vector2(FinalGame.pencilSprite.Width / 2, FinalGame.pencilSprite.Height / 2), pencilSpeed, rotation)); // ten is the speed of pencil
                pencils.Last().colorArr = new Color[ FinalGame.pencilSprite.Width * FinalGame.pencilSprite.Height ];
                FinalGame.pencilSprite.GetData<Color>( pencils.Last().colorArr );
                amtPencil--;
            }
        }

        // Update the Position And/Or Velocity
        public void update() {
            if (position.X + FinalGame.studentSprite.Width > FinalGame.WINDOW_WIDTH + FinalGame.screenOffset) { position.X = FinalGame.WINDOW_WIDTH + FinalGame.screenOffset - FinalGame.studentSprite.Width; }
            else { position.X += velocity.X; }
            position.Y += velocity.Y; 
            velocity.Y += FinalGame.GRAVITY;
            notebook.update(position + origin);

            //Note: The last translation MUST add their origin again since they are drawn at their origin point.
            //Without this addition, collisions will be wrong
            this.transform = 
                Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                Matrix.CreateRotationZ(this.rotation) *
                Matrix.CreateTranslation(new Vector3(this.position + this.origin, 0.0f));
            sprite.Texture.GetData<Color>( 0, sprite.SourceRect, colorArr,
                                           sprite.currentFrame*width, 
                                           width * height );
            
        }


    }
}