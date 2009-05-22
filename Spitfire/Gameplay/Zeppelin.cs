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
    class Zeppelin : Enemy
    {

        public Zeppelin(Level level, Difficulty difficulty, String spriteSet, bool looping)
            : base(level, difficulty, spriteSet, looping) { 
        
        
            //TODO Fill in details about statistics
            // -- HP and worth score look a little high. Might want to decrease them eventually

                //bulletTexture = bulletImage;
                if (difficulty == Difficulty.Easy)
                {
                    StartHP = 1000;
                    WorthScore = 5000;
                    BulletDamage = 5;
                }
                else if (difficulty == Difficulty.Medium)
                {
                    StartHP = 2000;
                    WorthScore = 10000;
                    BulletDamage = 10;
                }
                else
                {
                    StartHP = 5000;
                    WorthScore = 25000;
                    BulletDamage = 15;
                }
        
        } 




        //TODO add other overloading methods including update, shoot? etc



    }
}
