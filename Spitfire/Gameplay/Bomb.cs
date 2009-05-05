using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    public class Bomb : Sprite
    {

        public static Vector2 Origin = new Vector2(5f, 5f);
        //private bool isRight = false;

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
