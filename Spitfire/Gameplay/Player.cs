﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
    public class Player : Sprite
    {

        #region CONSTANTS
        public int PLAYER_BULLET_DAMAGE_START = 10;
        public int PLAYER_BOMB_START = 30;
        #endregion

        /// <summary>
        /// The amount of damage one bullet will do.
        /// </summary>
        public float DistanceFromGround
        {
            get { return bulletDamage; }
            set { distanceFromGround = value; }
        }
        private float distanceFromGround;
        public static float MAXHEIGHT = 850f;
        // Prevents the player from flying upwards when true
        private bool disableRightUpwardMovement = false;
        private bool disableLeftUpwardMovement = false;

        Random random = new Random();
        
        /// <summary>
        /// Bullets that the player has fired.
        /// </summary>
        public ArrayList Bullets
        {
            get { return bullets; }
            set { bullets = value; }
        }
        private ArrayList bullets;

        private bool crashGround;

        /// <summary>
        /// The amount of damage one bullet will do.
        /// </summary>
        public int BulletDamage
        {
            get { return bulletDamage; }
            set { bulletDamage = value; }
        }
        private int bulletDamage;

        // TODO: relocate to Bullet class
        private Texture2D bulletSprite;
        public Texture2D bulletTexture
        {
            get { return bulletSprite; }
            set { bulletSprite = value; }
        }

        /// <summary>
        /// Used to determine whether space key was pressed on last update
        /// </summary>
        Boolean spaceKeyWasPressed;
       
        /// <summary>
        /// Used to determine whether the d key was pressed on last update
        /// </summary>
        Boolean dKeyWasPressed;


        public ArrayList bombs;


        /*
        public int BombDamage
        {
            get { return bombDamage; }
            set { bombDamage = value; }
        }
        private int bombDamage;
        */

        // TODO: relocate to Bombs class
        private Texture2D bombSprite;
        public Texture2D bombTexture
        {
            get { return bombSprite; }
            set { bombSprite = value; }
        }

        /// <summary>
        /// The players current hit points.
        /// </summary>
        public int CurrentHP
        {
            get { return currentHP; }
            set { 
                currentHP = Math.Min(value, 100);
            }
        }
        private int currentHP;

        /// <summary>
        /// Detertines if collision detection should be performed with player.
        /// </summary>
        public bool IsImmortal
        {
            get
            {
                return isImmortal;
            }
        }
        private bool isImmortal = false; 

        public bool isAlive = true;

        //private Vector2 maxVelocity;
        private float pi = (float)Math.PI;
        private Vector2 initialVelocity = new Vector2(8f, 8f);
        private const float maxRotation = 0.78f; // how much the plane can rotate either up/down
        //private float rotateDistance = 0.02f;
        
        
        private float accelerant = 1.0f; // future: change to using dv/dt
        private float minAcceleration = 1.0f;
        private float maxAcceleraion = 2.0f;

        /// <summary>
        /// Determines if the player is slowing the plane down.
        /// </summary>
        private bool isSlowing = false;
        private float accelerationConstant = 0.125f; //0.25f;
        private static float maxAccelerant = 2.0f; //2.5f;
        private static float minAccelerant = 0.5f;
        private static float maxSlowedAccelerant = 1.0f;
        
        private float decelerationConstant = 0.25f;

        /// Variables to determine rumble feature
        TimeSpan rumbleStartTime;
        private int rumbleDuration = 200;
        private bool wasDamaged = false;


        /// <summary>
        /// The players current number of bombs.
        /// </summary>
        public int BombCount
        {
            get { return bombCount; }
            set { bombCount = value; }
        }
        private int bombCount;
        //private int bombCapacity;

        /// player sounds
        private SoundEffect bulletSound;
        private SoundEffect hitSound;
        private SoundEffect engineSound;
        public SoundEffectInstance engineSoundInst;
        public const float engineSoundVolume = 0.3f;

        //private SoundEffect playerExplosion;


        //private int burstTime;
        private int burstAmmount = 3; //Number of shots per burst
        private int burstCount; // Countdown of the no shots remaining in a burst. Vale set in constructor
        private int burstDelay = 70; // The Delay between each shot
        private int burstDelayCount; // The countdown of the delay. decrements each up date      
        TimeSpan bulletCreationTime;
        

        private bool isShooting = false;

        public bool avoidFirstFlipRight;
        public bool avoidFirstFlipLeft;
        /// <summary>
        /// When a player taps up or down, they won't rotate as far
        /// </summary>
        float rotateMultiplyer = 1.0f;


        private AnimationPlayer animate;

        public Animation NormalAni
        {
            get { return normalAni; }
            set
            {
                normalAni = value;
            }
        }
        private Animation normalAni;
        private Animation explodeAni;
        private Animation currentAni;

        private Animation flipAwayRight;
        private Animation flipAwayLeft;

        private Animation flipIntoRight;
        private Animation flipIntoLeft;

        private bool useAwayFlip = true;

        private bool flip = false; // A variable to determine is the plane is to be flipped or not.

        private Level level;

        /// <summary>
        /// A variable to determine how the up/down keys rotate the plane in the getinput method
        /// </summary>
        private bool controlIsRight = true; 

        /// <summary>
        /// Gets the bounding box positioned in world. Can also be used to get sprite width and height in world.
        /// </summary>
        public override Rectangle Size
        {
            get
            {
                return new Rectangle((int)Math.Round(Position.X - animate.Origin.X),
                        (int)Math.Round(Position.Y - animate.Origin.Y),
                        (int)(currentAni.FrameWidth * Scale),
                        (int)(currentAni.FrameHeight * Scale));
            }
        }
       
        /// <summary>
        /// Returns the animation texture if one is present (instead of default Sprite texture)
        /// </summary>
        public override Texture2D Texture
        {
            get
            {
                if (currentAni == null)
                {
                    return base.Texture;
                }
                else if (currentAni == explodeAni)
                {
                    return explodeAni.Texture;
                }
                else if (currentAni == flipAwayRight)
                {
                    return flipAwayRight.Texture;
                }
                else
                {
                    return normalAni.Texture;
                }
            }
        }

        ScreenManager screenManager;
        public HUD hud;

        public Player(ScreenManager screenManager, ContentManager content, Level level)
        {
            this.level = level;
            level.setPlayer(this);
            this.screenManager = screenManager;
            base.Position = new Vector2(this.screenManager.GraphicsDevice.Viewport.Width / 2, this.screenManager.GraphicsDevice.Viewport.Height / 2);
            base.Velocity = initialVelocity;
            determineVelocity();
            currentHP = 100;
            faceDirection = FaceDirection.Right;
            avoidFirstFlipRight = true;
            avoidFirstFlipLeft = false;
            bullets = new ArrayList();
            bombs = new ArrayList();
            bulletDamage = PLAYER_BULLET_DAMAGE_START;
            bombCount = PLAYER_BOMB_START;
            animate = new AnimationPlayer();
            burstCount = burstAmmount; //The default for burstAmmount is 3
        }

        public void resetStats()
        {
            isAlive = true;
            base.Velocity = initialVelocity;
            Rotation = 0;
            accelerant = 1;
            faceDirection = FaceDirection.Right;
            avoidFirstFlipRight = true;
            avoidFirstFlipLeft = false;
            currentHP = 100;
            bulletDamage = PLAYER_BULLET_DAMAGE_START;
            bombCount = PLAYER_BOMB_START;
            animate = new AnimationPlayer();
            currentAni = normalAni;
            animate.PlayAnimation(normalAni);

            bullets = new ArrayList();
            bombs = new ArrayList();
            burstCount = burstAmmount; //The default for burstAmmount is 3
        }

        public void LoadContent(ContentManager content)
        {
            NormalAni = new Animation(content.Load<Texture2D>("Sprites/Player/stillspitfire_final"), 0.08f, 1f, false);
            //NormalAni = new Animation(content.Load<Texture2D>("Sprites/Player/turnawayfromscreenmap"), 0.08f, 1f, false);
            //normalAni.IsLoopingBackwards = true;

            explodeAni = new Animation(content.Load<Texture2D>("Sprites/Enemies/expllarge_final"), 1f, 1f, false);

            flipAwayRight = new Animation(content.Load<Texture2D>("Sprites/Player/turnawayfromscreenmap_right"), 0.08f, 1f, false);
            flipAwayRight.IsGoingRightToLeft = true;

            flipAwayLeft = new Animation(content.Load<Texture2D>("Sprites/Player/turnawayfromscreenmap_right_upsidedown"), 0.08f, 1f, false);
            flipAwayLeft.IsGoingRightToLeft = true;

            flipIntoRight = new Animation(content.Load<Texture2D>("Sprites/Player/turnintoscreenmap_right"), 0.08f, 1f, false);
            flipIntoRight.IsGoingRightToLeft = true;

            flipIntoLeft = new Animation(content.Load<Texture2D>("Sprites/Player/turnintoscreenmap_right_upsidedown"), 0.08f, 1f, false);
            flipIntoLeft.IsGoingRightToLeft = true;

            //flipInto = new Animation(content.Load<Texture2D>("Sprites/Player/turnintoscreenmap"), 0.08f, 1f, false);
            //flipInto.IsGoingRightToLeft = true;

            bulletTexture = content.Load<Texture2D>("Sprites/Player/hero_ammo_final");
            bombTexture = content.Load<Texture2D>("Sprites/Player/herobomb_final");
            
            // NickSound
            bulletSound = content.Load<SoundEffect>("Sounds/Player/Single_shot1");
            engineSound = content.Load<SoundEffect>("Sounds/Player/Engine1");
            Bomb.BombSound = content.Load<SoundEffect>("Sounds/Player/whistle");
            Pickup.PickupSound = content.Load<SoundEffect>("Sounds/Chime");
            Bomb.explosionSound = content.Load<SoundEffect>("Sounds/explode_light2");
            
            engineSoundInst = engineSound.Play(engineSoundVolume, 0.0f, 0.0f, true);
            hitSound = content.Load<SoundEffect>("Sounds/Enemy/ricochet_hard");
        }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);


            float movement = gamePad.ThumbSticks.Left.Y * 1.0f; //MoveStickScale;
            float xMovement = gamePad.ThumbSticks.Left.X * 1.0f;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.1f)
            {
                movement = 0.0f;

                // determine controlIsRight value                
                if (!flip)
                    controlIsRight = true;
                else
                    controlIsRight = false;
            }
            if (Math.Abs(xMovement) < 0.2f)
                xMovement = 0.0f;


            /// Movement Controls

            if (xMovement > 0 && faceDirection == FaceDirection.Right)
            {
                setFlip(FaceDirection.Right);
                AutoAdjustRotation();
            }
            else if (xMovement < 0 && faceDirection == FaceDirection.Left)
            {
                setFlip(FaceDirection.Left);
                AutoAdjustRotation();
            }
            else if (movement > 0)
            {
                if (controlIsRight && !disableRightUpwardMovement)
                    minusRotation(1.5f * movement);
                else if (!controlIsRight && !disableLeftUpwardMovement)
                    plusRotation(1.5f * movement);
                else
                    AutoHeightCorrect();
                if ((distanceFromGround < MAXHEIGHT))
                {
                    disableLeftUpwardMovement = false;
                    disableRightUpwardMovement = false;
                }
            }
            else if (movement < 0)
            {
                if (controlIsRight && !disableLeftUpwardMovement)
                    minusRotation(1.5f * movement);
                else if (!controlIsRight && !disableRightUpwardMovement)
                    plusRotation(1.5f * movement);
                else
                    AutoHeightCorrect();
                if ((distanceFromGround < MAXHEIGHT))
                {
                    disableLeftUpwardMovement = false;
                    disableRightUpwardMovement = false;
                }

            }
            else if (keyboardState.IsKeyDown(Keys.Left) && faceDirection == FaceDirection.Left) {
                setFlip(FaceDirection.Left);
                if (faceDirection == FaceDirection.Left)
                    AutoAdjustRotation();

            }
            else if (keyboardState.IsKeyDown(Keys.Right) && faceDirection == FaceDirection.Right) {
                setFlip(FaceDirection.Right);
                if (faceDirection == FaceDirection.Right)
                    AutoAdjustRotation();

            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                rotateMultiplyer += 0.5f;
                if (rotateMultiplyer >= 1.5f) {
                    rotateMultiplyer = 1.5f;
                }
                
                if (controlIsRight && !disableRightUpwardMovement)
                    minusRotation(rotateMultiplyer);
                else if (!controlIsRight && !disableLeftUpwardMovement)
                    plusRotation(rotateMultiplyer);
                else
                    AutoHeightCorrect();
                if ((distanceFromGround < MAXHEIGHT))
                {
                    disableLeftUpwardMovement = false;
                    disableRightUpwardMovement = false;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                rotateMultiplyer += 0.5f;
                if (rotateMultiplyer >= 1.5f)
                {
                    rotateMultiplyer = 1.5f;
                }
                
                if (controlIsRight && !disableLeftUpwardMovement)
                    plusRotation(rotateMultiplyer);
                else if (!controlIsRight && !disableRightUpwardMovement)
                    minusRotation(rotateMultiplyer);
                else
                    AutoHeightCorrect();
                if ((distanceFromGround < MAXHEIGHT))
                {
                    disableLeftUpwardMovement = false;
                    disableRightUpwardMovement = false;
                }
            }// No movement controls are pressed
            else
            {
                rotateMultiplyer = 1.0f;
                
                if (distanceFromGround > MAXHEIGHT)
                {
                    AutoHeightCorrect();
                    if (faceDirection == FaceDirection.Right)
                        disableRightUpwardMovement = true;
                    else
                        disableLeftUpwardMovement = true;

                }
                else
                {
                    disableLeftUpwardMovement = false;
                    disableRightUpwardMovement = false;
                }

                // determine controlIsRight value                
                if (!flip)
                    controlIsRight = true;
                else
                    controlIsRight = false;

            }

            //Make plane speed across the screen. Player is invincible 
            //while button is pressed
            if (keyboardState.IsKeyDown(Keys.L))
            {
                if (Math.Sin(Rotation) == 0)
                {
                    isImmortal = true;
                    accelerant += 0.125f;
                    if (accelerant > 7f)
                    {
                        accelerant = 7f;
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.I))
            {
                if (!isImmortal)
                {
                    hud.drawImmortal = true;
                    Console.WriteLine("immortal");
                    isImmortal = true;
                }
                else
                {
                    hud.drawImmortal = false;
                    Console.WriteLine("not immortal");
                    isImmortal = false;
                }
            }


            //Decellerates the plane
            if (keyboardState.IsKeyDown(Keys.Left) && faceDirection == FaceDirection.Right)
                isSlowing = true;
            else if (keyboardState.IsKeyDown(Keys.Right) && faceDirection == FaceDirection.Left)
                isSlowing = true;
            else if (xMovement < -0.2f && faceDirection == FaceDirection.Right)
                isSlowing = true;
            else if (xMovement > 0.2f && faceDirection == FaceDirection.Left)
                isSlowing = true;
            else
                isSlowing = false;


            if (keyboardState.IsKeyDown(Keys.Space) && !spaceKeyWasPressed)
            {
                if (!isShooting)
                {
                    setIsShooting();
                    spaceKeyWasPressed = true;
                }
            }
            else if (gamePad.IsButtonDown(Buttons.A) && !spaceKeyWasPressed)
            {
                if (!isShooting)
                {
                    setIsShooting();
                    spaceKeyWasPressed = true;
                }
            }
            else if (!keyboardState.IsKeyDown(Keys.Space) && !gamePad.IsButtonDown(Buttons.A))
            {
                spaceKeyWasPressed = false;
            }

            ///Drop bomb ///
            if (keyboardState.IsKeyDown(Keys.D) && !dKeyWasPressed)
            {
                DropBomb();
                dKeyWasPressed = true;
            }
            else if (gamePad.IsButtonDown(Buttons.X) && !dKeyWasPressed)
            {
                DropBomb();
                dKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.D) && !gamePad.IsButtonDown(Buttons.X))
            {
                dKeyWasPressed = false;
            }

            
        }

        public void setHud(HUD hud)
        {
            hud.setPlayer(this);
            this.hud = hud;
        }

        public void setAnimation(Animation ani)
        {
            currentAni = ani;
            animate.PlayAnimation(ani);
        }

        
        /// <summary>
        /// Flips the plane up/down. Opposite to its current position
        /// </summary>
        /// <param name="direction"></param>
        public void setFlip(FaceDirection direction)
        {
            if (direction == FaceDirection.Right)
            {
                avoidFirstFlipLeft = false;
                if (!avoidFirstFlipRight)
                {
                    avoidFirstFlipRight = true;
                    if (random.Next(1,3) < 2)
                    {
                        setAnimation(flipAwayLeft);
                        useAwayFlip = false;
                    }
                    else
                    {
                        setAnimation(flipIntoLeft);
                    }
                }
                flip = false;
            }
            else if (direction == FaceDirection.Left)
            {
                avoidFirstFlipRight = false;
                if (!avoidFirstFlipLeft)
                {
                    avoidFirstFlipLeft = true;
                    if (random.Next(1, 3) < 2)
                    {
                        setAnimation(flipAwayRight);
                        useAwayFlip = false;
                    }
                    else
                    {
                        setAnimation(flipIntoRight);
                        useAwayFlip = true;
                    }
                }
                flip = true;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            if (currentAni == explodeAni && explodeAni.IsFinished)
            {
                if (crashGround && currentHP > 0)
                {
                    isImmortal = false;
                    isAlive = true;
                    level.resetPositions();
                    setAnimation(normalAni);
                    crashGround = false;
                    explodeAni.IsFinished = false;
                    accelerant =  minAcceleration; //Resets acceleration
                    Rotation = 0;
                    flip = false;
                    //NickSound
                    if (!GameplayScreen.muted)
                        engineSoundInst.Resume();
                }
                else
                {
                    GameOverScreen gameOverScreen = new GameOverScreen(level, hud);
                    this.screenManager.AddScreen(gameOverScreen, null);
                }
            }

            if (isAlive)
            {
                GetInput();
                SlowDown();// Slows the plane down if they press decelerate
                determineVelocity();

                // Uncomment this code to make it that the player can move up and down the screen
                //base.Position += new Vector2(0, Velocity.Y);
                //base.Velocity = new Vector2(Velocity.X, 0f);

                if (wasDamaged)
                {
                    rumbleStartTime = gameTime.TotalGameTime;
                    wasDamaged = false;
                }

                if ((gameTime.TotalGameTime - rumbleStartTime).TotalMilliseconds < rumbleDuration)
                {
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                }


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

            foreach (Bullet bullet in bullets.ToArray())
            {
                //_shot1.Position.X > 1280 || _shot1.Position.X < 0
                if (bullet.HasExceededDistance())
                {
                    bullets.Remove(bullet);
                }
                else
                {
                    bullet.Update(this.Velocity);
                }
            }

            foreach (Bomb bombN in bombs.ToArray())
            {
                if (bombN.Position.Y > 3000f)
                {

                    bombs.Remove(bombN);
                }
                else
                    bombN.Update(this.Velocity);
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw in direction player is facing
            //SpriteEffects flipSprite = flip == true ? SpriteEffects.FlipVertically : SpriteEffects.None;            
            SpriteEffects flipSprite = SpriteEffects.None;
            animate.Draw(gameTime, spriteBatch, Position, Rotation, flipSprite);

            foreach (Bullet bullet in bullets.ToArray())
                bullet.Draw(spriteBatch);

            foreach (Bomb bombX in bombs.ToArray())
                bombX.Draw(spriteBatch);
        }


        public void TakeDamage(int damage)
        {
            //NickSound
            if (!GameplayScreen.muted)
                hitSound.Play();
            currentHP -= damage;
            wasDamaged = true; // Activates rumble feature
            if (currentHP < 0)
            {
                Die(false);
            }
        }

        /// <summary>
        /// Recovers the plane HP. 
        /// NOTE: Might to set max HP at some point
        /// </summary>
        /// <param name="damage"></param>
        public void Recover(int damage) {
            currentHP += damage;
            if (currentHP > 100)
                currentHP = 100;
        
        }

        
        /// <summary>
        /// Will determine velocity based on rotation. More velocity if flying down, less if up.
        /// </summary>
        private void determineVelocity()
        {
            float yPercent = (float)Math.Sin(Rotation);
            float xPercent = 1 - Math.Abs(yPercent);
            determineFaceDirection();

            //if (isSwooping) {
            //    Accelerate(Math.Abs(yPercent));
            //}
            if (Math.Sin(Rotation) > 0)
                Accelerate(Math.Abs(yPercent));

            if (Math.Sin(Rotation) < 0)
                Decelerate(Math.Abs(yPercent));

            // Wind resistance
            if (accelerant > minAcceleration)
            {
                accelerant -= 0.0125f;
                if (accelerant < minAcceleration)
                    accelerant = minAcceleration;
            }
            else if (accelerant < minAcceleration)
            {
                accelerant += (0.0125f / 2);
            }
                                    
            Velocity = new Vector2(initialVelocity.X * xPercent * accelerant * (float)faceDirection,
                initialVelocity.Y * yPercent * accelerant);
             
        }

        
        /// <summary>
        /// Determines the direction will fly based on the player's rotation
        /// </summary>
        private void determineFaceDirection()
        {

            if (Math.Cos(Rotation) > 0f)
                faceDirection = FaceDirection.Right; 
            else if (Math.Cos(Rotation) < 0f)
                faceDirection = FaceDirection.Left;
        }  

        /// <summary>
        /// Adjusts the player's rotation when flying input controls are realeased
        /// </summary>
        private void AutoAdjustRotation()
        {
            if (faceDirection == FaceDirection.Left)
            {
                if (Math.Sin(Rotation) < 0f)
                {
                    minusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = pi;
                }
                else
                {
                    plusRotation(1f);
                    if (Math.Sin(Rotation) < 0f)
                        Rotation = pi;
                }
            }
            else if (faceDirection == FaceDirection.Right)
                if (Math.Sin(Rotation) < 0f)
                {
                    plusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = 0;
                }
                else
                {
                    minusRotation(1f);
                    if (Math.Sin(Rotation) < 0f)
                        Rotation = 0;
                }
        }
        
        /// <summary>
        /// Adjusts the player's rotation when they reach maximum height
        /// </summary>
        private void AutoHeightCorrect() {
            if (faceDirection == FaceDirection.Left)
            {
                if (Math.Sin(Rotation) < 0f)
                {
                    minusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = pi;
                }
            }
            else if (faceDirection == FaceDirection.Right) {

                if (Math.Sin(Rotation) < 0f)
                {
                    plusRotation(1f);
                    if (Math.Sin(Rotation) > 0f)
                        Rotation = 0;
                }
            }

        }

        
        /// <summary>
        /// Makes the plane rotate anti clockwires. 
        /// </summary>
        /// <param name="ammount"></Multiply the default rotation distance>
        private void minusRotation(float ammount)
        {

            Rotation -= (float)1 / 120 * pi * ammount;

        }

        /// <summary>
        /// Makes the plane rotate clockwires.
        /// </summary>
        /// <param name="ammount"></Multiply the default rotation distance>
        private void plusRotation(float ammount)
        {

            Rotation += (float)1 / 120 * pi * ammount;

        }

        private void Swoop()
        {
            if (faceDirection == FaceDirection.Left)
            {
                minusRotation(1.5f);
                if (Math.Cos(Rotation) > 0)
                    Rotation = (0.5f * pi);
            }
            else
            {
                plusRotation(1.5f);
                if (Math.Cos(Rotation) < 0)
                    Rotation = (0.5f * pi);
            }
            //isSwooping = true;
        }

        /*
        public void Dodge()
        {
            throw new System.NotImplementedException();
        }
        */

        public void Die(bool crashGround)
        {
            base.Velocity = new Vector2(0, 0);
            this.crashGround = crashGround;
            isAlive = false;
            //engineSoundInst.Stop();
            setAnimation(explodeAni);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void GameOverMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);// Stop controller from rumbling
            LoadingScreen.Load(screenManager, false, null, new BackgroundScreen("Menus/gamemenu_final"), new MainMenuScreen());
        }

        public void Accelerate(float yPercent)
        {
 
            if (accelerant < maxAcceleraion)
                accelerant += accelerationConstant * yPercent;
            if (accelerant > maxAcceleraion)
                accelerant = maxAcceleraion;

        }

        public void Decelerate(float yPercent)
        {
            if (accelerant > minAcceleration)
                accelerant -= (decelerationConstant * yPercent / 4);
            if (accelerant < minAcceleration)
                accelerant = minAcceleration;

        }

        public void SlowDown()
        {
            if (isSlowing)
            {
                maxAcceleraion = maxSlowedAccelerant;
                minAcceleration = minAccelerant;
            }
            else
            {
                maxAcceleraion = maxAccelerant;
                minAcceleration = maxSlowedAccelerant;
            }

        }

        public void Shoot()
        {
            Bullet bullet = new Bullet(this.Rotation, this.Position, this.faceDirection);
            bullet.Texture = bulletSprite;
            bullets.Add(bullet);
            // NickSound
            if (!GameplayScreen.muted)
                bulletSound.Play();
        }

        /// <summary>
        /// Switches the player's isShooting value to false if true/ to true if true. 
        /// When isShooting is true. The player will shoot a burst of bullets equal to the burstAmmount
        /// </summary>
        /// <remarks>
        public void setIsShooting()
        {
            if (isShooting)
            {
                isShooting = false;
            }
            else
            {
                isShooting = true;
            }
        }

        public void DropBomb()
        {
            if (bombCount > 0)
            {
                Bomb bombX = new Bomb(this.Rotation, this.Position, this.Velocity, this.faceDirection);
                bombX.Texture = bombSprite;
                //NickSound
                bombX.PlayDropSound();
                bombs.Add(bombX);
                bombCount--;
            }
        }
    }
}
