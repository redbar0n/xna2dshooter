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
    class ZeppelinBoss: Enemy
    {


        private Vector2 initialVelocity = new Vector2(2f, 0f);
        private Vector2 cannonLocation;

        private float pi = (float)Math.PI;

        private int burstAmmount = 10; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Value set in constructor
        private int burstDelay = 50; // The Delay in miliseconds between each shot        
        TimeSpan bulletCreationTime;

        private bool isShooting = false;

        private bool isBombing;
        private int bombBurstAmmount = 15;
        private int bombCount = 15;
        private int bombDelay = 250; // The Delay in miliseconds between each shot        
        TimeSpan bombCreationTime;

            public ZeppelinBoss(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping) {


                
            //TODO Fill in details about statistics
            // -- HP and worth score look a little high. Might want to decrease them eventually

                //bulletTexture = bulletImage;
                if (difficulty == Difficulty.Easy)
                {
                    StartHP = 2000;
                    WorthScore = 25000;
                    BulletDamage = 10;
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


      public void determineVelocity(Vector2 playersPosition) {

          float cannonLocationY = this.Position.Y + 100f;

          float yVelocity = 0;
          if (playersPosition.Y < (cannonLocationY -20)) {
              yVelocity = -0.5f;
          }
          else if (playersPosition.Y > (cannonLocationY + 20))
          {
              yVelocity = 0.5f;
          }


          if (faceDirection == FaceDirection.Left)
          {
              if (this.Position.X > playersPosition.X)
              {
                  base.Velocity = new Vector2(initialVelocity.X * -1, yVelocity);

              }
              else
                  base.Velocity = new Vector2(initialVelocity.X * -2, yVelocity);
          }
          else { // Face Direction is Right
              if ((this.Position.X + this.Texture.Height) < playersPosition.X)
              {
                  base.Velocity = new Vector2(initialVelocity.X,yVelocity);

              }
              else {
                  base.Velocity = new Vector2(initialVelocity.X * 2, yVelocity);
              }                  
          
          
          
          }        
          
          
          
              
      }


      public bool checkToDropBomb(Vector2 playersPosition)
      {
          if (faceDirection == FaceDirection.Left && this.Position.X < playersPosition.X)
              return true;
          else if (faceDirection == FaceDirection.Right && (this.Position.X + this.Texture.Height) > playersPosition.X)
              return true;

          return false;
      }

      public void Shoot()
      {
          if (faceDirection == FaceDirection.Left)
          {
              cannonLocation = new Vector2(base.Position.X, base.Position.Y + 100f);

          }
          else
          {
              cannonLocation = new Vector2(base.Position.X + Texture.Height , base.Position.Y + 100f);    
          }



          Bullet bullet = new Bullet(this.Rotation + pi, cannonLocation, this.faceDirection);

          bullet.Texture = bulletTexture;
          Bullets.Add(bullet);

      }


      public void DropBomb(Vector2 targetPosition)
      {
          if (faceDirection == FaceDirection.Left)
          {
              cannonLocation = new Vector2(base.Position.X + 5f, base.Position.Y + 100f);

          }
          else
          {
              cannonLocation = new Vector2(base.Position.X + 20f, base.Position.Y + 100f);
          }

          EnergyBomb bomb = new EnergyBomb(Rotation, cannonLocation, targetPosition, faceDirection);
          bomb.Texture = bombTexture;
          bombs.Add(bomb);
          if (!GameplayScreen.muted)
              Enemy.BombSound.Play();
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

          if (!GameplayScreen.muted)
          {
              // NickSound
              engineSoundInst.Volume = 100 / Math.Abs(playersPosition.X - Position.X);
              if (engineSoundInst.Volume < 0.05f)
                  engineSoundInst.Volume = 0;
              else if (engineSoundInst.Volume > 20f)
                  engineSoundInst.Volume = 20.0f;
          }

          if (Exploding)
          {
              Rotation = 0f;
              base.Velocity = new Vector2(0f, 0f);
          }
          else
          {

              determineVelocity(playersPosition);

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
                          DropBomb(playersPosition);
                          burstCount = burstAmmount;

                      }
                  }
              }
              else if (checkToDropBomb(playersPosition)) {
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
                          bombDelay = 1500;
                          bombCount = bombBurstAmmount;       
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

          foreach (EnergyBomb bomb in bombs.ToArray()) {
              if (bomb.HasExceededDistance())
              {
                  bombs.Remove(bomb);
              }
              else {
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
