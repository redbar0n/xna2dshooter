using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    /// <summary>
    /// Handles background, ground, enemies, scoring, win/lose cond., bonuses etc.
    /// </summary>
    /// <remarks>
    /// future: Level class to hold Background and Ground class
    /// future: upgrade background class to how it was done in project AdvancedScrollingA2DBackground
    /// </remarks>
    public class Level
    {

        //private int music;
        //private int sounds



        public ContentManager Content
        {
            get { return content; }
        }
        private ContentManager content;

        private Sprite sky;
        private Sprite skyTwo;
        private List<Sprite> backgrounds;
        private float backgroundBufferWidth = 0;

        /// <summary>
        /// Indicates position in the level. Updated when frames loop.
        /// </summary>
        /// <remarks>
        /// A simple frame counter that ticks as frames loop.
        /// </remarks>
        private float positionInLevel = 0;

        /// <summary>
        /// Indicates furthest progress in the level. Updated when frames loop.
        /// </summary>
        private float levelProgress = 0;


        /// <summary>
        /// The base velocity of everything in the level. Objects can have own velocity in addition.
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;

        /// <summary>
        /// Will make sure enemies are added only once after each LevelProgress change.
        /// </summary>
        bool addEnemies = false;

        /// <summary>
        /// Holds all enemies in level
        /// </summary>
        /// <remarks>
        /// future: update so that it only holds enemies in part of level surrounding player
        /// </remarks>
        public List<Enemy> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }
        List<Enemy> enemies;

        /// <summary>
        /// How much background should be scaled. Redundant if background texture images are scaled.
        /// </summary>
        private float scale = 1.25f; // NB: may cause error since scale is in sprite as well

        public Level()
        {
            backgrounds = new List<Sprite>();
            enemies = new List<Enemy>();
            sky = new Sprite();
            skyTwo = new Sprite();
        }

        // future: Optimize background loading to remove slight lag. Load smaller backgrounds.
        public void LoadContent(ContentManager content, String backgroundName, int nrOfBackgrounds)
        {
            this.content = content;
            
            // Load sky
            sky.Texture = this.content.Load<Texture2D>("Sprites/backgrounds/skyfinal");
            sky.Position = new Vector2(0, 0);
            skyTwo.Texture = sky.Texture;
            skyTwo.Position = new Vector2(sky.Position.X + sky.Size.Width, 0);

            // Load backgrounds
            for (int i = 0; i < nrOfBackgrounds; i++)
            {
                Sprite frame = new Sprite();
                //background.Texture = this.content.Load<Texture2D>(backgroundName + nrOfBackgrounds);
                frame.Texture = this.content.Load<Texture2D>(backgroundName + "1"); // should be set to i if more than one background
                frame.Scale = scale;
                frame.Position = new Vector2(backgroundBufferWidth, 0); // buffer width indicates xpos to insert background
                frame.Velocity = velocity;
                backgrounds.Add(frame);
                backgroundBufferWidth += frame.Size.Width; // increase width of buffer
            }

            // TODO: Load music and sounds
        }

        private void UpdateSky()
        {
            if (sky.Position.X < -sky.Size.Width)
            {
                sky.Position = new Vector2(skyTwo.Position.X + skyTwo.Size.Width, 0);
            }
            else if (skyTwo.Position.X < -skyTwo.Size.Width)
            {
                skyTwo.Position = new Vector2(sky.Position.X + sky.Size.Width, 0);
            }
            else if (sky.Position.X > sky.Size.Width)
            {
                // can use sky.Size.Width in the check because only two sprites. otherwise would have needed a bufferwidth like with backgrounds
                sky.Position = new Vector2(-sky.Size.Width + 4, 0); // 4 = magic constant to avoid mysterious gap between frames
            }
            else if (skyTwo.Position.X > skyTwo.Size.Width)
            {
                skyTwo.Position = new Vector2(-skyTwo.Size.Width + 4, 0);
            }


            // sky frames only scroll horizontally
            sky.Position += new Vector2(-1 * velocity.X / 2, 0);
            skyTwo.Position += new Vector2(-1 * velocity.X / 2, 0);
        }

        /// <summary>
        /// Also updates level progress
        /// </summary>
        private void UpdateBackground()
        {
            if (backgrounds.Count == 0)
                throw new NotSupportedException("No background loaded.");

            // Update position of background frames
            Object[] bg = backgrounds.ToArray(); // background arraylist as an array
            Sprite firstBackground = (Sprite)bg[0];
            Sprite lastBackground = (Sprite)bg[bg.Length - 1];
            for (int i = 0; i < bg.Length; i++)
            {
                Sprite frame = (Sprite)backgrounds[i];

                if (frame.Position.X < -frame.Size.Width)
                {
                    // only because backgrounds stored in array. relates them to each other in a loop.
                    if (i == 0)
                    {
                        // only called when background nr 0 goes completely outside left side of screen
                        frame.Position = new Vector2(lastBackground.Position.X + lastBackground.Size.Width, lastBackground.Position.Y);
                    }
                    else
                    {
                        Sprite prevFrame = (Sprite)backgrounds[i - 1];
                        frame.Position = new Vector2(prevFrame.Position.X + prevFrame.Size.Width,
                                                                prevFrame.Position.Y);
                    }


                    positionInLevel++;
                    if (positionInLevel > levelProgress)
                    {
                        levelProgress++;
                        addEnemies = true;
                        Console.WriteLine("levelProgress" + levelProgress);
                    }

                    Console.WriteLine(positionInLevel);
                }
                else if (frame.Position.X > (backgroundBufferWidth - frame.Size.Width))
                {
                    // only because backgrounds stored in array. relates them to each other in a loop.
                    if (i == (bg.Length - 1))
                    {
                        // only called when the last background goes completely outside right side of screen
                        frame.Position = new Vector2(firstBackground.Position.X - frame.Size.Width, firstBackground.Position.Y);
                    }
                    else
                    {
                        // since i != (bg.Length - 1) we are sure that the array has more backgrounds
                        Sprite nextFrame = (Sprite)backgrounds[i + 1];
                        frame.Position = new Vector2(nextFrame.Position.X - nextFrame.Size.Width, nextFrame.Position.Y);
                    }


                    positionInLevel--;
                    addEnemies = false;
                    Console.WriteLine(positionInLevel);
                }

                // ElapsedGameTime causes background to go very slowly up or down. Is it necessary?
                frame.Position += -1 * velocity; //* (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Based on level progress
        /// </summary>
        private void LoadNewEnemies()
        {
            // Load enemies depending on current background frame
            // future: important: specify loading level layout (and type of enemies based on level) from txt file
            // future: dynamically load/unload/save enemies depending on current background frame.
            if (positionInLevel == 1 && addEnemies)
            {
                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized");
                zeppelin.Position = new Vector2(1400, 300);
                zeppelin.Velocity = new Vector2(2, 0);
                zeppelin.WorthScore = 100;
                zeppelin.StartHP = 10;
                enemies.Add(zeppelin);
                addEnemies = false;
            }
            else if (positionInLevel == 2 && addEnemies)
            {
                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized");
                zeppelin.Position = new Vector2(1410, 300);
                zeppelin.Velocity = new Vector2(-2, -1);
                zeppelin.StartHP = 20;
                zeppelin.WorthScore = 150;
                enemies.Add(zeppelin);


                Enemy zeppelin2 = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized");
                zeppelin2.Position = new Vector2(1410, 200);
                zeppelin2.Velocity = new Vector2(-1, 1);
                zeppelin2.StartHP = 40;
                zeppelin2.WorthScore = 200;
                enemies.Add(zeppelin2);

                addEnemies = false;
            }
        }

        /// <summary>
        /// Update enemy speed relative to player speed, and remove exploded enemies.
        /// </summary>
        private void UpdateEnemies()
        {
            foreach (Enemy enemy in enemies.ToArray())
            {
                if (enemy.HasExploded)
                {
                    enemies.Remove(enemy);
                }
                else
                {
                    enemy.Position += -1 * velocity;
                    enemy.Update();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateSky();

            UpdateBackground();

            LoadNewEnemies();

            UpdateEnemies();

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Draw the sky
            sky.Draw(spriteBatch);
            skyTwo.Draw(spriteBatch);

            // Draw the background
            foreach (Sprite frame in backgrounds)
            {
                frame.Draw(spriteBatch);
                //Console.WriteLine("3: " + frame.Position);
            }

            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }
        }
    }
}
