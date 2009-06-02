using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    class LevelSummaryScreen : MenuScreen
    {
        int level;
        GameplayScreen gameplayScreen;

        public LevelSummaryScreen(int level, GameplayScreen gameplayScreen)
            : base("Background Story", new Vector2(300, 680))
        {
            this.level = level;
            this.gameplayScreen = gameplayScreen;

            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);
        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            level++;
            if (level <= 2)
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/level_summary"), new MissionBriefScreen(level, gameplayScreen));
            else
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/level_summary"), new MissionBriefScreen(level, gameplayScreen));
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

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "SCORE: " + gameplayScreen.Hud.Score, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
