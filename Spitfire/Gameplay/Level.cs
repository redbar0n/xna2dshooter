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
        //private int sounds;

        public ContentManager Content
        {
            get { return content; }
        }
        private ContentManager content;

        private Sprite sky;
        private Sprite skyTwo;
        private List<Sprite> backgrounds;
        private float backgroundBufferWidth = 0;
        public Sprite levelGround; // The ground within the level. Made public for collision detection
        public Sprite levelGroundTwo;

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
            levelGround = new Sprite();
            levelGroundTwo = new Sprite();
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

            // Load Ground
            levelGround.Texture = this.content.Load<Texture2D>("Sprites/backgrounds/earth[1]");
            levelGround.Position = new Vector2(0, 680f); // Not sure how to get height of a background frame
            levelGroundTwo.Texture = levelGround.Texture;
            levelGroundTwo.Position = new Vector2(levelGround.Position.X + levelGround.Size.Width, levelGround.Position.Y);

            // TODO: Load music and sounds
        }

        /// <summary>
        /// TODO: resolve sky fram alignment bug
        /// </summary>
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
        /// TODO: resolve sky fram alignment bug
        /// </summary>
        private void UpdateGround()
        {
            if (levelGround.Position.X < -levelGround.Size.Width)
            {
                levelGround.Position = new Vector2(levelGroundTwo.Position.X + levelGroundTwo.Size.Width, levelGroundTwo.Position.Y);
            }
            else if (levelGroundTwo.Position.X < -levelGroundTwo.Size.Width)
            {
                levelGroundTwo.Position = new Vector2(levelGround.Position.X + levelGround.Size.Width, levelGround.Position.Y);
            }
            else if (levelGround.Position.X > levelGround.Size.Width)
            {
                // can use sky.Size.Width in the check because only two sprites. otherwise would have needed a bufferwidth like with backgrounds
                levelGround.Position = new Vector2(-levelGround.Size.Width + 8, levelGround.Position.Y); // 8 = magic constant to avoid mysterious gap between frames
            }
            else if (levelGroundTwo.Position.X > levelGroundTwo.Size.Width)
            {
                levelGroundTwo.Position = new Vector2(-levelGroundTwo.Size.Width + 8, levelGroundTwo.Position.Y);
            }


            // sky frames only scroll horizontally
            levelGround.Position += new Vector2(-1 * velocity.X, -1 * velocity.Y);
            levelGroundTwo.Position += new Vector2(-1 * velocity.X, -1 * velocity.Y);
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
                Enemy mig = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig.Position = new Vector2(1400, 100);
                mig.Velocity = new Vector2(-4, 0);
                mig.WorthScore = 100;
                mig.StartHP = 10;
                enemies.Add(mig);
                addEnemies = false;

                Enemy mig2 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig2.Position = new Vector2(1450, 300);
                mig2.Velocity = new Vector2(-4, 0);
                mig2.WorthScore = 100;
                mig2.StartHP = 10;
                enemies.Add(mig2);
                addEnemies = false;
            }
            else if (positionInLevel == 2 && addEnemies)
            {
                Enemy hfighter = new Enemy(this, Enemy.Type.Exploding, "heavyfighter", false);
                hfighter.Position = new Vector2(1410, 250);
                hfighter.Velocity = new Vector2(-2, 0);
                hfighter.StartHP = 20;
                hfighter.WorthScore = 150;
                enemies.Add(hfighter);

                LightTank superTank = new LightTank(this, Enemy.Difficulty.Hard, "lighttankspritemapfinal", true);
                superTank.Velocity = new Vector2(-2, 0);
                superTank.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float) (superTank.Size.Height * 0.3));
                enemies.Add(superTank);


                addEnemies = false;
            }
            else if (positionInLevel == 3 && addEnemies)
            {
                Enemy ltank = new Enemy(this, Enemy.Type.Exploding, "lighttankspritemapfinal", true);
                ltank.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float) (ltank.Size.Height * 0.3));
                // Y-calculation is to align it with the background
                ltank.Velocity = new Vector2(-4, 0);
                ltank.WorthScore = 100;
                ltank.StartHP = 10;
                enemies.Add(ltank);

                Mig migMedium = new Mig(this, Enemy.Difficulty.Hard, "mig", false);
                migMedium.Position = new Vector2(1440, 300);
                migMedium.Velocity = new Vector2(-4, 0);
                enemies.Add(migMedium);

                Enemy ltank2 = new Enemy(this, Enemy.Type.Exploding, "lighttankspritemapfinal", true);
                ltank2.Position = new Vector2(1500, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(ltank.Size.Height * 0.3));
                // Y-calculation is to align it with the background
                ltank2.Velocity = new Vector2(-2, 0);
                ltank2.WorthScore = 100;
                ltank2.StartHP = 10;
                enemies.Add(ltank2);
                addEnemies = false;
            }
            else if (positionInLevel == 4 && addEnemies)
            {
                Enemy htank = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank.Size.Height * 0.3));
                // Y-calculation is to align it with the background
                htank.Velocity = new Vector2(-1, 0);
                htank.WorthScore = 100;
                htank.StartHP = 50;
                enemies.Add(htank);


                Enemy htank2 = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank2.Position = new Vector2(1550, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank.Size.Height * 0.3));
                // Y-calculation is to align it with the background
                htank2.Velocity = new Vector2(-1, 0);
                htank2.WorthScore = 100;
                htank2.StartHP = 50;
                enemies.Add(htank2);
                addEnemies = false;
            }
            else if (positionInLevel == 6 && addEnemies)
            {
                Enemy mig = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig.Position = new Vector2(1350, 100);
                mig.Velocity = new Vector2(-1, 0);
                mig.WorthScore = 100;
                mig.StartHP = 10;
                enemies.Add(mig);

                Enemy mig2 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig2.Position = new Vector2(1550, 100);
                mig2.Velocity = new Vector2(-1, 0);
                mig2.WorthScore = 100;
                mig2.StartHP = 10;
                enemies.Add(mig2);
                


                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized", false);
                zeppelin.Position = new Vector2(1450, 200);
                zeppelin.Velocity = new Vector2(-1, 0);
                zeppelin.WorthScore = 100;
                zeppelin.StartHP = 100;
                enemies.Add(zeppelin);
                
                Enemy mig3 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig3.Position = new Vector2(1350, 300);
                mig3.Velocity = new Vector2(-1, 0);
                mig3.WorthScore = 100;
                mig3.StartHP = 10;
                enemies.Add(mig3);
                
                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);
                addEnemies = false;
            }/*
            else if (positionInLevel == 6 && addEnemies)
            {
                Enemy mig = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig.Position = new Vector2(1350, 100);
                mig.Velocity = new Vector2(-1, 0);
                mig.WorthScore = 100;
                mig.StartHP = 10;
                enemies.Add(mig);

                Enemy mig2 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig2.Position = new Vector2(1550, 100);
                mig2.Velocity = new Vector2(-1, 0);
                mig2.WorthScore = 100;
                mig2.StartHP = 10;
                enemies.Add(mig2);


                Enemy mig3 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig3.Position = new Vector2(1350, 300);
                mig3.Velocity = new Vector2(-1, 0);
                mig3.WorthScore = 100;
                mig3.StartHP = 10;
                enemies.Add(mig3);

                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                Enemy mig5 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig5.Position = new Vector2(1550, 300);
                mig5.Velocity = new Vector2(-1, 0);
                mig5.WorthScore = 100;
                mig5.StartHP = 10;
                enemies.Add(mig5);
                // gg
                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                Enemy mig4 = new Enemy(this, Enemy.Type.Exploding, "mig", false);
                mig4.Position = new Vector2(1550, 300);
                mig4.Velocity = new Vector2(-1, 0);
                mig4.WorthScore = 100;
                mig4.StartHP = 10;
                enemies.Add(mig4);

                addEnemies = false;
            }*/
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

            UpdateGround();

            LoadNewEnemies();

            UpdateEnemies();

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Draw the sky
            sky.Draw(spriteBatch);
            skyTwo.Draw(spriteBatch);

            levelGround.Draw(spriteBatch);
            levelGroundTwo.Draw(spriteBatch);

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
