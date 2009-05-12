using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections; //For Array List (may remove later)

namespace Spitfire
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;
        Player player;
        HUD hud;
        Vector2 playersVelocity;
        private bool pause = false;
        Boolean enterKeyWasPressed = false;
        ArrayList explosions;   // STORAGE FOR BOMB EXPLOSIONS

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            level = new Level();
            player = new Player(graphics, this.Content);
            hud = new HUD(player);
            explosions = new ArrayList();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            level.LoadContent(this.Content, "Sprites/Backgrounds/mountainFlat", 2);
            player.NormalAni = new Animation(this.Content.Load<Texture2D>("Sprites/Player/Spitfireresized"), 1, true);
            player.bulletTexture = Content.Load<Texture2D>("Sprites/Player/heroammosize");
            player.bombTexture = Content.Load<Texture2D>("Sprites/Player/herobomb");
            hud.LoadContent(this.Content);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.4f; // magic constant
            MediaPlayer.Play(Content.Load<Song>("Sounds/213663_SOUNDDOGS__ba"));
            // load and add all animations?
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter) && !enterKeyWasPressed)
            {
                Pause();
                enterKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.Enter))
            {
                enterKeyWasPressed = false;
            }


            if (!pause)
            {

                player.Update();
                playersVelocity = player.Velocity;
                level.Velocity = player.Velocity;
                level.Update(gameTime); // includes updating enemies

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
                            bomb.BombSoundInst.Stop(true);
                            bomb.PlayExplosionSound();
                            Explosion explosion = new Explosion(bomb.Position, gameTime);
                            explosion.Texture = Content.Load<Texture2D>("Sprites/Enemies/zeppelin2sizedExplode");
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
                        bomb.BombSoundInst.Stop(true);
                        bomb.PlayExplosionSound();
                        Explosion explosion = new Explosion(new Vector2(bomb.Position.X, bomb.Position.Y - 50f), gameTime);
                        explosion.Texture = Content.Load<Texture2D>("Sprites/Enemies/zeppelin2sizedExplode");
                        explosions.Add(explosion);
                        player.bombs.Remove(bomb);
                    }

                }

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            foreach (Explosion explosion in explosions)
                explosion.Draw(spriteBatch);

            hud.Draw(spriteBatch, this.Window.ClientBounds.Width);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void Pause()
        {
            if (pause)
                pause = false;
            else
                pause = true;
        }

        public void HandleInput()
        {
            throw new System.NotImplementedException();
        }
    }
}
