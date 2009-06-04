#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Spitfire
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        
        Texture2D texture;
        Texture2D controlsTex;
        Level level;
        GameplayScreen gameplayScreen;

        MenuEntry muteGameMenuEntry;

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(Level level, GameplayScreen gameplayScreen)
            : base("Paused", null)
        {
            this.level = level;
            this.gameplayScreen = gameplayScreen;

            
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f); // Stop the rumble feature

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            
            if (!GameplayScreen.muted)
            {
                muteGameMenuEntry = new MenuEntry("Mute all sound");
            }
            else
            {
                muteGameMenuEntry = new MenuEntry("Unmute all sound");
            }
            MenuEntry lvl1GameMenuEntry = new MenuEntry("Go to level 1");
            MenuEntry lvl2GameMenuEntry = new MenuEntry("Go to level 2");
            MenuEntry finalbossGameMenuEntry = new MenuEntry("Go to final boss");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            muteGameMenuEntry.Selected += MuteGameMenuEntrySelected;
            lvl1GameMenuEntry.Selected += lvl1GameMenuEntrySelected;
            lvl2GameMenuEntry.Selected += lvl2GameMenuEntrySelected;
            finalbossGameMenuEntry.Selected += GotoFinalBossMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(muteGameMenuEntry);
            MenuEntries.Add(lvl1GameMenuEntry);
            MenuEntries.Add(lvl2GameMenuEntry);
            MenuEntries.Add(finalbossGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        override public void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            texture = content.Load<Texture2D>("Menus/pausescreen");
            controlsTex = content.Load<Texture2D>("Menus/controls3");
        }

        #endregion

        #region Handle Input


        void MuteGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (!GameplayScreen.muted)
            {
                GameplayScreen.muted = true;
                gameplayScreen.muteSounds();
                muteGameMenuEntry.Text = "Unmute all sound";
            }
            else
            {
                GameplayScreen.muted = false;
                gameplayScreen.unMuteSounds();
                muteGameMenuEntry.Text = "Mute all sound";
            }
            

        }

        void GotoFinalBossMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            level.levelNumber = 2;
            level.setLevelProgress(24);
            ExitScreen();

        }

        void lvl1GameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            level.levelNumber = 1;
            ExitScreen();
        }

        void lvl2GameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            level.levelNumber = 2;
            ExitScreen();
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen("Menus/gamemenu_final"),
                                                           new MainMenuScreen());
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 texturePos = new Vector2(viewport.Width - texture.Width, viewport.Height - texture.Height) / 2;

            Vector2 controlsPos = new Vector2((viewport.Width - controlsTex.Width) / 2, 60);

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(texture, texturePos, color);
            spriteBatch.Draw(controlsTex, controlsPos, color);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        #endregion
    }
}
