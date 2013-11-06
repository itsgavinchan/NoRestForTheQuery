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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;
        const float GRAVITY = 0.95F;

        //Student
        Texture2D studentSprite;
        Student student;
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
            student = new Student(studentSprite, new Vector2(WINDOW_WIDTH / 2 - studentSprite.Width / 2, 200 ));

            //One platform in the middle of the screen to test
            platforms.Add(new Platform(platformSprite, new Vector2(100, 400), 600, 20));
            platforms.Add(new Platform(platformSprite, new Vector2(500, 350), 100, 50));
            platforms.Add(new Platform(platformSprite, new Vector2(500, 230), 100, 50));
            
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
                    student.collided = false;
                }

                //Lateral movement
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { student.velocity.X = -studentSpeed; }
                if (Keyboard.GetState().IsKeyDown(Keys.D)) { student.velocity.X = studentSpeed; }
                
            }

            student.position.X += student.velocity.X;
            student.position.Y += student.velocity.Y;
            student.collisionRectangle.X = (int)student.position.X;
            student.collisionRectangle.Y = (int)student.position.Y;


            //Bookkeeping
            student.velocity.Y += GRAVITY;                              //Influence of gravity
            handleStudentPlatformCollision();                           //Handle student/platform collision
            if (student.velocity.Y == 0) { student.jumping = false; }   //Reset jump state
            if (oldKeyState.IsKeyUp(Keys.A) || oldKeyState.IsKeyUp(Keys.D)) { student.velocity.X = 0; }
            oldKeyState = Keyboard.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(student.sprite, student.position, Color.White);

            for (int i = 0; i < platforms.Count; ++i)
            {
                spriteBatch.Draw(platforms[i].sprite, platforms[i].position,
                                 platforms[i].collisionRectangle, Color.Black);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected void handleStudentPlatformCollision()
        {
            int interLeft, interRight, interTop, interBot, interWidth, interHeight;
            for( int i = 0; i < platforms.Count; ++i ){
                interLeft = Math.Max(student.collisionRectangle.Left, platforms[i].collisionRectangle.Left);
                interTop = Math.Max(student.collisionRectangle.Top, platforms[i].collisionRectangle.Top);
                interRight = Math.Min(student.collisionRectangle.Right, platforms[i].collisionRectangle.Right);
                interBot = Math.Min(student.collisionRectangle.Bottom, platforms[i].collisionRectangle.Bottom);
                interWidth = interRight - interLeft;
                interHeight = interBot - interTop;

                if (interWidth >= 0 && interHeight >= 0) //If the intersecting rect is valid, they hit!
                {
                    student.collided = true;
                    if (interWidth > interHeight) //If they hit on the top/bot
                    {
                        if (student.velocity.Y > 0) { student.position.Y -= interHeight+1; }
                        else if (student.velocity.Y < 0) { student.position.Y += interHeight+2; } //+1 to height prevents "sticking"
                        student.velocity.Y = 0;
                    }
                    else if (interWidth < interHeight) //If they hit on the left/right
                    {
                        if (student.velocity.X > 0) { student.position.X -= interWidth+2; }
                        else if (student.velocity.X < 0) { student.position.X += interWidth+2; }
                        student.velocity.X = 0;
                    }
                }
            }
        }
        protected void reset()
        {
            student.position.X = WINDOW_WIDTH / 2 - studentSprite.Width / 2;
            student.position.Y = 200;
            student.velocity = Vector2.Zero;
            student.jumping = false;
            student.collided = false;
        }
    }
}
