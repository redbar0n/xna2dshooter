using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Spitfire
{
    class Mig : Enemy
    {


        public Mig(Level level,Difficulty difficulty, String spriteSet, bool looping)
            : base (level,difficulty, spriteSet,looping)              
        {
            //bulletTexture = bulletImage;
            if (difficulty == Difficulty.Easy) {
                StartHP = 25;
                WorthScore = 50;
                BulletDamage = 5;
            }
            else if (difficulty == Difficulty.Medium)
            {
                StartHP = 50;
                WorthScore = 100;
                BulletDamage = 10;
            }
            else {
                StartHP = 125;
                WorthScore = 100;
                BulletDamage = 15;
            }
            

        }




    }
}
