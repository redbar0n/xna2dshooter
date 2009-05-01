using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    public class Sprite
    {
        private Vector2 position;
        private Vector2 velocity;
        private Texture2D texture;

        /// <summary>
        /// Bounding rectangle and in-game size of texture. Should be used to get height and width.
        /// </summary>
        /// <remarks> Should only be set indirectly through Texture and Scale.</remarks>
        /// 
        public Rectangle Size
        {
            get { return size; }
            set { size = value; }
            // should only be set indirectly through texture and scale
        }
        private Rectangle size;
        
        private float scale = 1.0f;

        /// <summary>
        /// Determines direction of movement.
        /// future: could faceDirection be determined by looking at X velocity?
        /// </summary>
        protected enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }
        protected FaceDirection faceDirection = FaceDirection.Left; // Left because of enemies

        /// <summary>
        /// // future: to be used for rotating sprite
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        private float rotation; 

        // future: add source rectangle to extract from sprite sheet. remember to change draw and setter methods accordingly.

        // NB: is this really neeeded?
        public Sprite()
        {
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Rotation = 0.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        # region == GETTERS AND SETTERS ==
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                size = new Rectangle(0, 0, (int) (texture.Width * Scale), (int) (texture.Height * Scale));
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                size = new Rectangle(0, 0, (int)(texture.Width * scale), (int)(texture.Height * scale));
            }
        }
        #endregion

    }
}
