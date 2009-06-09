using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Spitfire
{
    class CreditsScreen : MenuScreen
    {
        string gameover;

        public CreditsScreen()
            : base("Background Story", new Vector2(300, 680))
        {

            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            gameover = "CREDITS\n\nGame Design:\nBrian Lam\n\nArtwork:\nNicole McMahon\n\nCode:\nNick Maunder\nMagne Gasland\n\n\nContext:\nIt was made the first semester of 2009\nat QUT, Brisbane in Peta Wyeths unit:\nINB281 Adv. game design";
        }

        override public void LoadContent()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f; // magic constant
            MediaPlayer.Play(ScreenManager.Game.Content.Load<Song>("Sounds/Vera Lynn - Abide With Me"));
        }

        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/gamemenu_final"), new MainMenuScreen());
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
            Color color = Color.White;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            float scale = 1f;

            
            Vector2 position = new Vector2(50, 50);


            spriteBatch.Begin();
            spriteBatch.DrawString(font, gameover, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

    }
}
