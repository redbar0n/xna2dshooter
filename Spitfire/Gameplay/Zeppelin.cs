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
    class Zeppelin : Enemy
    {

        private Vector2 initialVelocity = new Vector2(2f, 0f);
        private Vector2 cannonLocation;
        private float pi = (float)Math.PI;

        private int burstAmmount = 10; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Value set in constructor
        private int burstDelay = 50; // The Delay in miliseconds between each shot        
        TimeSpan bulletCreationTime;

        private bool isShooting = false;

        public Zeppelin(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping) { 
        
        
            //TODO Fill in details about statistics
            // -- HP and worth score look a little high. Might want to decrease them eventually

                //bulletTexture = bulletImage;
                if (difficulty == Difficulty.Easy)
                {
                    StartHP = 1000;
                    WorthScore = 5000;
                    BulletDamage = 5;
                }
                else if (difficulty == Difficulty.Medium)
                {
                    StartHP = 2000;
                    WorthScore = 10000;
                    BulletDamage = 10;
                }
                else
                {
                    StartHP = 5000;
                    WorthScore = 25000;
                    BulletDamage = 15;
                }
        
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
                cannonLocation = new Vector2(base.Position.X + 5f, base.Position.Y + 40f);

            }
            else
            {
                cannonLocation = new Vector2(base.Position.X + 20f, base.Position.Y + 40f);
            }

            //Bullet bullet = new Bullet(barrelRotation, barrelLocation, this.faceDirection);

            Bullet bullet = new Bullet(this.Rotation + pi, cannonLocation, this.faceDirection);

            bullet.Texture = bulletTexture;
            Bullets.Add(bullet);

        }

        public void checkToShoot(Vector2 targetPosition)
        {
            /// Make enemy shoot
            if (targetPosition.Y > this.Position.Y - 100f && targetPosition.Y < this.Position.Y + 100f)
            {
                if (!isShooting && targetPosition.X < this.Position.X && faceDirection == FaceDirection.Left)
                    isShooting = true;
                else if (!isShooting && targetPosition.X > this.Position.X && faceDirection == FaceDirection.Right)
                    isShooting = true;
            }

        }

        public override void Update(Vector2 playersVelocity, Vector2 playersPosition, GameTime gameTime)
        {
            if (Exploding && getAnimationFinish())
            {
                HasExploded = true;
            }

            if (Exploding)
            {
                Rotation = 0f;
                base.Velocity = new Vector2(0f, 0f);
            }
            else
            {

                determineVelocity();

                // Check if the plane should shoot or not
                checkToShoot(playersPosition);
                // Turn Enemy around if required.
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
                            burstDelay = 50;
                            burstCount = burstAmmount;

                        }
                    }
                }



            }
            base.Position += (base.Velocity - playersVelocity);
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

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            foreach (Bullet bullet in Bullets.ToArray())
                bullet.Draw(spriteBatch);
        }



        //TODO add other overloading methods including update, shoot? etc



    }
}
