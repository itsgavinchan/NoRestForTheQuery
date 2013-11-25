using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace noRestForTheQuery {
    public class AnimatedSprite {
        public Texture2D spriteTexture;
        float timer, interval;
        public int previousFrame, currentFrame, width, height;
        Rectangle sourceRectangle;
        Vector2 origin;

        public AnimatedSprite(Texture2D texture, int currentFrame = 0, int spriteWidth = 50, int spriteHeight = 50)
        {
	        this.spriteTexture = texture;
            this.timer = 0F;
            this.interval = 200F;
	        this.currentFrame = currentFrame;
	        this.width = spriteWidth;
	        this.height = spriteHeight;
            sourceRectangle = new Rectangle( currentFrame * width, 0, width, height );
        }

        public Vector2 Origin       { get { return origin; }            set { origin = value; } }
        public Texture2D Texture    { get { return spriteTexture; }     set { spriteTexture = value; } }
        public Rectangle SourceRect { get { return sourceRectangle; }   set { sourceRectangle = value; } }
        
        //NOTE: For all sprite sheets
        //    0 is facing left
        //  1-3 is moving left
        //    4 is facing right
        //  5-7 is moving right

        public void animateLeft( KeyboardState keyState, KeyboardState oldKeyState, GameTime gameTime ){
            if( keyState != oldKeyState ){ currentFrame = 0; }

            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if( timer > interval ){
                currentFrame++;
                if( currentFrame > 3 ){ currentFrame = 1; }
                previousFrame = currentFrame;
                timer = 0F;
            }
        }

        public void animateRight( KeyboardState keyState, KeyboardState oldKeyState, GameTime gameTime ){
            if( keyState != oldKeyState ){ currentFrame = 4; }
            
            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if( timer > interval ){
                currentFrame++;
                if( currentFrame > 7 ){ currentFrame = 5; }
                previousFrame = currentFrame;
                timer = 0F;
            }
        }
    }
}
