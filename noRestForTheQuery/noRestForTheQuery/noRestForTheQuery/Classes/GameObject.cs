using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace noRestForTheQuery {
    public class GameObject {
        public bool isAlive = true;
        public Vector2 position, origin, velocity;
        public Color[] colorArr;
        public float rotation, rotSpeed, speed;
        public GameObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed) {
            this.position = position;
            this.velocity = velocity;
            this.origin = origin;
            this.speed = speed;
            this.rotation = 0;
        }

        public bool checkBoundaries( int width, int height ) {
            if (position.X <= -width) { return true; }
            if (position.X >= FinalGame.WINDOW_WIDTH + width) { return true; }
            if (position.Y <= -height) { return true; }
            if (position.Y >= FinalGame.WINDOW_HEIGHT + height) { return true; }
            return false;
        }
    }

    class Platform : GameObject {
        public int blockSize;
        public Rectangle rectangle;
        public Platform(Vector2 position, Vector2 velocity, int blockType, float speed)
            : base(position, 
                   new Vector2((FinalGame.defaultBlockSize / 2) * blockType, (FinalGame.defaultBlockSize / 2) * blockType), 
                   velocity, speed) 
        {
            blockSize = blockType;
            rectangle = new Rectangle((int)position.X, (int)position.Y, FinalGame.defaultBlockSize * blockType, FinalGame.defaultBlockSize * blockType);
        }
    }

    class DamagableObject : GameObject {
        protected int currentHealth, fullHealth, attackPower;
        public DamagableObject(Vector2 position, Vector2 origin, Vector2 velocity, float speed)
            : base(position, origin, velocity, speed) { }

        public bool checkDeath() {
            if (currentHealth <= 0) { isAlive = false; }
            return isAlive;
        }

        public void incrementHealth(int restore) { if (isAlive) currentHealth += restore; }
        public void decrementHealth(int damage) { if (isAlive) currentHealth -= damage; checkDeath(); }
        public void incrementAttack(int boost) { if (isAlive) attackPower += boost; }

    }

    


}
