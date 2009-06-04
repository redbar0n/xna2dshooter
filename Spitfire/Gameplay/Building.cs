using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Spitfire
{

    public class Building: Sprite //Remember this Nick, make the class public
    {

        /// <summary>
        /// The associated level
        /// </summary>
        /// <remarks>
        /// For loading content, and determining enemy behaviour based on level status etc.
        /// </remarks>
        public Level Level
        {
            get { return level; }
            set { level = value; }
        }
        Level level;




        public Building(Level level, String spriteSet) {
            this.level = level;
            spriteSet = "Sprites/Backgrounds/City/Buildings/" + spriteSet;
            this.Texture = Level.Content.Load<Texture2D>(spriteSet);
        
        }

        public void Update(Vector2 velocity) {

            this.Position -= velocity;
        }


    }
}
