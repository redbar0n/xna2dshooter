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
        private int maxHP;
        private int currentHP;
        private int explodeDamage;
        private Curve path; // in case it is a flying enemy
        private Animation explodeAni;
        private AnimationPlayer animate;
        
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
        /// Creates a new enemy at specified position.
        /// </summary>
        /// <param name="position">Position on screen</param>
        /// <param name="velocity">Velocity</param>
        /// <param name="scale">Scale size of sprite up or down.</param>
        /// <param name="maxHP">The starting hp of unit.</param>
        /// <param name="crashDamage">The damage the unit will do if crash or explode into other unit.</param>
        public Enemy(Type type, Vector2 position, Vector2 velocity, float scale, int startHP, int explodeDamage)
        {
            this.type = type;
            base.Position = position;
            base.Velocity = velocity;
            base.Scale = scale;
            maxHP = startHP;
            currentHP = startHP;
            this.explodeDamage = explodeDamage;
        }


        /// <summary>
        /// Loads a particular enemy sprite sheet
        /// future: and sounds.
        /// </summary>
        public void LoadContent(String spriteSet, ContentManager content)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            base.Texture = content.Load<Texture2D>(spriteSet);
            explodeAni = new Animation(content.Load<Texture2D>(spriteSet + "Explode"), 0.1f, false);
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
            animate.PlayAnimation(explodeAni);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw in direction enemy is facing
            SpriteEffects flip = base.faceDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            animate.Draw(gameTime, spriteBatch, Position, Rotation, flip);
        }
    }
}
