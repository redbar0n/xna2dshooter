using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
    public class Bomb : Sprite
    {

        public static Vector2 Origin = new Vector2(5f, 5f);
        //private bool isRight = false;


        public static SoundEffect BombSound
        {
            get { return Bomb.bombSound; }
            set { Bomb.bombSound = value; }
        }
        private static SoundEffect bombSound;
        
        public static SoundEffect ExplosionSound
        {
            get { return Bomb.explosionSound; }
            set { Bomb.explosionSound = value; }
        }
        private static SoundEffect explosionSound;

        public SoundEffectInstance BombSoundInst
        {
            get { return bombSoundInst; }
            set { bombSoundInst = value; }
        }
        private SoundEffectInstance bombSoundInst;   



        public Bomb(float rotation, Vector2 bombersPosition, Vector2 bombersVelocity, FaceDirection direction)
        {
            base.Rotation = rotation;
            base.Position = bombersPosition + new Vector2(0, 10);
            base.Velocity = new Vector2(bombersVelocity.X / 2, 5f);
            this.faceDirection = direction;
        }

        public void Update(Vector2 playerVelocity)
        {
            base.Position += (base.Velocity - playerVelocity);
            rotate();


        }

        public void PlayDropSound()
        {
            //NickSound
            //bombSoundInst = bombSound.Play(0.5f, 0.0f, 0.0f, false);
        }

        public void PlayExplosionSound()
        {
            explosionSound.Play();
        }


        public void rotate()
        {
            if (this.faceDirection == FaceDirection.Right)
                Rotation += 0.02f;
            else
                Rotation -= 0.02f;
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects flip = base.faceDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            {
                spriteBatch.Draw(base.Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, flip, 0);
            }
        }
    }
}
