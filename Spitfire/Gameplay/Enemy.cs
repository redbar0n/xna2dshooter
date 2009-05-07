using System;
using System.Collections.Generic;
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

        // Sounds

        private SoundEffect hitSound;
        private SoundEffect explodeSound;
        private SoundEffect engineSound;
        private SoundEffectInstance engineSoundInst;


        // future: add bullets

        /// <summary>
        /// Creates a new enemy.
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
            animate = new AnimationPlayer();
            LoadContent(spriteSet, looping);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet
        /// future: and sounds.
        /// </summary>
        public void LoadContent(String spriteSet, bool looping)
        {
            // Load animation(s).
            spriteSet = "Sprites/Enemies/" + spriteSet;

            normalAni = new Animation(Level.Content.Load<Texture2D>(spriteSet), 1f, looping);
            explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/expspritemap"), 1f, false);

            setAnimation(normalAni);

            // Load sounds

            if (spriteSet.Equals("Sprites/Enemies/mig"))
            {
                // NickSound
                //engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Lightfighter/002"); 
                //engineSoundInst = engineSound.Play(0.2f, 0.0f, 0.0f, true);
            }
            else if (spriteSet.Equals("Sprites/Enemies/heavyfighter"))
            {
                // NickSound
                //engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Heavyfighter/001");
                //engineSoundInst = engineSound.Play(0.3f, 0.0f, 0.0f, true);
            }
            else if (spriteSet.Equals("Sprites/Enemies/lighttankspritemapfinal") || spriteSet.Equals("Sprites/Enemies/finalheavytanksprite"))
            {
                // NickSound
                //engineSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Tank/304678_SOUNDDOGS_TA");
                //engineSoundInst = engineSound.Play(0.2f, 0.0f, 0.0f, true);
            }
            else if (spriteSet.Equals("Sprites/Enemies/zeppelin2sized"))
            {
                explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/zeppelin2sized"), 1f, false);
                ///TODO This animation makes the program crash. I have replaced it with the above statement
                //explodeAni = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemies/zepplinexplspritemap"), 1f, false);
            }

            // NickSound
            //hitSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/BulletHit");
            //explodeSound = Level.Content.Load<SoundEffect>("Sounds/Enemy/Explode");

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
                if (type == Type.ShotDown)
                {
                    ShotDown();
                }
                else
                {
                    Explode(); // will pass the exploding down to animateExplosion, which will pass it up to level which finally removes enemy
                }
            }
            // NickSound
            //hitSound.Play();
        }

        public void ShotDown()
        {
            //future: animate.PlayAnimation(shotDownAni);
        }

        public void Explode()
        {
            exploding = true;
            setAnimation(explodeAni);
            Velocity = Vector2.Zero;
            // NickSound
            //explodeSound.Play();
            //engineSoundInst.Stop();
        }

        public void Update()
        {
            base.Position += base.Velocity;

            if (exploding && animate.Animation.IsFinished)
            {
                hasExploded = true;
            }

        }

        public void Update(Vector2 playersVelocity)
        {
            base.Position += (base.Velocity - playersVelocity);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw in direction enemy is facing
            SpriteEffects flip = base.faceDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            animate.Draw(gameTime, spriteBatch, base.Position, base.Rotation, flip);
        }
    }
}
