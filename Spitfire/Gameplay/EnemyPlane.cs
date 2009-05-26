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
    
    class EnemyPlane: Enemy
    {

        private float pi = (float)Math.PI;
        private Vector2 initialVelocity = new Vector2(4f, 4f);    
        
        private float accelerant = 1.0f; // future: change to using dv/dt
        private float accelerationConstant = 0.125f; //0.25f;
        private static float MAXACCELERANT = 2.0f; //2.5f;
        private float decelerationConstant = 0.25f;

        private int burstAmmount = 5; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Value set in constructor
        private int burstDelay = 70; // The Delay in miliseconds between each shot        
        TimeSpan bulletCreationTime;


        private bool isShooting = false;

        


        public EnemyPlane(Level level,Difficulty difficulty, String spriteSet, bool looping)
            : base (level,difficulty, spriteSet,looping) 
        {
            //TODO maybe fill in stuff here
            this.faceDirection = FaceDirection.Left;
            
        }

        /// <summary>
        /// Makes the plane rotate anti clockwires. 
        /// </summary>
        /// <param name="ammount"></Multiply the default rotation distance>
        private void minusRotation(float ammount)
        {

            Rotation -= (float)1 / 120 * pi * ammount;

        }

        /// <summary>
        /// Makes the plane rotate clockwires.
        /// </summary>
        /// <param name="ammount"></Multiply the default rotation distance>
        private void plusRotation(float ammount)
        {

            Rotation += (float)1 / 120 * pi * ammount;

        }

        /// <summary>
        /// Adjusts the player's rotation when flying input controls are realeased
        /// </summary>
        private void AutoAdjustRotation()
        {
            if (faceDirection == FaceDirection.Left)
            {
                if (Math.Sin(Rotation) < 0f)
                {
                    plusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = 0;
                }
                else if (Math.Sin(Rotation) > 0f)
                {
                    minusRotation(1f);
                    if (Math.Sin(Rotation) < 0f)
                        Rotation = 0;
                }
            }
            else if (faceDirection == FaceDirection.Right)
                if (Math.Sin(Rotation) < 0f)
                {
                    minusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = pi;
                }
                else
                {
                    plusRotation(1f);
                    if (Math.Sin(Rotation) < 0f)
                        Rotation = pi;
                }
        }


        public void Accelerate(float yPercent)
        {
            if (accelerant < MAXACCELERANT)
                //if (Rotation < 1.0f)
                //    accelerant += accelerationBigConstant * yPercent;
                //else
                accelerant += accelerationConstant * yPercent;
            if (accelerant > MAXACCELERANT)
                accelerant = MAXACCELERANT;
        }

        public void Decelerate(float yPercent)
        {
            if (accelerant > 1.0f)
                accelerant -= (decelerationConstant * yPercent / 4);
            else if (accelerant < 1.0f)
                accelerant = 1.0f;

        }

        /// <summary>
        /// Determines the direction will fly based on the player's rotation
        /// </summary>
        private void determineFaceDirection()
        {

            if (Math.Cos(Rotation) > 0f)
                faceDirection = FaceDirection.Left;
            else if (Math.Cos(Rotation) < 0f)
                faceDirection = FaceDirection.Right;
        }

        /// <summary>
        /// Will determine velocity based on rotation. More velocity if flying down, less if up.
        /// </summary>
        private void determineVelocity()
        {
            float yPercent = (float)Math.Sin(Rotation);
            float xPercent = 1 - Math.Abs(yPercent);
            determineFaceDirection();

            if (Math.Sin(Rotation) > 0)
                Accelerate(Math.Abs(yPercent));

            if (Math.Sin(Rotation) < 0)
                Decelerate(Math.Abs(yPercent));

            // Wind resistance
            if (accelerant > 1f)
            {
                accelerant -= 0.0125f;
                if (accelerant < 1f)
                    accelerant = 1f;
            }



            Velocity = new Vector2(initialVelocity.X * xPercent * accelerant * (float)faceDirection,
                -initialVelocity.Y * yPercent * accelerant);
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


                if (playersPosition.Y < this.Position.Y - 50f && playersPosition.X < this.Position.X)
                {
                    if (Math.Sin(Rotation) < 0.8f)
                        plusRotation(1.5f);
                }
                else if (playersPosition.Y > this.Position.Y + 50f && playersPosition.X < this.Position.X)
                {
                    if (Math.Sin(Rotation) > -0.8f)
                        minusRotation(1.5f);
                }
                else
                {
                    AutoAdjustRotation();
                    
                }

                /// Make enemy shoot
                if (playersPosition.Y < this.Position.Y - 50f || playersPosition.Y > this.Position.Y + 50f) {
                    if (!isShooting && playersPosition.X < this.Position.X)
                        isShooting = true;
                }


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
            foreach (Bullet bullet in Bullets.ToArray())
            {
                //_shot1.Position.X > 1280 || _shot1.Position.X < 0
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

            foreach (Bomb bombX in bombs.ToArray())
                bombX.Draw(spriteBatch);
        }



       public void Shoot() {
            Bullet bullet = new Bullet(this.Rotation + pi , this.Position, this.faceDirection);
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



    }
}
