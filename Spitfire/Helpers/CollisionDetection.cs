using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Spitfire
{
    /// <summary>
    /// Checks if pixels in two different sprites overlap
    /// </summary>
    struct CollisionDetection
    {
        public static bool Collision(Sprite a, Sprite b)
        {
            if (a.Size.Intersects(b.Size))
            {
                Rectangle intersection = Intersection(a.Size, b.Size);
                Rectangle aIntersection = Normalize(a.Size, intersection);
                Rectangle bIntersection = Normalize(b.Size, intersection);

                int pixelCount = aIntersection.Width * aIntersection.Height;
                uint[] aPixels = new uint[pixelCount];
                uint[] bPixels = new uint[pixelCount];

                a.Texture.GetData<uint>(0, aIntersection, aPixels, 0, pixelCount);
                b.Texture.GetData<uint>(0, bIntersection, bPixels, 0, pixelCount);

                for (int i = 0; i < pixelCount; ++i)
                {
                    if (((aPixels[i] & 0xff000000) > 0)
                      && ((bPixels[i] & 0xff000000) > 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// To get the intersection rectangle.
        /// </summary>
        /// <param name="rectangle1"></param>
        /// <param name="rectangle2"></param>
        /// <returns></returns>
        public static Rectangle Intersection(Rectangle rectangle1, Rectangle rectangle2)
        {
            int x1 = Math.Max(rectangle1.Left, rectangle2.Left);
            int y1 = Math.Max(rectangle1.Top, rectangle2.Top);
            int x2 = Math.Min(rectangle1.Right, rectangle2.Right);
            int y2 = Math.Min(rectangle1.Bottom, rectangle2.Bottom);

            if ((x2 >= x1) && (y2 >= y1))
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }

        /// <summary>
        /// Translate coordinates from the intersection rectangle into the bounding box rectangle coordinates.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Rectangle Normalize(Rectangle reference, Rectangle rectangle)
        {
            return new Rectangle(
              rectangle.X - reference.X,
              rectangle.Y - reference.Y,
              rectangle.Width,
              rectangle.Height);
        }
    }
}
