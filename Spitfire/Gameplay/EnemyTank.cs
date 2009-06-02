using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
    class EnemyTank: Enemy
    {
        private float pi = (float)Math.PI;
        private Vector2 initialVelocity = new Vector2(4f, 0f);

        /// <summary>
        /// The rotation of the tank's barrel to determine the direction of shooting.
        /// </summary>
        private float barrelRotation;
        /// <summary>
        /// The the location of the tank's barrel on screen
        /// </summary>
        private Vector2 barrelLocation;


        private int burstAmmount = 1; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Value set in constructor
        private int burstDelay = 150; // The Delay in miliseconds between each shot        
        TimeSpan bulletCreationTime;


        private bool isShooting = false;

        public EnemyTank(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping)
        {


            //TODO Fill in details about statistics
            this.faceDirection = FaceDirection.Left;
            barrelRotation = (0 + pi);
            barrelLocation = base.Position;
            //barrelLocation = new Vector2(barrelLocation.X, barrelLocation.Y + 10f);
        }



        //TODO add other overloading methods including update, shoot? etc



        /// <summary>
        /// Determines the direction will move based on rotation. NOTE: Might seem strange and pointless.
        /// </summary>
        private void determineFaceDirection()
        {

            if (Math.Cos(Rotation) > 0f)
                faceDirection = FaceDirection.Left;
            else if (Math.Cos(Rotation) < 0f)
                faceDirection = FaceDirection.Right;
        }

        private void determineVelocity() 
        {
            if (faceDirection == FaceDirection.Left)
            {
                base.Velocity = new Vector2(initialVelocity.X * -1, 0f);
            }
            else
                base.Velocity = initialVelocity;
        
        }


        public void Shoot()
        {
            if (faceDirection == FaceDirection.Left)
            {
                barrelLocation = new Vector2(base.Position.X + 5f, base.Position.Y);
                barrelRotation = (1.05f * pi);
            }
            else
            {
                barrelLocation = new Vector2(base.Position.X + 20f, base.Position.Y);
                barrelRotation = ((1.05f * pi - pi) * -1);
            }

            //Bullet bullet = new Bullet(barrelRotation, barrelLocation, this.faceDirection);

            Bullet bullet = new TankBullet(barrelRotation, barrelLocation, this.faceDirection);

            bullet.Texture = bulletTexture;
            Bullets.Add(bullet);

        }

        /// <summary>
        /// Switches the player's isShooting value to false if true/ to true if true. 
        /// When isShooting is true. The player will shoot a burst of bullets equal to the burstAmmount
        /// </summary>
        /// <remarks>
        public void setIsShooting()
        {
            if (isShooting)
            {
                isShooting = false;
            }
            else
            {
                isShooting = true;
            }
        }

        public void checkToShoot(Vector2 targetPosition)
        {
            if (!isShooting && targetPosition.X < this.Position.X && faceDirection == FaceDirection.Left)
                isShooting = true;
            else if (!isShooting && targetPosition.X > this.Position.X && faceDirection == FaceDirection.Right)
                isShooting = true;
        }

        public override void Update(Vector2 playersVelocity, Vector2 playersPosition, GameTime gameTime)
        {
            if (Exploding)
            {
                Rotation = 0f;
                base.Velocity = new Vector2(0f, 0f);
            }
            else
            {
                determineVelocity();
                // determine when tank shoots
                // determine when tank shoots
                checkToShoot(playersPosition);
                // determine if the tank should turnaround
                checkToTurn(playersPosition);            
                
               if (isShooting)
               {
                   if ((gameTime.TotalGameTime - bulletCreationTime).TotalMilliseconds > burstDelay)
                   {
                       if (burstCount != 0)
                       {
                           Shoot();
                           burstCount -= 1;
                           if (burstCount > 0)
                               bulletCreationTime = gameTime.TotalGameTime;
                           else
                           {
                               bulletCreationTime = gameTime.TotalGameTime;
                               burstDelay = 500;
                           }
                       }
                       else
                       {
                           isShooting = false;
                           burstDelay = 300;
                           burstCount = burstAmmount;

                       }
                   }
               }
            }

            //barrelLocation += (base.Velocity - playersVelocity);
            foreach (Bullet bullet in Bullets.ToArray())
            {
                
                if (bullet.HasExceededDistance())
                {
                    Bullets.Remove(bullet);
                }
                else
                {
                    bullet.Update(playersVelocity);
                }
            }

            base.Update(playersVelocity, playersPosition, gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            foreach (Bullet bullet in Bullets.ToArray())
                bullet.Draw(spriteBatch);
        }

    }
}
