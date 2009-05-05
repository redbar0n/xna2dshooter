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
            // TODO: Add your initialization logic here
            level = new Level();
            player = new Player(graphics);
            hud = new HUD(player);

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

            // TODO: use this.Content to load your game content here
            level.LoadContent(this.Content, "Sprites/Backgrounds/mountainFlat", 2);
            player.NormalFlight = new Animation(this.Content.Load<Texture2D>("Sprites/Player/Spitfireresized"), 1, true);
            player.bulletTexture = Content.Load<Texture2D>("Sprites/Player/shots");
            player.bombTexture = Content.Load<Texture2D>("Sprites/Player/herobomb");
            hud.LoadContent(this.Content);

            // load and add all animations
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            player.Update();
            playersVelocity = player.Velocity;
            level.Velocity = player.Velocity;
            level.Update(gameTime); // includes updating enemies

            // Collision detection

            foreach (Bullet bullet in player.Bullets)
            {
                foreach (Enemy enemy in level.Enemies)
                {
                    if (CollisionDetection.Collision(bullet, enemy))
                    {
                        Console.WriteLine("exploding");
                        player.Bullets.Remove(bullet);
                        enemy.Explode(); // will pass the exploding down to animateExplosion, which will pass it up to level which finally removes enemy
                        hud.Score += enemy.WorthScore;
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

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            level.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            hud.Draw(spriteBatch, this.Window.ClientBounds.Width);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void HandleInput()
        {
            throw new System.NotImplementedException();
        }
    }
}
