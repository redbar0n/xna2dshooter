using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spitfire
{
    class IntroScreen : MenuScreen
    {
        // TODO: make proper implementation of IntroScreen
        public static int storyScreenNr = 2;
        int pagecounter = 0;
        string currentpage;
        string[] pages = new string[4];


        public IntroScreen()
            : base("Background Story", new Vector2(300, 680))
        {
            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            pages[0] =
                " These are desperate times soldier, our forces are few \n and thin, "
            + "spread around the borders of England. Ever \n since the soviets and nazis "
            + "joined forces in the 40's,\n nothing has been going right; it's a miracle how "
            + "we\n even survived for two decades. To say the war has not\n been going well for "
            + "us would be a severe understatement.\n Our men are demoralised as the allied "
            + "nations\n fall one by one, and the recent rumours do not help\n either.";

            pages[1] =
                " Supposedly Stalin and Hitler are developing a super\n weapon that would be capable "
            + "of destroying England\n in hours. Whether this information is real or not, it\n doesn't "
            + "matter, we will not sit idly by after hearing\n this. Your main objective is to confirm "
            + "whether or not\n this weapon exists, and if so, to destroy it.\n England needs this victory!";

            pages[2] =
              " However, a more immediate threat exists. Reports\n from a nearby city indicate our "
            + "forces have been overrun,\n and that the nazis have begun setting up base there. \n "
            + "Their nazi commander seems to be overlooking the battlefield \n "
            + "from an enormous aircraft, "
            + "giving them a great tactical\n advantage. It's only "
            + "a matter of time until they advance\n further. We must strike now, before their "
            + "preparations\n are finished, and take out their commander. Your\n mission will not be easy; you must fly out in the "
            + "field\n and eliminate all occupying forces, by yourself, until\n reinforcements arrive.";


            pages[3] =
                "Good Luck";

            currentpage = pages[pagecounter];
        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            if (pagecounter < 3)
            {
                pagecounter++;
                currentpage = pages[pagecounter];
            }
            else
            {
                BackgroundScreen briefScreen = new BackgroundScreen(("Menus/missionbrief_final"));
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, briefScreen, new MissionBriefScreen(1, new GameplayScreen()));
            }

            /*
            if (storyScreenNr <= 1)
            {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(("Menus/story_screen_final")), new IntroScreen());
                //ScreenManager.AddScreen(new BackgroundScreen(("Menus/story-screen" + storyScreenNr)), e.PlayerIndex);
                //ScreenManager.AddScreen(new IntroScreen(), e.PlayerIndex);
                storyScreenNr++;
            }
            else
            {
                //storyScreenNr = 2;
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
            }
             */
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
            Vector2 position = new Vector2(190, 300);

            if (currentpage == pages[3])
            {
                scale = 2.0f;
                position = new Vector2(400, 400);
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, currentpage, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
