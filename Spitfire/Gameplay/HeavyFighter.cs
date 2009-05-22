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
    class HeavyFighter:EnemyPlane
    {

       public HeavyFighter(Level level,Difficulty difficulty, String spriteSet, bool looping)
            : base (level,difficulty, spriteSet,looping)              
        {
            //bulletTexture = bulletImage;
            if (difficulty == Difficulty.Easy) {
                StartHP = 100;
                WorthScore = 500;
                BulletDamage = 5;
            }
            else if (difficulty == Difficulty.Medium)
            {
                StartHP = 200;
                WorthScore = 1000;
                BulletDamage = 10;
            }
            else {
                StartHP = 500;
                WorthScore = 2500;
                BulletDamage = 15;
            }
            

        }




    }
}
