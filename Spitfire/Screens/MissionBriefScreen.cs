using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Spitfire
{
    class MissionBriefScreen : MenuScreen
    {
        string brief;
        string brief1;
        string brief2;
        int level;
        GameScreen gameplayScreen;
        Texture2D controlsTex;

        public MissionBriefScreen(int level, GameScreen gameplayScreen)
            : base("Background Story", new Vector2(300, 680))
        {
            this.level = level;
            this.gameplayScreen = gameplayScreen;

            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            brief1 = "MISSION 1:\n\n"
            + "Destroy as many\n"
            + "enemies as you can,\n"
            + "and defeat their commander!";

            brief2 = "MISSION 2:\n\n"
            + "Destroy as many\n"
            + "enemies as you can,\n"
            + "and destroy the\n"
            + "mysterious super weapon!";

            if (level == 1)
                brief = brief1;
            else if (level == 2)
                brief = brief2;
        }

        override public void LoadContent()
        {
            if (level == 2)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 1f; // magic constant
                MediaPlayer.Play(ScreenManager.Game.Content.Load<Song>("Sounds/474459_SOUNDDOGS__th"));
            }
            ContentManager content = ScreenManager.Game.Content;
            controlsTex = content.Load<Texture2D>("Menus/controls3");
        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();
            if (level != 3)
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gameplayScreen);
            else
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
            Color color = Color.Black;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            float scale = 1f;
            
            Vector2 position = new Vector2(330, 80);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 controlsPos = new Vector2((viewport.Width - controlsTex.Width) / 2 + 50, 540);
            Color texColor = Color.White;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, brief, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(controlsTex, controlsPos, texColor);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
