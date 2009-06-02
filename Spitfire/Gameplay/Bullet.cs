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
        private float maxBulletDistance = 700f; // max distance bullet travels before disappearing
        
        /*
        public int bulletDamage
        {
            get { return damage; }
            set { damage = value; }
        }
        private int damage = 10;// default
        */
         
        private Vector2 initialVelocity = new Vector2(30f, 30f);


        public Bullet(float shootersRotation, Vector2 initialPosition, FaceDirection direction)
        {
            this.faceDirection = direction;
            base.Rotation = shootersRotation;
            determineVelocity();            
            base.Position = determinePosition(initialPosition);
            distanceTravelled = 0f;
        }

        private void determineVelocity()
        {            
            float yPercent = (float)Math.Sin(Rotation);
            float xPercent = 1 - Math.Abs(yPercent);
            determineFaceDirection();
            Velocity = new Vector2(initialVelocity.X * xPercent * (float)faceDirection, initialVelocity.Y * yPercent);

        }

        private Vector2 determinePosition(Vector2 initialPosition)
        {
            float yPercent = (float)Math.Sin(Rotation);
            float xPercent = 1 - Math.Abs(yPercent);            
            float planeHalfLength = 0f;
            if (faceDirection == FaceDirection.Left)
                planeHalfLength = -10;
            return new Vector2(initialPosition.X += (planeHalfLength * xPercent + 3f * (float)faceDirection),
                initialPosition.Y += (planeHalfLength * yPercent));

        }

        private void determineFaceDirection()
        {
            if (Math.Cos(Rotation) < 0f)
                faceDirection = FaceDirection.Left;
            else if (Math.Cos(Rotation) > 0f)
                faceDirection = FaceDirection.Right;
        }



        public virtual void Update(Vector2 playerVelocity)
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
