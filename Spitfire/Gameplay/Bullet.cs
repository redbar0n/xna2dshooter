using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    public class Bullet : Sprite
    {

        private float distanceTravelled;
        private float maxBulletDistance = 1200f; // max distance bullet travels before disappearing
        private int damage = 10;// default
        public int bulletDamage
        {
            get { return damage; }
            set { damage = value; }
        }


        private float pi = (float)Math.PI;
        private Vector2 initialVelocity = new Vector2(20f, 20f);


        public Bullet(float shootersRotation, Vector2 initialPosition)
        {
            base.Rotation = shootersRotation;
            determineVelocity();
            base.Position = initialPosition + new Vector2(20f, -8f);
            distanceTravelled = 0f;
        }

        private void determineVelocity()
        {
            float yPercent = Rotation / (pi / 2);
            float xPercent = 1 - Math.Abs(yPercent);

            if (Rotation > 0)
            {
                //accelerant += accelerationConstant * yPercent;
                Velocity = new Vector2(initialVelocity.X * xPercent, initialVelocity.Y * yPercent);
            }
            else
            {
                Velocity = new Vector2(initialVelocity.X * xPercent, initialVelocity.Y * yPercent);
            }
        }

        public void Update(Vector2 playerVelocity)
        {
            base.Position += (Velocity - playerVelocity);
            distanceTravelled += (float)Math.Sqrt(Math.Pow(Math.Abs(Velocity.X), 2) +
                Math.Pow(Math.Abs(Velocity.Y), 2));
        }

        public bool HasExceededDistance()
        {
            return (distanceTravelled >= maxBulletDistance);
        }

    }
}
