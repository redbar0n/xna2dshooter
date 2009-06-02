using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    class MissionBriefScreen : MenuScreen
    {
        string brief;
        string brief1;
        string brief2;
        string gameover;
        int level;
        GameScreen gameplayScreen;

        public MissionBriefScreen(int level, GameScreen gameplayScreen)
            : base("Background Story", new Vector2(300, 680))
        {
            this.level = level;
            this.gameplayScreen = gameplayScreen;

            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            brief1 = "MISSION 1:\n"
            + "Destroy as many enemies as you can\n"
            + "before reaching the end of the level.";

            brief2 = "MISSION 2:\n"
            + "Destroy as many enemies as you can,\n"
            + "and destroy Hitler and Stalin's super weapon!";

            gameover = " Congratulations, you completed the game!\n"
            + " You should be proud of yourself!\n\n\n\n\n This game was made by:\n Brian Lam (Game design),\n Nicole McMahon (Artwork),\n Nick Maunder (Code),\n Magne Gasland (Code).\n\n It was made the first semester of 2009\n at QUT, Brisbane in Peta Wyeths unit:\n INB281 Adv. game design";

            if (level == 1)
                brief = brief1;
            else if (level == 2)
                brief = brief2;
            else if (level == 3)
                brief = gameover;
        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (level != 3)
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gameplayScreen);
            else
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/gamemenu"), new MainMenuScreen());
        }


        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this game?";
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Color color = Color.Black;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            float scale = 1f;

            
            Vector2 position = new Vector2(250, 300);
            if (level == 3)
                position = new Vector2(250, 100);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, brief, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
            if (level != 3)
                base.Draw(gameTime);
        }

    }
}
