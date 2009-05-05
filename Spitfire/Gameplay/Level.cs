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
        // private int field; // QUESTION: what is this for?



        public ContentManager Content
        {
            get { return content; }
        }
        private ContentManager content;

        private List<Sprite> backgrounds;

        /// <summary>
        /// Indicates progress in the level. Updated when frames loop.
        /// </summary>
        /// <remarks>
        /// A simple frame counter that ticks as frames loop.
        /// </remarks>
        public float LevelProgress
        {
            get { return levelProgress; }
            set { levelProgress = value; }
        }
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

        // TODO: ENEMIES

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
        }

        // future: Optimize background loading to remove slight lag. Load smaller backgrounds.
        public void LoadContent(ContentManager content, String backgroundName, int nrOfBackgrounds)
        {
            this.content = content;

            // Load backgrounds
            float xPos = 0; // x-pos to insert background
            for (int i = 0; i < nrOfBackgrounds; i++)
            {
                Sprite frame = new Sprite();
                //background.Texture = this.content.Load<Texture2D>(backgroundName + nrOfBackgrounds);
                frame.Texture = this.content.Load<Texture2D>(backgroundName + "1");
                frame.Scale = scale;
                frame.Position = new Vector2(xPos, 0);
                frame.Velocity = velocity;
                backgrounds.Add(frame);
                xPos += frame.Size.Width;
            }

            // TODO: Load music and sounds
        }

        public void Update(GameTime gameTime)
        {
            if (backgrounds.Count == 0)
                throw new NotSupportedException("No background loaded.");

            // Update position of background frames
            Object[] bg = backgrounds.ToArray(); // background arraylist as an array
            Sprite lastBackground = (Sprite) bg[bg.Length - 1];
            for (int i = 0; i < bg.Length; i++)
			{
                Sprite frame = (Sprite)backgrounds[i];

                // TODO: flying backwards should also update LevelProgress-- to preserve level progress.
                if (frame.Position.X < -frame.Size.Width)
                {
			        if (i == 0)
                    {
                        frame.Position = new Vector2 (lastBackground.Position.X + lastBackground.Size.Width, lastBackground.Position.Y);
                    } else
                    {
                        Sprite prevFrame = (Sprite)backgrounds[i - 1];
                        frame.Position = new Vector2(prevFrame.Position.X + prevFrame.Size.Width,
                                                                prevFrame.Position.Y);
                    }

                    LevelProgress++;

                    addEnemies = true;
                    Console.WriteLine(levelProgress);
                }

                // TODO: Background only goes very slowly up or down! because of ElapsedGameTime
                frame.Position += -1 * velocity; //* (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

            // Load enemies depending on current background frame
            // future: important: specify loading level layout (and type of enemies based on level) from txt file
            // future: dynamically load/unload/save enemies depending on current background frame.

            if (LevelProgress == 1 && addEnemies)
            {
                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized");
                zeppelin.Position = new Vector2(1400, 300);
                zeppelin.Velocity = new Vector2(2, 0);
                zeppelin.WorthScore = 100;
                enemies.Add(zeppelin);
                addEnemies = false;
            }
            else if (LevelProgress == 2 && addEnemies)
            {
                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized");
                zeppelin.Position = new Vector2(1410, 300);
                zeppelin.Velocity = new Vector2(-2, -1);
                zeppelin.WorthScore = 150;
                enemies.Add(zeppelin);
                addEnemies = false;
            }

            // update enemy sprite speed relative to player speed
            foreach (Enemy enemy in enemies.ToArray()) //ToArray important to avoid iterating-remove-error
            {
                if (enemy.HasExploded)
                {
                    enemies.Remove(enemy);
                }
                else
                {
                    enemy.Position += -1 * velocity; // BUGS: when player flies up/down
                    enemy.Update();
                }
            }

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
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
