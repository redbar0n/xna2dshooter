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
    class HeavyTank : EnemyTank
    {

        public HeavyTank(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping)
        {
            //bulletTexture = bulletImage;
            if (difficulty == Difficulty.Easy)
            {
                StartHP = 500;
                WorthScore = 1000;
                BulletDamage = 15;
            }
            else /// Difficulty is medium or Hard
            {
                StartHP = 2500;
                WorthScore = 5000;
                BulletDamage = 45;
            }
        }



    }
}
