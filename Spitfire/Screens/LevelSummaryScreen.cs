using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Spitfire
{
    class LevelSummaryScreen : MenuScreen
    {
        int levelNr;
        GameplayScreen gameplayScreen;
        string summary;

        public LevelSummaryScreen(int levelNr, GameplayScreen gameplayScreen, Level level)
            : base("Background Story", new Vector2(300, 680))
        {
            this.levelNr = levelNr;
            this.gameplayScreen = gameplayScreen;

            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            if (levelNr == 2)
            {
                summary = "Congratulations! With the shrapnel gathered from the\n"
                + "wreckage of Hitler and Stalin's super weapon, our engineers\n"
                + "should be able to reconstruct it in no time. With it on our side,\n"
                + "we will be able to push the fight back to the Nazis and\n"
                + "Soviets. After you single handedly rampaged your way through\n"
                + "their attack force, we should easily be able to defeat the rest\n"
                + "of their army in the following months.\n\n"
                
                + "For now, our young pilot returns a hero. The trip may be long,\n"
                + "but the promise of a warm cup of tea waiting back at home\n"
                + "makes the hours of flight melt away...";

            }
            else
            {
                summary = "After your courageous victory over their commander in\n"
                + "the Great Zeppelin, there might be a glimmer of hope for\n"
                + "us all. But don't rest just yet, your skills will be needed\n"
                + "swiftly, as London has just been attacked! Reports say that\n"
                + "Nazi and Soviet forces have invaded the streets, and they are\n"
                + "accompanied by an enormous device that is laying waste to the\n"
                + "city! This must be their legendary super weapon! We've repaired\n"
                + "your plane and restocked your ammo, so you can once more fly\n"
                + "on a one man mission to save mother Britain.\n"
                + "For Queen and country!";
            }
        }

        override public void LoadContent()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f; // magic constant
            MediaPlayer.Play(ScreenManager.Game.Content.Load<Song>("Sounds/732695_SOUNDDOGS__ma"));
        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();
            levelNr++;
            if (levelNr == 2)
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/missionbrief_final_2"), new MissionBriefScreen(levelNr, gameplayScreen));
            else if (levelNr == 3)
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen("Menus/gamemenu_final"), new CreditsScreen());
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
            float scale = 0.8f;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 position = new Vector2(viewport.Width / 2 - 50, 500);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, summary, new Vector2(160, 50), color, 0, origin, scale, SpriteEffects.None, 0); 
            spriteBatch.DrawString(font, "SCORE", position, color, 0,
                                   origin, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, gameplayScreen.Hud.Score.ToString(), new Vector2(viewport.Width/2 - 100, 500), color, 0,
                       Vector2.Zero, 2.5f, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
