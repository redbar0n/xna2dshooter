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
    class EnergyBomb: Bomb
    {

        public static Vector2 Origin = new Vector2(5f, 5f);


        // TODO - possibly questionable constructor
        public EnergyBomb(float rotation, Vector2 bombersPosition, Vector2 bombersVelocity, FaceDirection direction)
            : base(rotation,bombersPosition,bombersVelocity,direction)
        {
            base.Rotation = rotation;
            base.Position = bombersPosition + new Vector2(0, 10);
            base.Velocity = new Vector2(2f, 2f);
            this.faceDirection = direction;
        }

        // TODO - Complete other update, rotation and other methods



    }
}
