using System;
using System.Collections.Generic;
using System.Text;

namespace Spitfire
{
    /// <summary>
    /// handles background, ground, enemies, scoring, win/lose cond., bonuses etc.
    /// </summary>
    /// <remarks></remarks>
    public class Level
    {
        private int music;
        private int frames;
        private int enemies;
        private int field;

        new public Vector2 Position
        {
            get { return position; }
            set
            {
                if (value.X > Position.X)
                    levelProgress += (value.X - Position.X);
                position = value;
            }
        }
        float levelProgress = 0;

        public Level(string position, string starVelocity)
        {
            throw new System.NotImplementedException();
        }

        public void Draw()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void LoadContent()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
