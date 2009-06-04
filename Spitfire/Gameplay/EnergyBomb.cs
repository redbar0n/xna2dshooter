using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
   /// <summary>
   /// The bomb used by the Zepplin boss
   /// </summary>
    public class EnergyBomb: Bomb
    {

        public new static Vector2 Origin = new Vector2(5f, 5f);
        public Vector2 initialVelocity = new Vector2(5f, 2f);





        // TODO - possibly questionable constructor
        public EnergyBomb(float rotation, Vector2 bombersPosition, Vector2 targetPosition, FaceDirection direction)
            : base(rotation, bombersPosition, targetPosition, direction)
        {
            base.Rotation = rotation;
            base.Position = bombersPosition + new Vector2(0, 10);

            if (targetPosition.X < bombersPosition.X)
                this.faceDirection = FaceDirection.Left;
            else
                this.faceDirection = FaceDirection.Right;
            base.Velocity = new Vector2(initialVelocity.X * (float)faceDirection, initialVelocity.Y);

            DistanceTravelled = 0;
        }

        // TODO - Complete other update, rotation and other methods

        private void determineVelocity(Vector2 targetsPosition)
        {
            if (targetsPosition.Y < this.Position.Y)
                base.Velocity = new Vector2(Velocity.X, initialVelocity.Y * -1);
            else if (targetsPosition.Y > this.Position.Y)
            {
                base.Velocity = new Vector2(Velocity.X, initialVelocity.Y);
            }
        }


        public void Update(Vector2 playerVelocity, Vector2 targetsPosition)
        {


            determineVelocity(targetsPosition);
            base.Update(playerVelocity);
        }



    }
}
