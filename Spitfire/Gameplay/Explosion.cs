using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    class Explosion : Sprite
    {
        TimeSpan creationTime;
        int delay = 1500;
        int damage = 10;
        bool isActive;

        public Explosion(Vector2 screenPosition, GameTime gameTime)
        {
            this.Position = screenPosition;
            isActive = true;
            creationTime = gameTime.TotalGameTime;
        }

        public void update(GameTime gameTime, Vector2 playerVelocity)
        {
            this.Velocity = playerVelocity;
            if ((gameTime.TotalGameTime - creationTime).TotalMilliseconds > delay)
                this.Position -= this.Velocity;
            else
                isActive = false;
        }
        
        
        
        public int getDamage()
        {
            return damage;
        }

        public bool returnActive()
        {
            return isActive;
        }





    }
}
