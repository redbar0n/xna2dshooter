using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    class AnimationPlayer
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        public float Time
        {
            get { return time; }
        }
        private float time;

        /// <summary>
        /// If animation frame is iterating towards the first direction its supposed to.
        /// </summary>
        bool goingFirst;

        bool flipSprite;

        /// <summary>
        /// Gets a texture origin at the center of each frame.
        /// Used in rotation.
        /// Texture will be drawn around this origin.
        /// </summary>
        public Vector2 Origin
        {
            get {
                //return Vector2.Zero;
                return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight / 2.0f);
            }
        }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            goingFirst = true;
            flipSprite = false;
            this.animation = animation;
            this.frameIndex = 0;
            if (animation.IsGoingRightToLeft)
                this.frameIndex = animation.FrameCount - 1;
            this.time = 0.0f;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        /// <remarks>
        /// Can be used to animate, or to rotate a single sprite.
        /// </remarks>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation, SpriteEffects spriteEffects)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (Animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % Animation.FrameCount;
                }
                else if (Animation.IsGoingLeftToRight)
                {
                    if (goingFirst)
                    {
                        frameIndex++;
                        if (frameIndex == Animation.FrameCount - 1)
                        {
                            goingFirst = false;
                        }
                    }
                    else
                    {
                        if (frameIndex == 0)
                        {
                            Animation.IsFinished = true;
                        }
                        flipSprite = true;
                        frameIndex = Math.Max(--frameIndex, 0);
                    }
                }
                else if (Animation.IsGoingRightToLeft)
                {
                    if (goingFirst)
                    {
                        if (frameIndex == 0)
                        {
                            goingFirst = false;
                        }
                        frameIndex = Math.Max(--frameIndex, 0);
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, animation.FrameCount - 1);
                        flipSprite = true;
                        if (frameIndex == Animation.FrameCount - 1)
                        {
                            Animation.IsFinished = true;
                        }
                    }
                }
                else
                {
                    if (frameIndex == (animation.FrameCount - 1))
                    {
                        animation.IsFinished = true;
                    }

                    frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                }
            }

            if (flipSprite)
                spriteEffects = SpriteEffects.FlipVertically;

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, rotation, Origin, Animation.Scale, spriteEffects, 0.0f);
        }
    }
}