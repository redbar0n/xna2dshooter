using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire {
    
    
    /// <summary>
    /// The base class for pickups like extra bombs, extra HP in the game.
    /// </summary>
    class Pickup:Sprite {


       /// <summary>
       /// Determines if the pickup has collided with the ground or with player
       /// </summary>           
        public bool HasCollided
        {
            get { return hasCollided; }
            set
            {
                hasCollided = value;
            }
        }        
        private bool hasCollided;
    

        
        /// <summary>
        /// The type of effect that pickup has
        /// </summary>
        public enum Effect {
            HP, BombIncrease, BulletSpeed
        }
        private Effect effect;

        /// <summary>
        /// Creates a pickup object. NOTE, - effect parametre may be unncessary
        /// </summary>
        /// <param name="initialPosition"></param>
        /// <param name="pickupEffect"></param>
        public Pickup(Vector2 initialPosition, Effect pickupEffect) {
            effect = pickupEffect;
            base.Position = initialPosition;
            hasCollided = false;
            base.Velocity = new Vector2(0f, 1f);
            
        }

        public void Update(Vector2 velocity){
            if (!hasCollided) {
                base.Position += (base.Velocity - velocity);
            }
        
        }

        

    }
}
