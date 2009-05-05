using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        private Animation normalAni;  // TODO: PRIORITY make enemy use its own sprite texture and not normalAni
        private Animation explodeAni;
        private AnimationPlayer animate;


        /// <summary>
        /// Tells if the explode animation is finished.
        /// </summary>
        public bool HasExploded
        {
            get { return hasExploded; }
            set { hasExploded = value; }
        }
        private bool hasExploded = false;
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

        // future: add bullets

        /// <summary>
        /// Creates a new enemy.
        /// </summary>
        /// <param name="position">Position on screen</param>
        /// <param name="velocity">Velocity</param>
        /// <param name="scale">Scale size of sprite up or down.</param>
        /// <param name="maxHP">The starting hp of unit.</param>
        /// <param name="crashDamage">The damage the unit will do if crash or explode into other unit.</param>
        public Enemy(Level level, Type type, String spriteSet)
        {
            this.level = level;
            this.type = type;
            animate = new AnimationPlayer();
            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet
        /// future: and sounds.
        /// </summary>
        public void LoadContent(String spriteSet)
        {
            // Load texture and animation(s).
            spriteSet = "Sprites/Enemies/" + spriteSet;

            Texture = Level.Content.Load<Texture2D>(spriteSet);
            explodeAni = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Explode"), 1f, false);

            animate.PlayAnimation(normalAni);
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
                    Explode();
                }
            }    
        }

        public void ShotDown()
        {
            //future: animate.PlayAnimation(shotDownAni);
        }

        public void Explode()
        {
            exploding = true;
            animate.PlayAnimation(explodeAni);
        }

        public void Update()
        {
            base.Position += base.Velocity;

            if (exploding && animate.FrameIndex == (animate.Animation.FrameCount - 1))
                hasExploded = true;

        }

        public void Update(Vector2 playersVelocity)
        {
            base.Position += (base.Velocity - playersVelocity);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw in direction enemy is facing
            SpriteEffects flip = base.faceDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            animate.Draw(gameTime, spriteBatch, Position, base.Rotation, flip);
        }
    }
}
