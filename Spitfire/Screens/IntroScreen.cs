using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Spitfire
{
    class IntroScreen : MenuScreen
    {
        // TODO: make proper implementation of IntroScreen
        public static int storyScreenNr = 2;

        public IntroScreen()
            : base("Background Story")
        {
            MenuEntry continueMenuEntry = new MenuEntry("Press space to continue...");
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

        }


        void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (storyScreenNr <= 4)
            {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(("Menus/story-screen" + storyScreenNr)), new IntroScreen());
                //ScreenManager.AddScreen(new BackgroundScreen(("Menus/story-screen" + storyScreenNr)), e.PlayerIndex);
                //ScreenManager.AddScreen(new IntroScreen(), e.PlayerIndex);
                storyScreenNr++;
            }
            else
            {
                storyScreenNr = 2;
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
            }

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


    }
}
