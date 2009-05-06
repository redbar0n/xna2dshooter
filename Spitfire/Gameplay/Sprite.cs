using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    public class Sprite
    {
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 position;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;
        
        /// <summary>
        /// The sprites texture. Player and enemy shouldn't use this texture, but rather the Animation class.
        /// </summary>
        public virtual Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        private Texture2D texture;

        /// <summary>
        /// Bounding rectangle and in-game size of texture. Should be used to get height and width.
        /// Bounds the texture in the game world.
        /// </summary>
        public virtual Rectangle Size
        {
            get {
                return new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), (int)(texture.Width * scale), (int)(texture.Height * scale)); ;
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        private float scale = 1.0f;

        /// <summary>
        /// Determines direction of movement.
        /// future: could faceDirection be determined by looking at X velocity?
        /// </summary>
        public enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }
        public FaceDirection faceDirection = FaceDirection.Left; // Left because of enemies

        /// <summary>
        /// // future: to be used for rotating sprite
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        private float rotation; 

        // NB: is this really neeeded?
        public Sprite()
        {
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Rotation = 0.0f;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
