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
    class SuperWeapon:Enemy
    {

        bool hasGotInitialTime = false;




        // Increase and decreases to make the superweapon hover.
        private double hoverValue; //- used to calculate verticalAccelerant
        private float verticalAccelerant = 0;

        private int hoverTime = 5000; // The ammount of time in miliseconds the enemy hovers  
        TimeSpan hoverStartTime;

        bool isHovering = true;

        bool isMoving = false;// Determines if the Superweapon moves across the screen
        TimeSpan moveStartTime;
        int moveTime = 5000;

        private float pi = (float)Math.PI;


        private Vector2 cannonLocation;

        private int burstAmmount = 10; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Value set in constructor
        private int burstDelay = 50; // The Delay in miliseconds between each shot        
        TimeSpan bulletCreationTime;

        private bool isShooting = false;


        private Vector2 initialVelocity = new Vector2(15f, 4f);


        private bool isBombing;
        private int bombBurstAmmount = 15;
        private int bombCount = 15;
        private int bombDelay = 250; // The Delay in miliseconds between each shot        
        TimeSpan bombCreationTime;
        
        
        
        public SuperWeapon(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping) {

                faceDirection = FaceDirection.Left;
            if (difficulty == Difficulty.Easy)
                {
                    StartHP = 1000;
                    WorthScore = 25000;
                    BulletDamage = 5;
                }
                else if (difficulty == Difficulty.Medium)
                {
                    StartHP = 2000;
                    WorthScore = 50000;
                    BulletDamage = 10;
                }
                else
                {
                    StartHP = 4000;
                    WorthScore = 80000;
                    BulletDamage = 15;
                }
        }


        private void hover() {
            verticalAccelerant = (float)Math.Sin(hoverValue += 0.025);     
        }

        public void determineVelocity()
        {
            if (!isHovering) {
                verticalAccelerant = 0;
            }

            float horizontalSpeed = 0;
            if (isMoving) {
                horizontalSpeed = initialVelocity.X;
            }
            
            if (faceDirection == FaceDirection.Left)
            {
                base.Velocity = new Vector2(horizontalSpeed * (float)faceDirection, initialVelocity.Y * verticalAccelerant);//initialVelocity.X * -1
            }
            else
                base.Velocity = new Vector2(horizontalSpeed * (float)faceDirection, initialVelocity.Y * verticalAccelerant);//initialVelocity.X

        }
        
        
        
        public void Shoot()
        {
            if (faceDirection == FaceDirection.Left)
            {
                cannonLocation = new Vector2(base.Position.X - 300f, base.Position.Y + 18f);

            }
            else
            {
                cannonLocation = new Vector2(base.Position.X + 300f , base.Position.Y);
            }



            Bullet bullet = new Bullet(this.Rotation + pi, cannonLocation, this.faceDirection);

            bullet.Texture = bulletTexture;
            Bullets.Add(bullet);

        }

        public void DropBomb(Vector2 targetPosition)
        {
            if (faceDirection == FaceDirection.Left)
            {
                cannonLocation = new Vector2(base.Position.X - 200f, base.Position.Y + 18f);

            }
            else
            {
                cannonLocation = new Vector2(base.Position.X + 200f, base.Position.Y + 12f);
            }

            EnergyBomb bomb = new EnergyBomb(Rotation, cannonLocation, targetPosition, faceDirection);
            bomb.Texture = bombTexture;
            bombs.Add(bomb);


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
            
            ///Gets initial time to start the loop
            if (!hasGotInitialTime){
                hoverStartTime = gameTime.TotalGameTime;
                isHovering = true;
                hasGotInitialTime = true;
            }
            
            
            
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

                // Check if the plane should shoot or not
                checkToShoot(playersPosition);
                // Turn Enemy around if required.
                checkToTurn(playersPosition);
                
                
                
                if (isHovering) {
                    if ((gameTime.TotalGameTime - hoverStartTime).TotalMilliseconds < hoverTime)
                    {
                        hover();
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
                    else {
                        isHovering = false;
                        isBombing = true;
                    }


                }
                else if (isBombing == true)
                {
                    if ((gameTime.TotalGameTime - bombCreationTime).TotalMilliseconds > bombDelay)
                    {
                        if (bombCount != 0)
                        {
                            DropBomb(playersPosition);
                            bombCount -= 1;
                            if (bombCount > 0)
                                bombCreationTime = gameTime.TotalGameTime;
                            else
                            {
                                bombCreationTime = gameTime.TotalGameTime;
                                bombDelay = 500;
                            }
                        }
                        else
                        {
                            isBombing = false;
                            bombDelay = 250;
                            bombCount = bombBurstAmmount;
                            //isHovering = true;
                            //hoverStartTime = gameTime.TotalGameTime;
                            moveStartTime = gameTime.TotalGameTime;
                            isMoving = true;
                        }
                    }



                }
                else { 
                    if ((gameTime.TotalGameTime - moveStartTime).TotalMilliseconds > moveTime){
                        isHovering = true;
                        hoverStartTime = gameTime.TotalGameTime;
                        isMoving = false;
                    }
                
                }

                
                
                
                determineVelocity();








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

            foreach (EnergyBomb bomb in bombs.ToArray())
            {
                if (bomb.HasExceededDistance())
                {
                    bombs.Remove(bomb);
                }
                else
                {
                    bomb.Update(playersVelocity, playersPosition);
                }

            }


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            foreach (Bullet bullet in Bullets.ToArray())
                bullet.Draw(spriteBatch);

            foreach (Bomb bomb in bombs.ToArray())
                bomb.Draw(spriteBatch);
        }


    }
}
