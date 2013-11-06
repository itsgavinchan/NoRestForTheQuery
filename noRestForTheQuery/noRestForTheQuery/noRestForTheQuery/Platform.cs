﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery
{
    public class Platform
    {
        public Texture2D sprite;
        public Vector2 position;
        public int width;
        public int height;
        public Rectangle collisionRectangle;

        public Platform(Texture2D platformSprite, Vector2 startPos, int newWidth = 100, int newHeight = 10)
        {
            sprite = platformSprite;
            position = startPos;
            width = newWidth;
            height = newHeight;
            collisionRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
        }
    }
}