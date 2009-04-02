using System;
using System.Collections.Generic;
using System.Text;

namespace Spitfire
{
    public class EnemyPlane : Sprite
    {
        private int maxHP;
        private int currentHP;
        private int crashDamage;
        private Curve path;

        public void Die()
        {
            throw new System.NotImplementedException();
        }
    }
}
