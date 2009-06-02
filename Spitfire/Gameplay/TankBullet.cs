using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    class TankBullet: Bullet
    {

        private Vector2 initialVelocity = new Vector2(15f, 15f);
        

        private float accelerant;
        
        public TankBullet(float shootersRotation, Vector2 initialPosition, FaceDirection direction)
            : base(shootersRotation, initialPosition, direction) 
        
        {
            accelerant = -1.0f;

        }


        public override void Update(Vector2 playerVelocity)
        {
            determineVelocity();            
            base.Position += (Velocity - playerVelocity);
            

        }

        private void determineVelocity()
        {
            changeAccelerant();
            this.Velocity = new Vector2(initialVelocity.X * (float)faceDirection, initialVelocity.Y * accelerant);

        }

        private void changeAccelerant() {
            accelerant += 0.025f;
            if (accelerant > 5f){
                accelerant = 5;
            }
        }


    }
}
