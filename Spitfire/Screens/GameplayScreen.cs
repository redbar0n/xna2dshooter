#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Spitfire;
using System.Collections; //For Array List (may remove later)
#endregion

namespace Spitfire
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Level level;
        Player player;
        HUD hud;

        public HUD Hud
        {
            get
            { return hud; }
        }


        public static bool muted = false;

        /// <summary>
        /// Storage for bomb explosions
        /// </summary>
        ArrayList explosions;

        #endregion

        #region CONSTANTS
        public const int PLAYER_CRASH_INTO_GROUND_DAMAGE = 10;
        public const int PLAYER_CRASH_INTO_ENEMY_DAMAGE = 5;
        public const int PLAYER_EXTRA_HEALTH_ON_CRATE_PICKUP = 20;
        public const int PLAYER_EXTRA_BOMBS_ON_CRATE_PICKUP = 5;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

        public void muteSounds()
        {
            muted = true;

            MediaPlayer.Stop();
            //NickSound
            player.engineSoundInst.Stop();
            
            foreach (Enemy enemy in level.Enemies)
            {
                // NickSound
                enemy.engineSoundInst.Stop();
            }
        }

        public void unMuteSounds()
        {
            muted = false;
            MediaPlayer.Resume();
            player.engineSoundInst.Resume();
            foreach (Enemy enemy in level.Enemies)
            {
                enemy.engineSoundInst.Resume();
            }
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");
            if (level == null)
            {
                level = new Level(this, content);
            }
            else
            {
                level.startOfLevelScore = hud.Score;
            }
            player = new Player(ScreenManager, content, level);
            if (hud == null)
            {
                Console.WriteLine("new hud");
                hud = new HUD(player);
            }
            else
            {
                Console.WriteLine("hud remains");
                player.CurrentHP = 100;
                player.BombCount = player.PLAYER_BOMB_START;
            }
            player.setHud(hud);
            explosions = new ArrayList();

            //level.LoadContent(content, "Sprites/Backgrounds/Mountain/mountain_final_", 4);
            level.LoadContent();
            player.LoadContent(content);
            hud.LoadContent(content);

            player.setAnimation(player.NormalAni);
            
            // load and add all animations?

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            MediaPlayer.Stop();
            content.Unload();
        }


        #endregion

        public void unPauseSounds()
        {
            // unmute all sounds
            player.engineSoundInst.Resume();
            foreach (Enemy enemy in level.Enemies)
            {
                enemy.engineSoundInst.Resume();
            }
        }

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
               
                player.Update(gameTime);
                level.Velocity = player.Velocity;
                level.playersPosition = player.Position;
                level.Update(gameTime); // includes updating enemies
                player.DistanceFromGround = (level.grounds[0].Position.Y - player.Position.Y);

                foreach (Explosion kaboom in explosions.ToArray())
                {
                    if (kaboom.returnActive() == false)
                    {
                        explosions.Remove(kaboom);

                    }
                    else
                        kaboom.update(gameTime, player.Velocity);

                }


                foreach (Enemy enemy in level.Enemies.ToArray())
                {
                    // ENEMY/BULLET Collision detection
                    foreach (Bullet bullet in player.Bullets.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, bullet))
                        {
                            if (!enemy.Exploding)
                            {
                                player.Bullets.Remove(bullet);
                                enemy.TakeDamage(player.BulletDamage);
                                enemy.playHitSound();
                                if (enemy.Exploding)
                                {
                                    hud.Score += enemy.WorthScore;
                                }
                            }
                        }
                    }

                    // ENEMY/BOMB Collision detection
                    foreach (Bomb bomb in player.bombs.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, bomb))
                        {
                            //NickSound
                            bomb.BombSoundInst.Stop();
                            if (!GameplayScreen.muted)
                                Bomb.explosionSound.Play();
                            Explosion explosion = new Explosion(bomb.Position, gameTime);
                            explosion.Texture = content.Load<Texture2D>("Sprites/Enemies/expllarge_final");
                            explosions.Add(explosion);
                            player.bombs.Remove(bomb);
                        }
                    }

                    // ENEMY/BOMB EXPLOSION Collision detection
                    foreach (Explosion kaboom in explosions.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, kaboom) && !enemy.Exploding)
                        {
                            enemy.TakeDamage(kaboom.getDamage());
                            if (enemy.Exploding)
                                hud.Score += enemy.WorthScore;
                        }
                    }

                    // ENEMY/PLAYER Collision detection
                    if (!player.IsImmortal && player.isAlive && CollisionDetection.Collision(enemy, player) && !enemy.Exploding)
                    {
                        if (enemy is SuperWeapon || enemy is ZeppelinBoss)
                        {
                            // BOSS/PLAYER Collision detection
                            player.TakeDamage(PLAYER_CRASH_INTO_ENEMY_DAMAGE);
                            player.Die(true);
                        }
                        else
                        {
                            enemy.Explode();
                            player.TakeDamage(PLAYER_CRASH_INTO_ENEMY_DAMAGE);
                        }
                    }

                    //ENEMY BULLET/PLAYER Collision detection 
                    foreach (Bullet bullet in enemy.Bullets.ToArray()) {
                        if (!player.IsImmortal && player.isAlive && (CollisionDetection.Collision(bullet, player)))
                        {
                            player.TakeDamage(enemy.BulletDamage);
                            enemy.Bullets.Remove(bullet);
                        } 

                    }

                    //ENEMY BULLET/GROUND Collision detection 
                    foreach (Bullet bullet in enemy.Bullets.ToArray()) {
                        foreach (Sprite ground in level.grounds) {
                            if (player.isAlive && CollisionDetection.Collision(ground, bullet))
                            {
                                Explosion spatter = new Explosion(new Vector2(bullet.Position.X, bullet.Position.Y), gameTime);
                                spatter.Texture = content.Load<Texture2D>("Sprites/Player/spatterground");                                
                                enemy.Bullets.Remove(bullet);
                            }
                        }                    
                    }

                    //ENEMY BOMB/PLAYER Collision detection 
                    foreach (EnergyBomb bomb in enemy.bombs.ToArray())
                    {
                        if (!player.IsImmortal && player.isAlive && CollisionDetection.Collision(player, bomb))
                        {
                            player.TakeDamage(7);// 7 damage from a bomb
                            Explosion explosion = new Explosion(bomb.Position, gameTime);
                            explosion.Texture = content.Load<Texture2D>("Sprites/Enemies/expllarge_final");
                            explosions.Add(explosion);
                            enemy.bombs.Remove(bomb);

                        }
                    }
                    
                    

                }
                // GROUND/PLAYER collision detection
                foreach (Sprite ground in level.grounds)
	            {
                    if (player.isAlive && CollisionDetection.Collision(ground, player))
                    {
                        player.TakeDamage(PLAYER_CRASH_INTO_GROUND_DAMAGE);
                        player.Die(true);
                    }

	            }


                // BOMB/GROUND collision detection
                foreach (Bomb bomb in player.bombs.ToArray())
                {
                    foreach (Sprite ground in level.grounds)
                    {
                        if (CollisionDetection.Collision(ground, bomb))
                        {
                            //NickSound
                            bomb.BombSoundInst.Stop();
                            if (!GameplayScreen.muted)
                                Bomb.explosionSound.Play();
                            Explosion explosion = new Explosion(new Vector2(bomb.Position.X, bomb.Position.Y - 50f), gameTime);
                            explosion.Texture = content.Load<Texture2D>("Sprites/Enemies/expllarge_final");
                            explosions.Add(explosion);
                            player.bombs.Remove(bomb);
                        }
                    }
                }

                // GROUND/BULLET collision detection
                foreach (Bullet bullet in player.Bullets.ToArray())
                {
                    foreach (Sprite ground in level.grounds)
                    {
                        if (CollisionDetection.Collision(ground, bullet))
                        {
                            Explosion spatter = new Explosion(new Vector2(bullet.Position.X, bullet.Position.Y), gameTime);
                            spatter.Texture = content.Load<Texture2D>("Sprites/Player/spatterground");
                            explosions.Add(spatter);
                            player.Bullets.Remove(bullet);
                        }
                    }
                }
                // BUILDINGS / PLAYER
                foreach (Building building in level.buildings.ToArray()) {
                    if (!player.IsImmortal && player.isAlive && CollisionDetection.Collision(building, player))
                    {
                        player.TakeDamage(PLAYER_CRASH_INTO_GROUND_DAMAGE);
                        player.Die(true);
                    }
                }


                // CRATES/PLAYER
                foreach (Pickup pickup in level.Pickups.ToArray())
                {
                    if (player.isAlive && CollisionDetection.Collision(pickup, player))
                    {
                        player.BombCount += PLAYER_EXTRA_BOMBS_ON_CRATE_PICKUP;
                        player.Recover(PLAYER_EXTRA_HEALTH_ON_CRATE_PICKUP);
                        level.Pickups.Remove(pickup);
                        Pickup.PickupSound.Play();
                    }

                    // CRATES/GROUND
                    foreach (Sprite ground in level.grounds)
                    {
                        if (CollisionDetection.Collision(ground, pickup))
                        {
                            level.pickups.Remove(pickup);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame(ControllingPlayer))
            {
                
                // mute all enginesounds, but not music
                player.engineSoundInst.Pause();
                foreach (Enemy enemy in level.Enemies)
                {
                    enemy.engineSoundInst.Pause();
                }
                ScreenManager.AddScreen(new PauseMenuScreen(level, this), ControllingPlayer);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            
            level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            foreach (Explosion explosion in explosions)
                explosion.Draw(spriteBatch);

            hud.Draw(spriteBatch, ScreenManager.Game.Window.ClientBounds.Width);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
