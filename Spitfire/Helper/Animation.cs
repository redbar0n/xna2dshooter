using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Frames are fetched from a horizontal spritemap texture, assuming the height of the texture equals the width of each frame.
    /// The number of frames in the animation are inferred from this.
    /// </remarks>
    public class Animation
    {
        Texture2D texture; // all frames arranged horizontally
        float frameTime; // duration of time to show each frame
        bool isLooping;
   
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }

        #region getters
        public Texture2D Texture
        {
            get { return texture; }
        }

        public float FrameTime
        {
            get { return frameTime; }
        }

        public bool IsLooping
        {
            get { return isLooping; }
        }
        #endregion


        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        /// <remarks> Assumes square frames. width = height.</remarks>
        public int FrameWidth
        {
            get { return texture.Height; }
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        /// <remarks> Assumes spritemap texture is only horizontally laid out. </remarks>
        public int FrameCount
        {
            get { return texture.Width / FrameWidth; }
        }


        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        /// <remarks> Assumes spritemap texture is only horizontally laid out. frame height = texture height. </remarks>
        public int FrameHeight
        {
            get { return texture.Height; }
        }


    }
}
