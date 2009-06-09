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
    class LightTank: EnemyTank
    {
        public LightTank(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping) {
                //bulletTexture = bulletImage;
                if (difficulty == Difficulty.Easy)
                {
                    StartHP = 100;
                    WorthScore = 250;
                    BulletDamage = 6;
                }
                else /// Difficulty is medium or Hard
                {
                    StartHP = 500;
                    WorthScore = 1250;
                    BulletDamage = 30;
                }
        
        }
    }
}
