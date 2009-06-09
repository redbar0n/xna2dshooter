using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Spitfire
{
    public class HUD
    {
        // future: make HUD a static class
        // future: optimise hud according to Platformer1. store central hud location in vector. apply shadow to fonts.

        /// <summary>
        /// The players current score.
        /// </summary>
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        private int score = 0;      // future: score is stored here for now. may want to put in own class later, with highscore.
        
        private Player player;

        private Texture2D healthBar;
        SpriteFont spriteFont; // gui text font

        private static Vector2 vBombsTextLoc = new Vector2(30, 30);
        private static Vector2 vScoreTextLoc;

        private string immortal = "Immortal";
        public bool drawImmortal = false;

        public HUD(Player player)
        {
            this.player = player;
        }

        public void setPlayer(Player player)
        {
            this.player = player;
        }

        public void LoadContent(ContentManager content)
        {
            healthBar = content.Load<Texture2D>("HUD/healthBar") as Texture2D;
            spriteFont = content.Load<SpriteFont>(@"HUD/Pericles");
        }

        public void Draw(SpriteBatch spriteBatch, int windowWidth)
        {
            //Draw the negative space for the health bar
            spriteBatch.Draw(healthBar, new Rectangle(windowWidth / 2 - healthBar.Width / 2,
                 30, healthBar.Width, 22), new Rectangle(0, 45, healthBar.Width, 44), Color.Gray);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle(windowWidth / 2 - healthBar.Width / 2,
                 30, (int)(healthBar.Width * ((double) player.CurrentHP / 100)), 22),
                 new Rectangle(0, 45, healthBar.Width, 44), Color.Red);

            //Draw the box around the health bar
            spriteBatch.Draw(healthBar, new Rectangle(windowWidth / 2 - healthBar.Width / 2,
                  30, healthBar.Width, 22), new Rectangle(0, 0, healthBar.Width, 44), Color.White);

            spriteBatch.DrawString(spriteFont, "Bombs: " + player.BombCount.ToString(), vBombsTextLoc, Color.White);

             vScoreTextLoc = new Vector2(windowWidth-180, 30);

            spriteBatch.DrawString(spriteFont, "Score: " + score.ToString(), vScoreTextLoc, Color.White);

            if (drawImmortal)
                spriteBatch.DrawString(spriteFont, immortal, new Vector2(windowWidth / 2 - 20, 60), Color.White);
        }
    }
}
