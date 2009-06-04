using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
    public class Enemy : Sprite
    {
        /// <summary>
        /// The number of HP an enemy starts off with
        /// </summary>
        public int StartHP
        {
            get { return startHp; }
            set {
                startHp = value;
                currentHP = value;
            }
        }
        private int startHp;

        private int currentHP;


        /// <summary>
        /// The score points that the player will get for killing this enemy.
        /// </summary>
        public int WorthScore
        {
            get { return worthScore; }
            set { worthScore = value; }
        }
        private int worthScore;


        /// <summary>
        /// The amount of damage the enemy will deal to surrounding units when exploding.
        /// </summary>
        public int ExplodeDamage
        {
            get { return explodeDamage; }
            set { explodeDamage = value; }
        }
        private int explodeDamage;


        /// <summary>
        /// The associated level
        /// </summary>
        /// <remarks>
        /// For loading content, and determining enemy behaviour based on level status etc.
        /// </remarks>
        public Level Level
        {
            get { return level; }
            set { level = value; }
        }
        Level level;

        //private Curve path; // in case it is a flying enemy
        private Animation normalAni;
        private Animation explodeAni;
        private Animation currentAni; // current animation
        private AnimationPlayer animate;


        private bool flip = false; // A variable to determine is the enemy sprite is to be flipped or not.

        //public method to allow subclasses findout if the animation is finished or not
        public bool getAnimationFinish(){
            return animate.Animation.IsFinished;
        }


        /// <summary>
        /// A sprite which appears when the enemy appears offscreen. Use to indicate general direction.
        /// </summary>
        public Sprite arrowPointer = new Sprite();

        /// <summary>
        /// Returns the animation texture if one is present (instead of default Sprite texture).
        /// Returns the explode texture if it is exploding.
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
                else
                {
                    return normalAni.Texture;
                }
            }
        }

        public override float Scale
        {
            get
            {
                if (currentAni == null)
                {
                    return base.Scale;
                }
                else
                {
                    return currentAni.Scale;
                }
            }
            set
            {
                if (currentAni == null)
                {
                    base.Scale = value;
                }
                else
                {
                    explodeAni.Scale = value;
                    normalAni.Scale = value;
                }
            }
        }

        /// <summary>
        /// Tells if the explode animation is finished.
        /// </summary>
        public bool HasExploded
        {
            get { return hasExploded; }
            set { hasExploded = value; }
        }
        private bool hasExploded = false;

        /// <summary>
        /// Tells if the enemy is exploding. Used for updating the score from the Game class.
        /// </summary>
        public bool Exploding
        {
            get { return exploding; }
            set { exploding = value; }
        }
        private bool exploding = false;
        
        /// <summary>
        /// Enemy falling to ground or exploding. NB: may be redundant.
        /// </summary>
        public enum Type
        {
            Exploding,
            ShotDown,
        }
        private Type type;

        /// <summary>
        /// Enemy falling to ground or exploding. NB: may be redundant.
        /// </summary>
        public enum Difficulty
        {
            Easy,Medium,Hard
        }
        private Difficulty difficulty;


        // Sounds 
        private SoundEffect hitSound;
        private const float hitSoundVolume = 0.4f; 
        private SoundEffect explodeSound;
        private SoundEffect engineSound;
        public SoundEffectInstance engineSoundInst;
        private const float engineSoundVolume = 0.2f;

        
        /// <summary>
        /// Bullets that the enemy has fired.
        /// </summary>
        public List<Bullet> Bullets
        {
            get { return bullets; }
            set { bullets = value; }
        }
        private List<Bullet> bullets;


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


        // Bombs used by enemys NOTE: May not be used
        public ArrayList bombs;
        // TODO: relocate to Bombs class
        private Texture2D bombSprite;
        public Texture2D bombTexture
        {
            get { return bombSprite; }
            set { bombSprite = value; }
        }
        
        /// <summary>
        /// TODO: DEPRECATED. USE OTHER CONSTRUCTOR
        /// </summary>
        /// <param name="position">Position on screen</param>
        /// <param name="velocity">Velocity</param>
        /// <param name="scale">Scale size of sprite up or down.</param>
        /// <param name="maxHP">The starting hp of unit.</param>
        /// <param name="crashDamage">The damage the unit will do if crash or explode into other unit.</param>
        public Enemy(Level level, Type type, String spriteSet, bool looping)
        {
            this.level = level;
            this.type = type;
            bullets = new List<Bullet>();
            animate = new AnimationPlayer();
            LoadContent(spriteSet, looping);
        }

        /// <summary>





        public Enemy(Level level, Difficulty difficulty, String spriteSet, bool looping)
        {
            this.level = level;            
            animate = new AnimationPlayer();
            LoadContent(spriteSet, looping);
            this.difficulty = difficulty;
            Bullets = new List<Bullet>();
            bombs = new ArrayList();

        }

        /// <summary>
        /// Loads a particular enemy sprite sheet
        /// future: and sounds.
        /// </summary>
        public void LoadContent(String spriteSet, bool looping)
        {
            // Load animation(s).
            spriteSet = "Sprites/Enemies/" + spriteSet;

            normalAni = new Animation(Level.Content.Load<Texture2D>(spriteSet), 1f, 1f, looping);
            explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/expllarge_final"), 1f, 1f, false);

            ///TODO This animation makes the program crash. I have replaced it with the above statement
            //explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/zepplinexplspritemap"), 1f, false);

            arrowPointer.Texture = Level.Content.Load<Texture2D>("Sprites/Enemies/arrow");

            setAnimation(normalAni);

            // Load sounds
            if (spriteSet.Equals("Sprites/Enemies/mig"))
            {
                // NickSound
                engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Lightfighter/Engine1");
                engineSoundInst = engineSound.Play(0, 0.0f, 0.0f, true);
                hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/ricochet_soft");
                explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/explode_light1");
                bulletTexture = Level.Content.Load<Texture2D>("Sprites/Enemies/enemyammo");
            }
            else if (spriteSet.Equals("Sprites/Enemies/heavyfighter"))
            {
                // NickSound
                engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Heavyfighter/Engine3");
                engineSoundInst = engineSound.Play(0, 0.0f, 0.0f, true);
                hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/ricochet_soft");
                explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/explode_light1");
                bulletTexture = Level.Content.Load<Texture2D>("Sprites/Enemies/enemyammo");
            }
            else if (spriteSet.Equals("Sprites/Enemies/lighttankspritemapfinal") || spriteSet.Equals("Sprites/Enemies/finalheavytanksprite"))
            {
                // NickSound
                engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Tank/Tank");
                hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/ricochet_hard");
                explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/explode_large");
                engineSoundInst = engineSound.Play(0, 0.0f, 0.0f, true);
                bulletTexture = Level.Content.Load<Texture2D>("Sprites/Enemies/tankammo");
            }
            else if (spriteSet.Equals("Sprites/Enemies/zepplinarmoured_final"))
            {
                bulletTexture = Level.Content.Load<Texture2D>("Sprites/Enemies/tankammo");
                //NickSound
               engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Zeppelin/Engine2");
               engineSoundInst = engineSound.Play(0, 0.0f, 0.0f, true);
               hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/ricochet_hard");
               explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/explode_large");
               explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/zepplinexpl_final1"), 0.2f, 1f, false);
            }
            else if (spriteSet.Equals("Sprites/Enemies/ulimateweaponspritemap_final"))
            {
                //NickSound
                engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Zeppelin/Engine2");
                engineSoundInst = engineSound.Play(0, 0.0f, 0.0f, true);
                hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/ricochet_hard");
                explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/explode_large");
                explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/expllarge_final"), 0.2f, 1f, false);
            }
        }


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


        private void setAnimation(Animation ani)
        {
            currentAni = ani;
            animate.PlayAnimation(ani);
        }

        public void TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP <= 0)
            {
                //if (type == Type.ShotDown)
                //{
                //    ShotDown();
                //}
                //else
                //{
                    Explode(); // will pass the exploding down to animateExplosion, which will pass it up to level which finally removes enemy
                //}
            }
        }

        public void playHitSound()
        {
            // NickSound
            if (!GameplayScreen.muted)
                hitSound.Play(hitSoundVolume);
        }

        public void ShotDown()
        {
            //future: animate.PlayAnimation(shotDownAni);
        }


        /// <summary>
        /// Turns around the enemy when method is called.
        /// </summary>
        public void turnAround()
        {
            if (faceDirection == FaceDirection.Left)
            {
                faceDirection = FaceDirection.Right;
                Rotation = (float)Math.PI;
                flip = true;
            }
            else
            {
                faceDirection = FaceDirection.Left;
                Rotation = 0;
                flip = false;
            }

        }

        /// <summary>
        /// Checks whether or not the enemy should turn around depending on the location of its target.
        /// </summary>
        /// <param name="position"></param>
        public void checkToTurn(Vector2 position)
        {
            if (this.faceDirection == FaceDirection.Left && Position.X < position.X - 1500f)
                turnAround();
            else if ((this.faceDirection == FaceDirection.Right && Position.X > position.X + 1500f))
                turnAround();

        }
        
        
        
        public void Explode()
        {
            exploding = true;
            setAnimation(explodeAni);
            Velocity = Vector2.Zero;
            // NickSound
            if (!GameplayScreen.muted)
                explodeSound.Play();
            engineSoundInst.Stop();
        }

        /// <summary>
        /// Updates enemy position and its bullets and bomb onscreen. Method is overloaded in each
        /// subsequent derived enemy class.
        /// </summary>
        /// <param name="playersVelocity">Players velocity to determine location on screen</param>
        /// <param name="playersPosition">Players location on screen. Used to determine behaviour</param>
        public virtual void Update(Vector2 playersVelocity, Vector2 playersPosition, GameTime gameTime)
        {
            base.Position += (base.Velocity - playersVelocity);

            if (exploding && animate.Animation.IsFinished)
            {
                hasExploded = true;
            }

            if (!GameplayScreen.muted)
            {
                // NickSound
                engineSoundInst.Volume = 100 / Math.Abs(playersPosition.X - Position.X);
                if (engineSoundInst.Volume < 0.05f)
                    engineSoundInst.Volume = 0;
                else if (engineSoundInst.Volume > 10f)
                    engineSoundInst.Volume = 10.0f;
            }

            //foreach (Bullet bullet in bullets.ToArray())
            //{
                
            //    if (bullet.HasExceededDistance())                
            //        bullets.Remove(bullet);                
            //    else                
            //        bullet.Update(this.Velocity);                
            //}

            //foreach (Bomb bombN in bombs.ToArray())
            //{
            //    if (bombN.Position.Y > 3000f)           
            //        bombs.Remove(bombN);                
            //    else
            //        bombN.Update(this.Velocity);
            //}
        }



        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw in direction enemy is facing
            //SpriteEffects flip = base.faceDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //animate.Draw(gameTime, spriteBatch, base.Position, base.Rotation, flip);
            // draw in direction player is facing
            float preferredWidth = 1024f;//1280
            float preferredHeight = 768f;//720


            if (this.Position.X + Texture.Height > preferredWidth)
            {
                arrowPointer.Position = new Vector2(preferredWidth - 40f, Position.Y);
                arrowPointer.Rotation = 0;
                arrowPointer.Draw(spriteBatch);
            }
            else if (this.Position.X - (float)Texture.Height < 0)
            {
                arrowPointer.Position = new Vector2(40f, Position.Y);
                arrowPointer.Rotation = (float)Math.PI;
                arrowPointer.Draw(spriteBatch);
            }
            else if (this.Position.Y - Texture.Height < 0)
            {
                arrowPointer.Position = new Vector2(Position.X, 40f);
                arrowPointer.Rotation = (float)(1.5 * Math.PI);
                arrowPointer.Draw(spriteBatch);
            }
            else if (this.Position.Y > preferredHeight)
            {
                arrowPointer.Position = new Vector2(Position.X, preferredHeight - 40f);
                arrowPointer.Rotation = (float)(0.5 * Math.PI);
                arrowPointer.Draw(spriteBatch);
            }
            
            
            
            SpriteEffects flipSprite = flip == true ? SpriteEffects.FlipVertically : SpriteEffects.None;
            animate.Draw(gameTime, spriteBatch, Position, Rotation, flipSprite);






        }



    }
}
