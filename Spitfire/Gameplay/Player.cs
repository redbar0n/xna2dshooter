using System;
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

        private bool isSwooping;
        //private bool isTurning = false; // Determines if you are in the middle of a turn
        
        //private bool isDodging;
        //private int dodgeTime;



        /// <summary>
        /// Bullets that the player has fired.
        /// </summary>
        public ArrayList Bullets
        {
            get { return bullets; }
            set { bullets = value; }
        }
        private ArrayList bullets;


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
            set { currentHP = value; }
        }
        private int currentHP;

        //private Vector2 maxVelocity;
        private float pi = (float)Math.PI;
        private Vector2 initialVelocity = new Vector2(8f, 8f);
        private const float maxRotation = 0.78f; // how much the plane can rotate either up/down
        //private float rotateDistance = 0.02f;
        private float accelerant = 1.0f; // future: change to using dv/dt
        private float accelerationConstant = 0.25f;
        private static float maxAccelerant = 2.7f;
        private float decelerationConstant = 0.25f;

        // TODO: rewrite. are isAccelerating and isDecelerating needed?
        private bool isAccelerating = false;
        private bool isDecelerating = false;

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
        private SoundEffect burstSound;
        private SoundEffect engineSound;
        private SoundEffect bombSound;
        private SoundEffect swoopSound;
        //private SoundEffect playerExplosion;


        //private int burstTime;
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
        private Animation currentAni;

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
                else
                {
                    return normalAni.Texture;
                }
            }
        }

        // future: method to fly to end of screen when reached level end.

        public Player(GraphicsDeviceManager graphics, ContentManager content)
        {
            base.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            base.Velocity = initialVelocity;
            determineVelocity();
            currentHP = 100;
            faceDirection = FaceDirection.Right;
            bullets = new ArrayList();
            bombs = new ArrayList();
            bulletDamage = 10;
            bombCount = 50;
            animate = new AnimationPlayer();
            // NickSound
            //burstSound = content.Load<SoundEffect>("Sounds/Player/Browning, short burst 2, 278626_SOUNDDOGS__gu");
            //bombSound = content.Load<SoundEffect>("Sounds/Player/");
            //swoopSound = content.Load<SoundEffect>("Sounds/Player/");
            //engineSound = content.Load<SoundEffect>("Sounds/Player/005");
           // engineSound.Play(0.2f, 0.0f, 0.0f, true);
        }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // future: optimize the following if-sentences
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (faceDirection == FaceDirection.Left)
                {

                }
                else
                {
                    TurnAround();
                    //SlowDown();
                }
            }

            if (keyboardState.IsKeyDown(Keys.A))
                Console.WriteLine(this.accelerant);

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (faceDirection == FaceDirection.Right)
                {

                }
                else
                {
                    TurnAround();
                }
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Swoop();
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                //FlyUp();
                isSwooping = false;
                RotateUp();
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                //FlyDown(); // QUESTION: plane will actually fly up here
                isSwooping = false;
                RotateDown();
            }
            else
            {
                isSwooping = false;
                AutoAdjustRotation();
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !spaceKeyWasPressed)
            {
                Shoot();
                spaceKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.Space))
            {
                spaceKeyWasPressed = false;
            }

            ///Drop bomb ///
            if (keyboardState.IsKeyDown(Keys.D) && !dKeyWasPressed)
            {
                DropBomb();
                dKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.D))
            {
                dKeyWasPressed = false;
            }
            

            isAccelerating = false;
            isDecelerating = false;
        }

        private void setAnimation(Animation ani)
        {
            currentAni = ani;
            animate.PlayAnimation(ani);
        }

        public void Update()
        {
            // TODO: update player logic

            GetInput();

            determineVelocity();

            setAnimation(normalAni);

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
            SpriteEffects flip = base.faceDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            animate.Draw(gameTime, spriteBatch, Position, Rotation, flip);

            foreach (Bullet bullet in bullets.ToArray())
                bullet.Draw(spriteBatch);

            foreach (Bomb bombX in bombs.ToArray())
                bombX.Draw(spriteBatch);
        }


        public void TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP < 0)
            {
                Die();
            }
        }

        private void RotateUp()
        {
            if (faceDirection == FaceDirection.Right)
            {
                Rotation -= (float)1 / 120 * pi;
                if (Rotation < -pi / 2)
                    Rotation = -pi / 2;
            }
            else
            {
                Rotation += (float)1 / 120 * pi;
                if (Rotation > pi / 2)
                    Rotation = pi / 2;

            }
        }

        private void RotateDown()
        {
            if (faceDirection == FaceDirection.Right)
            {
                Rotation += (float)1 / 120 * pi;
                if (Rotation > pi / 2)
                    Rotation = pi / 2;
            }
            else
            {
                Rotation -= (float)1 / 120 * pi;
                if (Rotation < -pi / 2)
                    Rotation = -pi / 2;
            }
        }


        /// <summary>
        /// Will determine velocity based on rotation. More velocity if flying down, less if up.
        /// </summary>
        private void determineVelocity()
        {
            float yPercent = Rotation / (pi / 2);
            float xPercent = 1 - Math.Abs(yPercent);

            if (faceDirection == FaceDirection.Right)
            {
                if (Rotation > 0)
                {
                    //accelerant += accelerationConstant * yPercent;
                    if (isSwooping)
                        Accelerate(Math.Abs(yPercent));
                    //Velocity = new Vector2(initialVelocity.X * xPercent * accelerant, initialVelocity.Y * yPercent * accelerant);
                }
                else if (Rotation < 0)
                {
                    Decelerate(Math.Abs(yPercent));

                }

            }
            else
            {
                if (Rotation > 0)
                {
                    Decelerate(Math.Abs(yPercent));


                }
                else if (Rotation < 0)
                {
                    if (isSwooping)
                        Accelerate(Math.Abs(yPercent));
                }
            }
            Velocity = new Vector2(initialVelocity.X * xPercent * accelerant * (float)faceDirection,
                initialVelocity.Y * yPercent * accelerant * (float)faceDirection);
        }

        private void AutoAdjustRotation()
        {

            if (faceDirection == FaceDirection.Right)
            {
                if (Rotation > 0)
                {
                    RotateUp();
                    if (Rotation < 0)
                        Rotation = 0f;
                }
                else if (Rotation < 0)
                {
                    RotateDown();
                    if (Rotation > 0)
                        Rotation = 0f;
                }
            }
            else
            {
                if (Rotation > 0)
                {
                    RotateDown();
                    if (Rotation < 0)
                        Rotation = 0f;
                }
                else if (Rotation < 0)
                {
                    RotateUp();
                    if (Rotation > 0)
                        Rotation = 0f;
                }
            }      
        }

        /// <summary>
        /// Slows the plane down and makes it turn around when the accelerant value < 1
        /// Resets the accelerant when the plane turns around
        /// </summary>
        /// <remarks>
        /// future: should be removed, plane shouldn't stop mid-air. need other way of turning.
        /// </remarks>
        private void TurnAround()
        {
            this.accelerant -= decelerationConstant;
            if (this.Rotation == 0 && faceDirection == FaceDirection.Right && accelerant < 1)
            {
                faceDirection = FaceDirection.Left;
                accelerant = 1.0f;
            }
            else if (this.Rotation == 0 && faceDirection == FaceDirection.Left && accelerant < 1)
            {
                faceDirection = FaceDirection.Right;
                accelerant = 1.0f;
            }

        }

        private void Swoop()
        {
            RotateDown();
            RotateDown();
            isSwooping = true;
        }

        /*
        public void Dodge()
        {
            throw new System.NotImplementedException();
        }
        */

        public void Die()
        {
            Console.WriteLine("YOU DIED");
            //throw new System.NotImplementedException();
        }

        public void Accelerate()
        {
            if ((accelerant < 2.0f) && (!isDecelerating))
            {
                isAccelerating = true;
                this.accelerant += accelerationConstant;
            }
        }


        public void Accelerate(float yPercent)
        {
            if (accelerant < maxAccelerant)
                //if (Rotation < 1.0f)
                //    accelerant += accelerationBigConstant * yPercent;
                //else
                accelerant += accelerationConstant * yPercent;
            if (accelerant > maxAccelerant)
                accelerant = maxAccelerant;
        }

        public void Decelerate(float yPercent)
        {
            if (accelerant > 1.0f)
                accelerant -= (decelerationConstant * yPercent / 4);
            else if (accelerant < 1.0f)
                accelerant = 1.0f;

        }

        public void Decelerate()
        {
            if (!isAccelerating)
            {
                isDecelerating = true;
                this.accelerant -= accelerationConstant;
                if (this.accelerant < 0f)
                {
                    this.accelerant = 0f;
                }
            }
        }

        public void Shoot()
        {
            Bullet bullet = new Bullet(this.Rotation, this.Position, this.faceDirection);
            bullet.Texture = bulletSprite;
            bullets.Add(bullet);
            // NickSound
            //burstSound.Play();
        }

        public void DropBomb()
        {
            if (bombCount > 0)
            {
                Bomb bombX = new Bomb(this.Rotation, this.Position, this.Velocity, this.faceDirection);
                bombX.Texture = bombSprite;
                bombs.Add(bombX);
                bombCount--;
            }
        }
    }
}
