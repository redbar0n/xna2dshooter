using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    public class Player : Sprite
    {
        //private bool isDodging;
        //private bool isSwooping;
        //private bool isTurning = false; // Determines if you are in the middle of a turn
        //private int dodgeTime;
        //private int maxHP;


        public ArrayList _shots;
        private Texture2D bulletSprite;
        public Texture2D bulletTexture
        {
            get { return bulletSprite; }
            set { bulletSprite = value; }
        }

        /// <summary>
        /// Used to determine whether space key was pressed on last update
        /// </summary>
        Boolean _spaceKeyWasPressed;
        /// <summary>
        /// Used to determine whether the d key was pressed on last update
        /// </summary>
        Boolean _dKeyWasPressed;


        public ArrayList bombs;
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
        private float rotateDistance = 0.02f;
        private float accelerant = 1.0f; // future: change to using dv/dt
        private float accelerationConstant = 0.25f;

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

        //private int bombDamage;
        //private int bulletDamage;
        //private int burstTime;
        private AnimationPlayer animate;

        public Animation NormalFlight
        {
            get { return normalFlight; }
            set
            {
                normalFlight = value;
            }
        }
        private Animation normalFlight;

        // future: method to fly to end of screen when reached level end.

        public Player(GraphicsDeviceManager graphics)
        {
            base.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            base.Velocity = initialVelocity;
            determineVelocity();
            currentHP = 100;
            //faceDirection = FaceDirection.Right;
            _shots = new ArrayList();
            bombs = new ArrayList();
            bombCount = 3;
        }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // future: optimize the following if-sentences
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (faceDirection == FaceDirection.Left)
                {
                    Accelerate();
                }
                else
                {
                    SlowDown();
                }
            }

            if (keyboardState.IsKeyDown(Keys.A))
                Console.WriteLine(this.accelerant);

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (faceDirection == FaceDirection.Right)
                {
                    Accelerate();
                }
                else
                {
                    SlowDown();
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                //FlyUp();
                RotateUp();
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                //FlyDown(); // QUESTION: plane will actually fly up here
                RotateDown();
            }
            else
            {
                AutoAdjustRotation();
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !_spaceKeyWasPressed)
            {
                Shoot();
                _spaceKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.Space))
            {
                _spaceKeyWasPressed = false;
            }

            ///Drop bomb ///
            if (keyboardState.IsKeyDown(Keys.D) && !_dKeyWasPressed)
            {
                dropBomb();
                _dKeyWasPressed = true;
            }
            else if (!keyboardState.IsKeyDown(Keys.D))
            {
                _dKeyWasPressed = false;
            }



            //XVelocityReset();
            //YVelocityReset();
            //AccelerationReset();
            isAccelerating = false;
            isDecelerating = false;
        }

        public void Update()
        {
            // TODO: update player logic

            GetInput();

            determineVelocity();

            animate.PlayAnimation(normalFlight);

            foreach (Bullet _shot1 in _shots.ToArray())
            {
                //_shot1.Position.X > 1280 || _shot1.Position.X < 0
                if (_shot1.HasExceededDistance())
                {
                    _shots.Remove(_shot1);
                }
                else
                {
                    _shot1.Update(this.Velocity);
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
            // draw in direction enemy is facing
            SpriteEffects flip = base.faceDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            animate.Draw(gameTime, spriteBatch, Position, Rotation, flip);

            foreach (Bullet _shot1 in _shots.ToArray())
                _shot1.Draw(spriteBatch);

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
            Rotation -= (float)1 / 120 * pi;
            if (Rotation < -pi / 2)
                Rotation = -pi / 2;
        }

        private void RotateDown()
        {
            Rotation += (float)1 / 120 * pi;
            if (Rotation > pi / 2)
                Rotation = pi / 2;
        }

        private void determineVelocity()
        {
            float yPercent = Rotation / (pi / 2);            
            float xPercent = 1 - Math.Abs(yPercent);

            if (Rotation > 0)
            {
                //accelerant += accelerationConstant * yPercent;
                Accelerate(yPercent);
                //Velocity = new Vector2(initialVelocity.X * xPercent * accelerant, initialVelocity.Y * yPercent * accelerant);
            }
            else if (Rotation < 0)
            {
                Decelerate(Math.Abs(yPercent));
                
            }
            Velocity = new Vector2(initialVelocity.X * xPercent * accelerant,
                initialVelocity.Y * yPercent * accelerant);
        }

        private void AutoAdjustRotation()
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

        /*
                private void FlyUp()
                {

                    // plane faces right and greater than - 70 degrees approx
                    if ((Rotation > -1.2f) && (faceDirection == FaceDirection.Right))
                    {
                        Rotation -= rotateDistance;//* accelerant;

                        base.Velocity += new Vector2(0f, 0.1f);
                        if (Rotation < -maxRotation)
                        {
                            base.Velocity += new Vector2(0.15f, 0f);
                        }
                        else if (Rotation > maxRotation)
                        {
                            base.Velocity -= new Vector2(0.15f, 0f);
                        }
                    }

                    //plane faces left and greater than - 70 degrees approx
                    if ((Rotation < 1.2f) && (faceDirection == FaceDirection.Left))
                    {
                        base.Velocity += new Vector2(0f, 0.1f);
                
                        // future: use math clamp instead
                        Rotation += rotateDistance;//* accelerant;
                        if (Rotation > maxRotation)
                        {
                            base.Velocity -= new Vector2(0.15f, 0f);
                        }

                    }
                }

                private void FlyDown()
                {
                    if ((Rotation < 1.2f) && (faceDirection == FaceDirection.Right))
                    {
                        {
                            Rotation += rotateDistance ;//* accelerant;
                            base.Velocity -= new Vector2(0f, 0.1f);
                            if (Rotation <= -maxRotation)
                            {
                                base.Velocity -= new Vector2(0.15f, 0f);
                            }
                            else if (Rotation >= maxRotation) {
                                base.Velocity += new Vector2(0.15f, 0f);
                            }
                        }
                    }
                    if ((Rotation > -1.2f) && (faceDirection == FaceDirection.Left))
                    {
                        Rotation -= rotateDistance;//* accelerant;
                        base.Velocity -= new Vector2(0f, 0.1f);
                        if (Rotation > maxRotation)
                        {
                            base.Velocity += new Vector2(0.15f, 0f);
                        }
                    }

                }
        */
        /*
        public float GetX_Speed()
        {
            return base.Velocity.X * accelerant;
        }

        public float GetY_Speed()
        {
            return base.Velocity.Y * accelerant;
        }
        */


        /// <summary>
        /// Slows the plane down and centres its rotation. Once the base.Velocity reaches 0, the plane turns around.
        /// </summary>
        /// <remarks>
        /// future: make other way of turning. plane shouldn't stop mid-air.
        /// </remarks>
        private void SlowDown()
        {
            if (faceDirection == FaceDirection.Right)
            {

                if (Rotation > 0)
                {
                    //FlyUp();
                    RotateUp();
                }
                else if (Rotation < 0)
                {
                    //FlyDown();
                    RotateDown();
                }
            }
            else
            { // facing left
                if (Rotation < 0)
                {
                    //FlyUp();
                    RotateUp();
                }
                else if (Rotation > 0)
                {
                    //FlyDown();
                    RotateDown();

                }
            }
            Decelerate();
            // To make sure that the plane's rotation is nearly at 0 before turning around
            if ((Rotation < 0.05f) && (Rotation > -0.05f))
                TurnAround();
        }


        /// <summary>
        /// Reset xbase.Velocity after getting input.
        /// </summary>
        /// <remarks>
        /// future: could xbase.VelocityReset and ybase.VelocityReset be avoided?
        /// </remarks>
        public void XVelocityReset()
        {
            if (faceDirection == FaceDirection.Right)
            {
                if ((Rotation > -maxRotation) && (Rotation < maxRotation))
                {
                    if (base.Velocity.X > -5f)
                    {
                        base.Velocity -= new Vector2(0.5f, 0f);
                        if (base.Velocity.X < -5f) base.Velocity = new Vector2(-5f, base.Velocity.Y); // capped at -5
                    }
                }
            }
            else if (faceDirection == FaceDirection.Left)
            {
                if ((Rotation > -maxRotation) && (Rotation < maxRotation))
                {
                    if (base.Velocity.X < 5f)
                    {
                        base.Velocity += new Vector2(0.5f, 0f);
                        if (base.Velocity.X > 5f) base.Velocity = new Vector2(5f, base.Velocity.Y); // capped at 5
                    }
                }
            }
        }

        public void YVelocityReset()
        {
            if ((Rotation > -rotateDistance) && (Rotation < rotateDistance))
            {
                base.Velocity = new Vector2(0f, base.Velocity.Y);
            }
        }

        /// <summary>
        /// Reset acceleration after getting input.
        /// </summary>
        /// <remarks>
        /// future: could accelerationReset be avoided?
        /// </remarks>
        public void AccelerationReset()
        {
            if ((!isAccelerating) && (!isDecelerating))
            {
                if (accelerant > 1)
                {
                    accelerant -= accelerationConstant;
                    if (accelerant < 1)
                        accelerant = 1;
                }

                if (accelerant < 1)
                {
                    accelerant += accelerationConstant;
                    if (accelerant > 1)
                        accelerant = 1;
                }

            }
        }

        /// <summary>
        /// Turns plane around if x-speed is 0.
        /// </summary>
        /// <remarks>
        /// future: should be removed, plane shouldn't stop mid-air. need other way of turning.
        /// </remarks>
        public void TurnAround()
        {
            if (base.Velocity.X == 0)
            {
                if (faceDirection == FaceDirection.Right)
                {
                    faceDirection = FaceDirection.Left;
                }
                else
                {
                    faceDirection = FaceDirection.Right;
                }

            }
        }

        /* future: TurnRight and TurnLeft could be used as the new turn mechanic
        public void TurnRight()
        {
            throw new System.NotImplementedException();
        }

        public void TurnLeft()
        {
            throw new System.NotImplementedException();
        }
        */

        /* future: FireBullet(), DropBomb() and Dodge() implementation
        public void FireBullet()
        {
            throw new System.NotImplementedException();
        }

        public void DropBomb()
        {
            throw new System.NotImplementedException();
        }

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
            if (accelerant < 3.0f)
                accelerant += accelerationConstant * yPercent;
            if (accelerant > 3.0f)
                accelerant = 3.0f;
        }

        public void Decelerate(float yPercent)
        {
            if (accelerant > 1.0f)
                accelerant -= (accelerationConstant * yPercent/4);
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
            
            //Bullet shot = new Bullet(this.Rotation, this.animate.Origin);
            Bullet shot = new Bullet(this.Rotation, this.Position);
            shot.Texture = bulletSprite;
            _shots.Add(shot);

        }

        public void dropBomb()
        {
            if (bombCount > 0)
            {
                Bomb bombX = new Bomb(this.Rotation, this.Position, this.Velocity);
                bombX.Texture = bombSprite;
                bombs.Add(bombX);
                bombCount--;
            }
        }
    }
}
