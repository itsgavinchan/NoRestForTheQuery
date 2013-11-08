using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace noRestForTheQuery
{
    public class FinalGame : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public const int WINDOW_WIDTH = 1200;
        public const int WINDOW_HEIGHT = 600;
        public const float GRAVITY = 0.95F;
        const float HOMEWORKSPAWNPROB = 0.05F;
        const int MAXHOMEWORK = 5;

        // Stand-By Map
        public static int PENCILCOST = 5;
        public static int SLEEPINCREMENT = 50;

        //Game tools
        public static int defaultBlockSize = 50;
        public static int gameLevel;
        public static Random rand = new Random();

        //Student
        Student1 student;
        //float studentSpeed = 3.5F;
        

        //Testing platform array
        List<Platform> platforms = new List<Platform>();
        
        // Stuffs
        public static Texture2D platformSprite, studentSprite, pencilSprite, markerSprite, homeworkSprite, midtermSprite, finalSprite, notebookSprite, professorSprite;
        List<Homework> homeworks = new List<Homework>();

        //Misc
        KeyboardState lastKeyState = Keyboard.GetState();
        MouseState lastMouseState = Mouse.GetState();

        public FinalGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            IsMouseVisible = true;
        }
        protected override void Initialize() { base.Initialize(); }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            studentSprite = Content.Load<Texture2D>(@"Sprites/player");
            platformSprite = Content.Load<Texture2D>(@"Sprites/platform");
            pencilSprite = Content.Load<Texture2D>(@"Sprites/pencil");
            markerSprite = Content.Load<Texture2D>(@"Sprites/marker");
            homeworkSprite = Content.Load<Texture2D>(@"Sprites/homework");
            midtermSprite = Content.Load<Texture2D>(@"Sprites/midterm");
            finalSprite = Content.Load<Texture2D>(@"Sprites/final");
            notebookSprite = Content.Load<Texture2D>(@"Sprites/notebook");
            professorSprite = Content.Load<Texture2D>(@"Sprites/professor");


            //Student starts in the top center of the screen for testing
            student = new Student1( new Vector2(WINDOW_WIDTH / 2 - studentSprite.Width / 2, 200 ),  // Position
                                    new Vector2(studentSprite.Width/2, studentSprite.Height/2),     // Origin
                                    Vector2.Zero, 3.5F);                                               // Velocity, speed

            //One platform in the middle of the screen to test
            for (int i = 100; i < 700; i += defaultBlockSize) {
                platforms.Add(new Platform(new Vector2(i, 400), Vector2.Zero, 2, 0));
            }
            platforms.Add(new Platform(new Vector2(100, 250), Vector2.Zero, 1, 0));
            platforms.Add(new Platform(new Vector2(250, 250), Vector2.Zero, 1, 0));
            platforms.Add(new Platform(new Vector2(400, 350), Vector2.Zero, 1, 0));

            

        }
        protected override void UnloadContent() { }
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive) {
                // Randomly Spawn Homework For Now
                if ( homeworks.Count() < MAXHOMEWORK && rand.NextDouble() > HOMEWORKSPAWNPROB ) { 
                    homeworks.Add(  new Homework(   new Vector2( WINDOW_WIDTH, rand.Next( 0, WINDOW_HEIGHT - homeworkSprite.Height ) ), 
                                                    new Vector2( homeworkSprite.Width/2, homeworkSprite.Height/2 ),
                                                    Vector2.Zero) );
                }
                // Reset the Game
                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); }
                
                // Player Jumps
                if (Keyboard.GetState().IsKeyDown(Keys.W) && lastKeyState.IsKeyUp(Keys.W) && !student.jumping ) { student.jump(); }

                // Left-Right Movement
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { student.velocity.X = -student.speed; }
                if (Keyboard.GetState().IsKeyDown(Keys.D)) { student.velocity.X = student.speed; }

                // Player Shoots
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) {
                    double x = (Mouse.GetState().X - (student.position.X + studentSprite.Width / 2));
                    double y = (Mouse.GetState().Y - (student.position.Y + studentSprite.Height / 2));
                    student.shoot((float)(Math.Atan2(y, x)));
                }
            }

            // Update Object Positions
            student.update();

            int index = 0;
            while( index < homeworks.Count() ) {
                homeworks[index].update();
                if (homeworks[index].checkBoundaries(homeworkSprite.Width, homeworkSprite.Height)) { homeworks.RemoveAt(index); }
                else { index++; }
            }

            index = 0;
            while (index < student.pencils.Count()) {
                student.pencils[index].update();
                if (student.pencils[index].checkBoundaries(pencilSprite.Width, pencilSprite.Height)) { student.pencils.RemoveAt(index); }
                else { index++; }
            }


            //Bookkeeping
            student.velocity.Y += GRAVITY;                              //Influence of gravity
            handleStudentPlatformCollision();                           //Handle student/platform collision
            if (student.velocity.Y == 0 && student.onGround) { student.jumping = false; }   //Reset jump state
            if (lastKeyState.IsKeyUp(Keys.Left) || lastKeyState.IsKeyUp(Keys.Right)) { student.velocity.X = 0; }
            lastKeyState = Keyboard.GetState();
            lastMouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // Color Coordinating
            // Platform = Black
            // Student = Red
            // Notebook = White
            // Pencil = Yellow
            // Homework = Orange

            for (int i = 0; i < platforms.Count; ++i) { spriteBatch.Draw(platformSprite, platforms[i].position, platforms[i].rectangle, Color.Black); }
            for (int i = 0; i < homeworks.Count(); i++) { spriteBatch.Draw(homeworkSprite, homeworks[i].position, Color.Orange); }
            for (int i = 0; i < student.pencils.Count(); i++) { spriteBatch.Draw(pencilSprite, student.pencils[i].position, null, Color.White, student.pencils[i].rotation, student.pencils[i].origin, 1.0F, SpriteEffects.None, 0.0F); }
            if (student.notebook.isAlive) { spriteBatch.Draw(notebookSprite, student.notebook.position, null, Color.White, student.notebook.rotation, student.notebook.origin, 1.0F, SpriteEffects.None, 0.0F); }
            spriteBatch.Draw(studentSprite, student.position, Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        public bool purchasePencils(int quantity) {
            if (quantity * PENCILCOST > student.budget) { return false; }
            else { student.amtPencil += quantity; student.budget -= quantity * PENCILCOST; return true; }
        }
        protected void handleStudentPlatformCollision() {
            int interLeft, interRight, interTop, interBot, interWidth, interHeight;
            for (int i = 0; i < platforms.Count; ++i) {
                interLeft = Math.Max((int)student.position.X, platforms[i].rectangle.Left);
                interTop = Math.Max((int)student.position.Y, platforms[i].rectangle.Top);
                interRight = Math.Min((int)student.position.X + studentSprite.Width, platforms[i].rectangle.Right);
                interBot = Math.Min((int)student.position.Y + studentSprite.Height, platforms[i].rectangle.Bottom);
                interWidth = interRight - interLeft;
                interHeight = interBot - interTop;

                if (interWidth >= 0 && interHeight >= 0) {  //If the intersecting rect is valid, they hit!
               
                    student.colliding = true;

                    //Movement collision on ground
                    if (interHeight > interWidth) {
                        //If student going to the right, stop them at the left edge of the platform
                        if (student.velocity.X > 0) { student.position.X -= interWidth; }

                        //If going to the left
                        if (student.velocity.X < 0) { student.position.X += interWidth; }

                        //They collided, so stop moving
                        student.velocity.X = 0;
                    }

                    //Vertical movement collision
                    if (interWidth > interHeight) {
                        //If student is falling, stop them at the top edge of the platform (Make sure it's above the platform)
                        if (student.velocity.Y > 0 && student.position.Y < platforms[i].position.Y) {
                            student.position.Y -= interHeight; 
                            student.onGround = true;
                            student.velocity.Y = 0;
                        }

                        //If student is going upward from a jump, put them down (w/ a little force)
                        if (student.velocity.Y < 0) { 
                            student.position.Y += interHeight; 
                            student.velocity.Y *= -.1F; 
                        }
                    }
                }
                else { student.colliding = false; }
            }
        }
        protected void reset() {
            homeworks.Clear();
            student.reset();
        }
    }
}
