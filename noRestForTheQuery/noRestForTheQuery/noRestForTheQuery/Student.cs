using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery
{
    class Student
    {
        public Texture2D sprite;
        public Vector2 position;
        public Vector2 velocity;
        public Rectangle collisionRectangle;

        public Student(Texture2D studentSprite, Vector2 startPos)
        {
            sprite = studentSprite;
            position = startPos;
            velocity = Vector2.Zero;
            collisionRectangle = new Rectangle( (int)position.X, (int)position.Y, sprite.Width, sprite.Height);
        }
        
    }
}
