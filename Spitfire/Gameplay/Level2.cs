using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
        public ContentManager Content
        {
            get { return content; }
        }
        private ContentManager content;

        private GameScreen gameplayScreen;

        public int levelNumber;

        private Sprite sky;
        private Sprite skyTwo;
        private List<Sprite> backgrounds;
        private float backgroundBufferWidth = 0;

        public List<Sprite> grounds; // The ground within the level. Made public for collision detection
        private float groundsBufferWidth = 0;

        /// <summary>
        /// Indicates position in the level. Updated when frames loop.
        /// </summary>
        /// <remarks>
        /// A simple frame counter that ticks as frames loop.
        /// </remarks>
        public float positionInLevel = 0;

        /// <summary>
        /// Indicates furthest progress in the level. Updated when frames loop.
        /// </summary>
        private float levelProgress = 0;

        // for testing: cheat method to get to finalboss
        public void setLevelProgress(int i)
        {
            positionInLevel = i;
            levelProgress = i;
            addEnemies = true;
        }

        /// <summary>
        /// The base velocity of everything in the level. Objects can have own velocity in addition.
        /// Based on the player velocity.
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;


        public Vector2 playersPosition {
            get { return playerPosition; }
            set { playerPosition = value; }
        }
        private Vector2 playerPosition;

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



        //public List<Pickup> Pickups
        //{
        //    get { return pickups; }
        //    set { pickups = value; }

        //}
        List<Pickup> pickups;


        /// <summary>
        /// How much background should be scaled. Redundant if background texture images are scaled.
        /// </summary>
        private float scale = 2.1f; // NB: may cause error since scale is in sprite as well

        public Level(GameScreen gameplayScreen)
        {
            levelNumber = 2;
            backgrounds = new List<Sprite>();
            enemies = new List<Enemy>();
            pickups = new List<Pickup>();
            sky = new Sprite();
            skyTwo = new Sprite();
            grounds = new List<Sprite>();
            this.gameplayScreen = gameplayScreen;
        }

        public void changeLevel(int levelnr)
        {
            backgroundBufferWidth = 0;
            groundsBufferWidth = 0;
            setLevelProgress(0);
            backgrounds.Clear();
            grounds.Clear();
            enemies.Clear();
            pickups.Clear();

            levelNumber = levelnr;
            if (levelnr == 1)
            {
                LoadContent(content, "Sprites/Backgrounds/Mountain/mountainfinal", "Sprites/Backgrounds/ground_final", 4);
            }
            else if (levelnr == 2)
            {
                LoadContent(content, "Sprites/Backgrounds/City/cityback1_0", "Sprites/Backgrounds/groundtwo_final_tmp", 3);
            }
        }

        // future: Optimize background loading to remove slight lag. Load smaller backgrounds.
        public void LoadContent(ContentManager content, String backgroundName, String groundName, int nrOfBackgrounds)
        {
            this.content = content;
            
            // Load sky
            sky.Texture = this.content.Load<Texture2D>("Sprites/backgrounds/finalsky");
            sky.Position = new Vector2(0, 0);
            skyTwo.Texture = sky.Texture;
            skyTwo.Position = new Vector2(sky.Position.X + sky.Size.Width, 0);

            // Load backgrounds
            for (int i = 1; i <= nrOfBackgrounds; i++)
            {
                Sprite frame = new Sprite();
                frame.Texture = this.content.Load<Texture2D>(backgroundName + i); // should be set to i if more than one background
                frame.Scale = scale;
                frame.Position = new Vector2(backgroundBufferWidth, 0); // buffer width indicates xpos to insert background
                frame.Velocity = velocity;
                backgrounds.Add(frame);
                backgroundBufferWidth += frame.Size.Width; // increase width of buffer
            }

            /*
            // load an extra background buffer
            for (int i = 1; i <= nrOfBackgrounds; i++)
            {
                Sprite frame = new Sprite();
                frame.Texture = this.content.Load<Texture2D>(backgroundName + i); // should be set to i if more than one background
                frame.Scale = scale;
                frame.Position = new Vector2(backgroundBufferWidth, 0); // buffer width indicates xpos to insert background
                frame.Velocity = velocity;
                backgrounds.Add(frame);
                backgroundBufferWidth += frame.Size.Width; // increase width of buffer
            }

             */
            
            // Load Ground
            for (int i = 0; i < 6; i++)
            {
                Sprite frame = new Sprite();
                frame.Texture = this.content.Load<Texture2D>(groundName);
                frame.Position = new Vector2(groundsBufferWidth, backgrounds[0].Size.Height);
                frame.Velocity = velocity;
                grounds.Add(frame);
                groundsBufferWidth += frame.Size.Width;
            }

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
            else if (sky.Position.X > sky.Size.Width) // plane is going left
            {
                // can use sky.Size.Width in the check since only two sprites. otherwise would have needed a bufferwidth like with backgrounds
                sky.Position = new Vector2(skyTwo.Position.X - sky.Size.Width, 0);
            }
            else if (skyTwo.Position.X > skyTwo.Size.Width)
            {
                skyTwo.Position = new Vector2(sky.Position.X - skyTwo.Size.Width, 0);
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
            
            // update the frames position
            for (int i = 0; i < bg.Length; i++)
            {
                Sprite frame = (Sprite)backgrounds[i];
                // ElapsedGameTime causes background to go very slowly up or down. Is it necessary?
                frame.Position += -1 * velocity; //* (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // make the frames loop continuously when going either right or left
            for (int i = 0; i < bg.Length; i++)
            {
                Sprite frame = (Sprite)backgrounds[i];

                if (frame.Position.X < -frame.Size.Width)
                {
                    // plane is going to the right

                    // only because backgrounds stored in array. relates them to each other in a loop.
                    if (i == 0)
                    {
                        // only called when background nr 0 goes completely outside LEFT side of screen
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
                        //Console.WriteLine("levelProgress: " + levelProgress);
                    }
                }
                else if (frame.Position.X > (backgroundBufferWidth - frame.Size.Width))
                {
                    // plane is going to the left

                    // only because backgrounds stored in array. relates them to each other in a loop.
                    if (i == (bg.Length - 1))
                    {
                        // only called when the last background goes completely outside RIGHT side of screen
                        frame.Position = new Vector2(firstBackground.Position.X - frame.Size.Width, firstBackground.Position.Y);
                    }
                    else
                    {
                        // since i != (bg.Length - 1) we are sure that the array has more backgrounds
                        Sprite nextFrame = (Sprite)backgrounds[i + 1];
                        frame.Position = new Vector2(nextFrame.Position.X - frame.Size.Width, nextFrame.Position.Y);
                    }


                    positionInLevel--;
                    addEnemies = false;
                    //Console.WriteLine(positionInLevel);
                }
            }
        }

        private void UpdateGround()
        {
            if (grounds.Count == 0)
                throw new NotSupportedException("No ground loaded.");

            Object[] bg = grounds.ToArray();
            Sprite firstGround = (Sprite)bg[0];
            Sprite lastGround = (Sprite)bg[bg.Length - 1];

            // update the frames position
            for (int i = 0; i < bg.Length; i++)
            {
                Sprite frame = (Sprite)grounds[i];
                frame.Position += -1 * velocity;
            }

            // make the frames loop continuously when going either right or left
            for (int i = 0; i < bg.Length; i++)
            {
                Sprite frame = (Sprite)grounds[i];

                if (frame.Position.X < -frame.Size.Width)
                {
                    // plane is going to the right
                    if (i == 0)
                    {
                        // only called when background nr 0 goes completely outside LEFT side of screen
                        frame.Position = new Vector2(lastGround.Position.X + lastGround.Size.Width, lastGround.Position.Y);
                    }
                    else
                    {
                        Sprite prevFrame = (Sprite)grounds[i - 1];
                        frame.Position = new Vector2(prevFrame.Position.X + prevFrame.Size.Width,
                                                                prevFrame.Position.Y);
                    }
                }
                else if (frame.Position.X > (groundsBufferWidth - frame.Size.Width))
                {
                    // plane is going to the left

                    // only because backgrounds stored in array. relates them to each other in a loop.
                    if (i == (bg.Length - 1))
                    {
                        // only called when the last background goes completely outside RIGHT side of screen
                        frame.Position = new Vector2(firstGround.Position.X - frame.Size.Width, firstGround.Position.Y);
                    }
                    else
                    {
                        // since i != (bg.Length - 1) we are sure that the array has more backgrounds
                        Sprite nextFrame = (Sprite)grounds[i + 1];
                        frame.Position = new Vector2(nextFrame.Position.X - frame.Size.Width, nextFrame.Position.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Based on level progress
        /// </summary>
        private void LoadNewEnemiesLevel1()
        {
            // Load enemies depending on current background frame
            // future: important: specify loading level layout (and type of enemies based on level) from txt file
            // future: dynamically load/unload/save enemies depending on current background frame.
            if (positionInLevel == 1 && addEnemies)
            {
                // 2 light fighters scouting the area

                Mig migMedium1 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium1.Position = new Vector2(1400, grounds[0].Position.Y - 100f);
                migMedium1.Velocity = new Vector2(-4, 0);
                enemies.Add(migMedium1);

                Mig migMedium2 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium2.Position = new Vector2(1450, grounds[0].Position.Y - 300f);
                migMedium2.Velocity = new Vector2(-4, 0);
                enemies.Add(migMedium2);

                Pickup pickupA = new Pickup(new Vector2(1000f, 100f), Pickup.Effect.HP);
                pickupA.Texture = this.content.Load<Texture2D>("Sprites/healthcrate");
                pickupA.Scale = 0.2f;
                pickups.Add(pickupA);

                addEnemies = false;
            }
            else if (positionInLevel == 2 && addEnemies)
            {
                // 1 heavy fighter responds to distress calls from the scouts

                Enemy hfighter = new Enemy(this, Enemy.Type.Exploding, "heavyfighter", false);
                hfighter.Position = new Vector2(1410, 250);
                hfighter.Velocity = new Vector2(-2, 0);
                hfighter.StartHP = 20;
                hfighter.WorthScore = 150;
                enemies.Add(hfighter);

                addEnemies = false;
            }
            else if (positionInLevel == 3 && addEnemies)
            {
                // 2 light tanks scouting the area

                LightTank superTank1 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank1.Velocity = new Vector2(-4, 0);
                superTank1.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank1.Size.Height * 0.3));
                enemies.Add(superTank1);

                LightTank superTank2 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank2.Velocity = new Vector2(-2, 0);
                superTank2.Position = new Vector2(1500, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank2.Size.Height * 0.3));
                enemies.Add(superTank2);

                addEnemies = false;
            }
            else if (positionInLevel == 4 && addEnemies)
            {
                // 2 heavy tanks opening up a path

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
                // A Zeppelin floats, guarded by 4 light fighters

                Mig migMedium1 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium1.Position = new Vector2(1350, grounds[0].Position.Y - 300f);
                migMedium1.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium1);

                Mig migMedium2 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium2.Position = new Vector2(1550, grounds[0].Position.Y - 300f);
                migMedium2.Velocity = new Vector2(-4, 0);
                enemies.Add(migMedium2);

                Mig migMedium3 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium3.Position = new Vector2(1350, grounds[0].Position.Y - 100f);
                migMedium3.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium3);

                Mig migMedium4 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium4.Position = new Vector2(1550, grounds[0].Position.Y - 100f);
                migMedium4.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium4);

                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized_tmp_flipped", true);
                zeppelin.Position = new Vector2(1450, 200);
                zeppelin.Velocity = new Vector2(-1, 0);
                zeppelin.WorthScore = 100;
                zeppelin.StartHP = 100;
                enemies.Add(zeppelin);

                addEnemies = false;
            }
            else if (positionInLevel == 8 && addEnemies)
            {
                // 10 light fighters spearhead the attack from the distress call of the Zeppelin.

                Mig migMedium1 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium1.Position = new Vector2(1400, grounds[0].Position.Y - 250f);
                migMedium1.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium1);

                Mig migMedium2 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium2.Position = new Vector2(1500, grounds[0].Position.Y - 215f);
                migMedium2.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium2);

                Mig migMedium3 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium3.Position = new Vector2(1500, grounds[0].Position.Y - 265f);
                migMedium3.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium3);

                Mig migMedium4 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium4.Position = new Vector2(1600, grounds[0].Position.Y - 180f);
                migMedium4.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium4);

                Mig migMedium5 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium5.Position = new Vector2(1600, grounds[0].Position.Y - 230f);
                migMedium5.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium5);

                Mig migMedium6 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium6.Position = new Vector2(1600, grounds[0].Position.Y - 280f);
                migMedium6.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium6);

                Mig migMedium7 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium7.Position = new Vector2(1700, grounds[0].Position.Y - 145f);
                migMedium7.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium7);

                Mig migMedium8 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium8.Position = new Vector2(1700, grounds[0].Position.Y - 195f);
                migMedium8.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium8);

                Mig migMedium9 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium9.Position = new Vector2(1700, grounds[0].Position.Y - 245f);
                migMedium9.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium9);

                Mig migMedium10 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium10.Position = new Vector2(1700, grounds[0].Position.Y - 295f);
                migMedium10.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium10);

                addEnemies = false;
            }
            else if (positionInLevel == 11 && addEnemies)
            {
                // 8 light fighters and 4 light tanks, additional reinforcements.

                Mig migMedium1 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium1.Position = new Vector2(1300, grounds[0].Position.Y - 200f);
                migMedium1.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium1);

                Mig migMedium2 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium2.Position = new Vector2(1300, grounds[0].Position.Y - 300f);
                migMedium2.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium2);

                Mig migMedium3 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium3.Position = new Vector2(1400, grounds[0].Position.Y - 200f);
                migMedium3.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium3);

                Mig migMedium4 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium4.Position = new Vector2(1400, grounds[0].Position.Y - 300f);
                migMedium4.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium4);

                Mig migMedium5 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium5.Position = new Vector2(1500, grounds[0].Position.Y - 100f);
                migMedium5.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium5);

                Mig migMedium6 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium6.Position = new Vector2(1500, grounds[0].Position.Y - 200f);
                migMedium6.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium6);

                Mig migMedium7 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium7.Position = new Vector2(1600, grounds[0].Position.Y - 300f);
                migMedium7.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium7);

                Mig migMedium8 = new Mig(this, Enemy.Difficulty.Easy, "mig", false);
                migMedium8.Position = new Vector2(1600, grounds[0].Position.Y - 400f);
                migMedium8.Velocity = new Vector2(-1, 0);
                enemies.Add(migMedium8);

                LightTank superTank1 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank1.Velocity = new Vector2(-2, 0);
                superTank1.Position = new Vector2(1300, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank1.Size.Height * 0.3));
                enemies.Add(superTank1);

                LightTank superTank2 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank2.Velocity = new Vector2(-2, 0);
                superTank2.Position = new Vector2(1500, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank2.Size.Height * 0.3));
                enemies.Add(superTank2);

                LightTank superTank3 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank1.Velocity = new Vector2(-2, 0);
                superTank1.Position = new Vector2(1700, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank3.Size.Height * 0.3));
                enemies.Add(superTank3);

                LightTank superTank4 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank2.Velocity = new Vector2(-2, 0);
                superTank2.Position = new Vector2(1900, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank4.Size.Height * 0.3));
                enemies.Add(superTank4);

                addEnemies = false;
            }
            else if (positionInLevel == 13 && addEnemies)
            {
                // 2 heavy tanks, more forces.

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
            else if (positionInLevel == 15 && addEnemies)
            {
                // 4 heavy tanks, even more forces.

                Enemy htank = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank.Size.Height * 0.3));
                htank.Velocity = new Vector2(-1, 0);
                htank.WorthScore = 100;
                htank.StartHP = 50;
                enemies.Add(htank);


                Enemy htank2 = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank2.Position = new Vector2(1550, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank.Size.Height * 0.3));
                htank2.Velocity = new Vector2(-1, 0);
                htank2.WorthScore = 100;
                htank2.StartHP = 50;
                enemies.Add(htank2);

                Enemy htank3 = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank3.Position = new Vector2(1700, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank3.Size.Height * 0.3));
                htank3.Velocity = new Vector2(-1, 0);
                htank3.WorthScore = 100;
                htank3.StartHP = 50;
                enemies.Add(htank3);

                Enemy htank4 = new Enemy(this, Enemy.Type.Exploding, "finalheavytanksprite", true);
                htank4.Position = new Vector2(1850, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(htank4.Size.Height * 0.3));
                htank4.Velocity = new Vector2(-1, 0);
                htank4.WorthScore = 100;
                htank4.StartHP = 50;
                enemies.Add(htank4);

                addEnemies = false;
            }
            else if (positionInLevel == 17 && addEnemies)
            {
                // A Zeppelin and 2 heavy fighter, the other half of the major strike force.

                Enemy hfighter = new Enemy(this, Enemy.Type.Exploding, "heavyfighter", false);
                hfighter.Position = new Vector2(1300, 250);
                hfighter.Velocity = new Vector2(-1, 0);
                hfighter.StartHP = 20;
                hfighter.WorthScore = 150;
                enemies.Add(hfighter);

                Enemy zeppelin = new Enemy(this, Enemy.Type.Exploding, "zeppelin2sized_tmp_flipped", false);
                zeppelin.Position = new Vector2(1550, 250);
                zeppelin.Velocity = new Vector2(-1, 0);
                zeppelin.WorthScore = 100;
                zeppelin.StartHP = 100;
                enemies.Add(zeppelin);


                Enemy hfighter2 = new Enemy(this, Enemy.Type.Exploding, "heavyfighter", false);
                hfighter2.Position = new Vector2(1850, 250);
                hfighter2.Velocity = new Vector2(-1, 0);
                hfighter2.StartHP = 20;
                hfighter2.WorthScore = 150;
                enemies.Add(hfighter2);

                addEnemies = false;
            }
            else if (positionInLevel == 19 && addEnemies)
            {
                // 16 light tanks, the remaining forces, rush out of the city in a last stand.

                LightTank superTank1 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank1.Velocity = new Vector2(-2, 0);
                superTank1.Position = new Vector2(1300, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank1.Size.Height * 0.3));
                enemies.Add(superTank1);

                LightTank superTank2 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank2.Velocity = new Vector2(-2, 0);
                superTank2.Position = new Vector2(1450, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank2.Size.Height * 0.3));
                enemies.Add(superTank2);

                LightTank superTank3 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank1.Velocity = new Vector2(-2, 0);
                superTank1.Position = new Vector2(1600, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank3.Size.Height * 0.3));
                enemies.Add(superTank3);

                LightTank superTank4 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                superTank2.Velocity = new Vector2(-2, 0);
                superTank2.Position = new Vector2(1750, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank4.Size.Height * 0.3));
                enemies.Add(superTank4);

                LightTank supertank5 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank5.Velocity = new Vector2(-2, 0);
                supertank5.Position = new Vector2(1900, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank5.Size.Height * 0.3));
                enemies.Add(supertank5);

                LightTank supertank6 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank6.Velocity = new Vector2(-2, 0);
                supertank6.Position = new Vector2(2050, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank6.Size.Height * 0.3));
                enemies.Add(supertank6);

                LightTank supertank7 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank7.Velocity = new Vector2(-2, 0);
                supertank7.Position = new Vector2(2200, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank7.Size.Height * 0.3));
                enemies.Add(supertank7);

                LightTank supertank8 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank8.Velocity = new Vector2(-2, 0);
                supertank8.Position = new Vector2(2350, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank8.Size.Height * 0.3));
                enemies.Add(supertank8);

                LightTank supertank9 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank9.Velocity = new Vector2(-2, 0);
                supertank9.Position = new Vector2(2500, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank9.Size.Height * 0.3));
                enemies.Add(supertank9);

                LightTank supertank10 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank10.Velocity = new Vector2(-2, 0);
                supertank10.Position = new Vector2(2650, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank10.Size.Height * 0.3));
                enemies.Add(supertank10);

                LightTank supertank11 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank11.Velocity = new Vector2(-2, 0);
                supertank11.Position = new Vector2(2800, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank11.Size.Height * 0.3));
                enemies.Add(supertank11);

                LightTank supertank12 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank12.Velocity = new Vector2(-2, 0);
                supertank12.Position = new Vector2(2950, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank12.Size.Height * 0.3));
                enemies.Add(supertank12);

                LightTank supertank13 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank13.Velocity = new Vector2(-2, 0);
                supertank13.Position = new Vector2(3100, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank13.Size.Height * 0.3));
                enemies.Add(supertank13);

                LightTank supertank14 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank14.Velocity = new Vector2(-2, 0);
                supertank14.Position = new Vector2(3250, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank14.Size.Height * 0.3));
                enemies.Add(supertank14);

                LightTank supertank15 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank15.Velocity = new Vector2(-2, 0);
                supertank15.Position = new Vector2(3400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank15.Size.Height * 0.3));
                enemies.Add(supertank15);

                LightTank supertank16 = new LightTank(this, Enemy.Difficulty.Easy, "lighttankspritemapfinal", true);
                supertank16.Velocity = new Vector2(-2, 0);
                supertank16.Position = new Vector2(3550, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(supertank16.Size.Height * 0.3));
                enemies.Add(supertank16);

                addEnemies = false;
            }
            else if (positionInLevel == 26 && addEnemies)
            {
                // END LEVEL
                changeLevel(2);
                //MediaPlayer.Stop();
                // TODO: kill all sounds
                //gameplayScreen.ExitScreen();
                // TODO: Add level summary screen
            }
        }

        private void LoadNewEnemiesLevel2()
        {
            if (positionInLevel == 1 && addEnemies)
            {
                // 1 heavy fighter responds to distress calls from the scouts

                Enemy hfighter = new Enemy(this, Enemy.Type.Exploding, "heavyfighter", false);
                hfighter.Position = new Vector2(1410, 250);
                hfighter.Velocity = new Vector2(-2, 0);
                hfighter.StartHP = 20;
                hfighter.WorthScore = 150;
                enemies.Add(hfighter);

                LightTank superTank = new LightTank(this, Enemy.Difficulty.Hard, "lighttankspritemapfinal", true);
                superTank.Velocity = new Vector2(-2, 0);
                superTank.Position = new Vector2(1400, backgrounds[0].Position.Y + backgrounds[0].Size.Height - (float)(superTank.Size.Height * 0.3));
                enemies.Add(superTank);

                addEnemies = false;
            }
            else if (positionInLevel == 25 && addEnemies)
            {
                Enemy finalboss = new Enemy(this, Enemy.Type.Exploding, "finalboss", true);
                finalboss.Position = new Vector2(1450, 200);
                finalboss.Velocity = new Vector2(-1, 0);
                finalboss.WorthScore = 1000;
                finalboss.StartHP = 1000;
                enemies.Add(finalboss);

                addEnemies = false;
            }
        }

        /// <summary>
        /// Update enemy speed relative to player speed, and remove exploded enemies.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies.ToArray())
            {
                if (enemy.HasExploded)
                {
                    //Create an item pickup
                    Pickup item = new Pickup(enemy.Position, Pickup.Effect.HP);
                    item.Texture = this.content.Load<Texture2D>("Sprites/healthcrate");
                    item.Scale = 0.2f;
                    pickups.Add(item);
                    enemies.Remove(enemy);
                }
                else
                {
                    //enemy.Position += -1 * velocity;
                    //enemy.Update();
                    enemy.Update(velocity,playerPosition, gameTime);
                }
            }
        }

        private void UpdatePickUps() { 
            foreach (Pickup pickup in pickups.ToArray())
                if (pickup.HasCollided)
                {
                    pickups.Remove(pickup);
                }
                else {
                    pickup.Update(velocity);
                }
        
        }

        public void Update(GameTime gameTime)
        {
            UpdateSky();

            UpdateBackground();

            UpdateGround();

            UpdateEnemies(gameTime); // important that this is called before LoadNewEnemies, because otherwise new enemies will get velocity updated twice

            if (levelNumber == 1)
            {
                LoadNewEnemiesLevel1();
            }
            else if (levelNumber == 2)
            {
                LoadNewEnemiesLevel2();
            }

            UpdatePickUps();

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Draw the sky
            sky.Draw(spriteBatch);
            skyTwo.Draw(spriteBatch);

            foreach (Sprite frame in grounds)
            {
                frame.Draw(spriteBatch);
            }

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
            //Draw pickups
            foreach (Pickup pickup in pickups) {
                pickup.Draw(spriteBatch);
            }

        }
    }
}
