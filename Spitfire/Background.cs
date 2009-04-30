using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    /// <summary>
    /// Background class handling the frames composing the background, their velocity etc.
    /// </summary>
    /// <remarks>
    /// future: Level class to hold Background and Ground class
    /// </remarks>
    public class Background
    {
        private ArrayList backgrounds;

        /// <summary>
        /// Decides if backgrounds go left or right.
        /// </summary>
        private Vector2 direction; // future: might be removed when using plane velocity.

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;

        /// <summary>
        /// How much background should be scaled. Redundant if background texture images are scaled.
        /// </summary>
        private float scale = 1.25f; // NB: may cause error since scale is in sprite as well

        public Background()
        {
            backgrounds = new ArrayList();
            direction = new Vector2(-1, 0);
        }

        // future: Optimize background loading to remove slight lag. Load smaller backgrounds.
        public void LoadContent(ContentManager content, String backgroundName, int nrOfBackgrounds)
        {
            float xPos = 0; // x-pos to insert background
            for (int i = 0; i < nrOfBackgrounds; i++)
            {
                Sprite frame = new Sprite();
                //background.Texture = content.Load<Texture2D>(backgroundName + nrOfBackgrounds);
                frame.Texture = content.Load<Texture2D>(backgroundName + "1");
                frame.Scale = scale;
                frame.Position = new Vector2(xPos, 0);
                frame.Velocity = velocity;
                backgrounds.Add(frame);
                xPos += frame.Size.Width;
            }
        }

        public void Update(GameTime gameTime)
        {
            Object[] bg = backgrounds.ToArray(); // background arraylist as an array
            Sprite lastBackground = (Sprite) bg[bg.Length - 1];
            for (int i = 0; i < bg.Length; i++)
			{
                Sprite frame = (Sprite)backgrounds[i];

                if (frame.Position.X < -frame.Size.Width)
                {
			        if (i == 0)
                    {
                        frame.Position = new Vector2 (lastBackground.Position.X + lastBackground.Size.Width, lastBackground.Position.Y);
                    } else
                    {
                        Sprite prevFrame = (Sprite)backgrounds[i - 1];
                        frame.Position = new Vector2(prevFrame.Position.X + prevFrame.Size.Width,
                                                                prevFrame.Position.Y);
                    }
                }

                // TODO: Background doesn't go up or down even though velocity changes in Y direction.
                frame.Position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite background in backgrounds)
            {
                background.Draw(spriteBatch);
            }
        }
    }
}
