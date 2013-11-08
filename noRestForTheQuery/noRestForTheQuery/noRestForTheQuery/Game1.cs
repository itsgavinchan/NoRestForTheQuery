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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;
        const float GRAVITY = 0.95F;

        //Game tools
        public static int defaultBlockSize = 50;
        public static int gameLevel;
        public static Random rand = new Random();

        //Student
        Texture2D studentSprite;
        Student1 student;
        float studentSpeed = 3.5F;

        //Testing platform array
        Texture2D platformSprite;
        List<Platform> platforms = new List<Platform>();

        //Misc
        KeyboardState oldKeyState = Keyboard.GetState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            studentSprite = Content.Load<Texture2D>(@"Sprites/simplePlayer");
            platformSprite = Content.Load<Texture2D>(@"Sprites/simplePlatform");

            //Student starts in the top center of the screen for testing
            student = new Student1( new Vector2(WINDOW_WIDTH / 2 - studentSprite.Width / 2, 200 ), 
                                    new Vector2(studentSprite.Width/2, studentSprite.Height/2),
                                    Vector2.Zero );

            //One platform in the middle of the screen to test
            for (int i = 100; i < 700; i += defaultBlockSize)
            {
                platforms.Add(new Platform(new Vector2(i, 400), Vector2.Zero, 2));
            }
            platforms.Add(new Platform(new Vector2(100, 250), Vector2.Zero, 1));
            platforms.Add(new Platform(new Vector2(250, 250), Vector2.Zero, 1));
            platforms.Add(new Platform(new Vector2(400, 350), Vector2.Zero, 1));
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive)
            {
                //Reset player
                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); }
                
                //Jumping
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space) && !student.jumping )
                {
                    student.velocity.Y = 0; //Guarantees a full jump
                    student.velocity.Y -= 15;
                    student.jumping = true;
                    student.onGround = false;
                }

                //Lateral movement
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { student.velocity.X = -studentSpeed; }
                if (Keyboard.GetState().IsKeyDown(Keys.D)) { student.velocity.X = studentSpeed; }
                
            }

            student.position.X += student.velocity.X;
            student.position.Y += student.velocity.Y;


            //Bookkeeping
            student.velocity.Y += GRAVITY;                              //Influence of gravity
            handleStudentPlatformCollision();                           //Handle student/platform collision
            if (student.velocity.Y == 0 && student.onGround) { student.jumping = false; }   //Reset jump state
            if (oldKeyState.IsKeyUp(Keys.A) || oldKeyState.IsKeyUp(Keys.D)) { student.velocity.X = 0; }
            oldKeyState = Keyboard.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            for (int i = 0; i < platforms.Count; ++i)
            {
                spriteBatch.Draw(platformSprite, platforms[i].position,
                                 platforms[i].rectangle, Color.Black);
            }

            spriteBatch.Draw(studentSprite, student.position, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected void handleStudentPlatformCollision()
        {
            int interLeft, interRight, interTop, interBot, interWidth, interHeight;
            for (int i = 0; i < platforms.Count; ++i)
            {
                interLeft = Math.Max((int)student.position.X, platforms[i].rectangle.Left);
                interTop = Math.Max((int)student.position.Y, platforms[i].rectangle.Top);
                interRight = Math.Min((int)student.position.X + studentSprite.Width, platforms[i].rectangle.Right);
                interBot = Math.Min((int)student.position.Y + studentSprite.Height, platforms[i].rectangle.Bottom);
                interWidth = interRight - interLeft;
                interHeight = interBot - interTop;

                if (interWidth >= 0 && interHeight >= 0) //If the intersecting rect is valid, they hit!
                {
                    student.colliding = true;

                    //Movement collision on ground
                    if (interHeight > interWidth)
                    {
                        //If student going to the right, stop them at the left edge of the platform
                        if (student.velocity.X > 0) { student.position.X -= interWidth; }

                        //If going to the left
                        if (student.velocity.X < 0) { student.position.X += interWidth; }

                        //They collided, so stop moving
                        student.velocity.X = 0;
                    }

                    //Vertical movement collision
                    if (interWidth > interHeight)
                    {
                        //If student is falling, stop them at the top edge of the platform (Make sure it's above the platform)
                        if (student.velocity.Y > 0 && student.position.Y < platforms[i].position.Y)
                        {
                            student.position.Y -= interHeight; 
                            student.onGround = true;
                            student.velocity.Y = 0;
                        }

                        //If student is going upward from a jump, put them down (w/ a little force)
                        if (student.velocity.Y < 0) 
                        { 
                            student.position.Y += interHeight; 
                            student.velocity.Y *= -.1F; 
                        }
                    }
                }
                else { student.colliding = false; }
            }
        }
        protected void reset()
        {
            student.position.X = WINDOW_WIDTH / 2 - studentSprite.Width / 2;
            student.position.Y = 200;
            student.velocity = Vector2.Zero;
            student.jumping = false;
            student.colliding = false;
        }
    }
}
