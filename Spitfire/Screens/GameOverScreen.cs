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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Spitfire
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class GameOverScreen : MenuScreen
    {
        
        Texture2D texture;
        HUD hud;
        Level level;

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverScreen(Level level, HUD hud)
            : base("Game Over", null)
        {
            this.level = level;
            this.hud = hud;


            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f); // Stop the rumble feature

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry restartLevelMenuEntry = new MenuEntry("Restart level");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit to Main Menu");
            
            // Hook up menu event handlers.
            restartLevelMenuEntry.Selected += RestartLevelMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(restartLevelMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        public void RestartLevelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            level.player.resetStats();
            level.changeLevel();
            ExitScreen();
        }

        override public void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            texture = content.Load<Texture2D>("Menus/gameover");
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            LoadingScreen.Load(ScreenManager, true, playerIndex, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit to the Main Menu?";

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
        /// Draws the game over menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            Vector2 texturePos = new Vector2(viewport.Width - texture.Width, viewport.Height - texture.Height) / 2;
            string scoreText = "SCORE: " + hud.Score;
            Vector2 scoreTextSize = ScreenManager.Font.MeasureString(scoreText);
            Vector2 scorePos = texturePos + new Vector2((texture.Width-scoreTextSize.X)/2, 50);

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(texture, texturePos, color);

            spriteBatch.DrawString(ScreenManager.Font, "SCORE: " + hud.Score, scorePos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        #endregion
    }
}
