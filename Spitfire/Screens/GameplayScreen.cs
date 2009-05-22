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

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        Level level;
        Player player;
        HUD hud;
        Vector2 playersVelocity;
        ArrayList explosions;   // STORAGE FOR BOMB EXPLOSIONS

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


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            level = new Level();
            player = new Player(ScreenManager, content);
            hud = new HUD(player);
            explosions = new ArrayList();

            level.LoadContent(content, "Sprites/Backgrounds/mountainFlat", 2);
            player.NormalAni = new Animation(content.Load<Texture2D>("Sprites/Player/Spitfireresized"), 1, true);

            player.bulletTexture = content.Load<Texture2D>("Sprites/Player/heroammosize");
            player.bombTexture = content.Load<Texture2D>("Sprites/Player/herobomb");
            hud.LoadContent(content);

            player.setAnimation(player.NormalAni);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.4f; // magic constant
            MediaPlayer.Play(content.Load<Song>("Sounds/213663_SOUNDDOGS__ba"));
            
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
            content.Unload();
        }


        #endregion

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
                playersVelocity = player.Velocity;
                level.Velocity = player.Velocity;
                level.playersPosition = player.Position;
                level.Update(gameTime); // includes updating enemies
                player.DistanceFromGround = (level.levelGround.Position.Y - player.Position.Y);
                //Console.WriteLine(level.levelGround.Position.Y - player.Position.Y); 

                foreach (Explosion kaboom in explosions.ToArray())
                {
                    if (kaboom.returnActive() == false)
                    {
                        explosions.Remove(kaboom);

                    }
                    else
                        kaboom.update(gameTime, playersVelocity);

                }

                // Collision detection
                foreach (Enemy enemy in level.Enemies.ToArray())
                {
                    foreach (Bullet bullet in player.Bullets.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, bullet))
                        {
                            if (!enemy.Exploding)
                            {
                                player.Bullets.Remove(bullet);
                                enemy.TakeDamage(player.BulletDamage);
                                if (enemy.Exploding)
                                    hud.Score += enemy.WorthScore;
                            }
                        }
                    }

                    foreach (Bomb bomb in player.bombs.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, bomb))
                        {
                            //NickSound
                            //bomb.BombSoundInst.Stop(true);
                            //bomb.PlayExplosionSound();
                            Explosion explosion = new Explosion(bomb.Position, gameTime);
                            explosion.Texture = content.Load<Texture2D>("Sprites/Enemies/zeppelin2sizedExplode");
                            explosions.Add(explosion);
                            player.bombs.Remove(bomb);
                        }

                    }

                    foreach (Explosion kaboom in explosions.ToArray())
                    {
                        if (CollisionDetection.Collision(enemy, kaboom) && !enemy.Exploding)
                        {
                            enemy.TakeDamage(kaboom.getDamage());
                            if (enemy.Exploding)
                                hud.Score += enemy.WorthScore;
                        }


                    }

                    if (CollisionDetection.Collision(enemy, player) && !enemy.Exploding)
                    {
                        enemy.Explode();
                        player.TakeDamage(20);
                    }


                }
                if ((CollisionDetection.Collision(level.levelGround, player)) ||
                    (CollisionDetection.Collision(level.levelGroundTwo, player)))
                {
                    player.TakeDamage(100);
                    player.Die();
                }

                foreach (Bomb bomb in player.bombs.ToArray())
                {
                    if ((CollisionDetection.Collision(level.levelGround, bomb)) ||
                        (CollisionDetection.Collision(level.levelGroundTwo, bomb)))
                    {
                        //NickSound
                       // bomb.BombSoundInst.Stop(true);
                        //bomb.PlayExplosionSound();
                        Explosion explosion = new Explosion(new Vector2(bomb.Position.X, bomb.Position.Y - 50f), gameTime);
                        explosion.Texture = content.Load<Texture2D>("Sprites/Enemies/zeppelin2sizedExplode");
                        explosions.Add(explosion);
                        player.bombs.Remove(bomb);
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

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
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
