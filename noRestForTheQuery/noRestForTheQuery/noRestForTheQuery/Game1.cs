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
        const float GRAVITY = 9.81F;

        //Student
        Texture2D studentSprite;
        Student student;

        //Testing platform array
        Texture2D platformSprite;
        List<Platform> platforms = new List<Platform>();

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
            student = new Student(studentSprite, new Vector2(WINDOW_WIDTH / 2 - studentSprite.Width / 2, 0 ));

            //One platform in the middle of the screen to test
            platforms.Add( new Platform( platformSprite, 
                                         new Vector2( 100, 400 ),
                                         600, 10) );
            
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            student.position.X += student.velocity.X;
            student.position.Y += student.velocity.Y;
            handleStudentPlatformCollision();

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
        }
    }
}
