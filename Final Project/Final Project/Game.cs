using Final_Project.ScreenManager;
using Final_Project.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;


namespace Final_Project
{
    public enum MENU_OPTIONS
    {
        START_GAME,
        INSTRUCTIONS,
        HIGH_SCORES,
        EXIT
    }

    public enum PAUSE_OPTIONS
    {
        RESUME,
        LEAVE_GAME
    }

    public enum GAMEOVER_OPTIONS
    {
        RETURN_TO_MAIN_MENU
    }
    
    public class Game : Microsoft.Xna.Framework.Game
    {
        Account currentAccount;

        Screen scrActive;
        Screen_Basic scrStart;
        Screen_Play scrPlay;
        scrPopup scrPause;
        scrPopup scrGameOver;
        scrPopup scrInstructions;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState mouseState;
        MouseState oldMouseState;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        frmHighScores frmHighScores;

        //------------------------------------------------------------------------------------

        public SpriteFont Font
        {
            get { return spriteFont; }
        }

        //------------------------------------------------------------------------------------

        public Game()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 600;
            graphics.ApplyChanges();

            Settings.Viewport = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

            IsMouseVisible = true;

            Content.RootDirectory = Settings.Content;

            // initialize accounts
            if (Directory.Exists(Settings.Accounts))
            {
                if (Account.DefaultAccountExist())
                {
                    currentAccount = new Account();
                }
                else
                {
                    Account.CreateDefaultAccount(ref currentAccount);
                }
            }
            else
            {
                Directory.CreateDirectory(Settings.Accounts);
                Account.CreateDefaultAccount(ref currentAccount);
            }
        }

        protected override void LoadContent()
        {
            frmHighScores = new frmHighScores();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("GameFont");
            spriteFont.LineSpacing = 20;

            scrStart = new Screen_Basic(this, spriteBatch, Content.Load<Texture2D>("Graphics/Backgrounds/Start"), Menu.MenuItems(new MENU_OPTIONS()));
            Components.Add(scrStart);
            scrStart.Hide();

            scrInstructions = new scrPopup(this, spriteBatch, Content.Load<Texture2D>("Graphics/Backgrounds/Instructions"), scrStart, 500, 500);
            Components.Add(scrInstructions);
            scrInstructions.Hide();

            scrActive = scrStart;
            scrActive.Show();
        }

        protected override void Update(GameTime gameTime)
        {
            Rectangle cursor = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            previousKeyboardState = currentKeyboardState;
            oldMouseState = mouseState;

            currentKeyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // check if player is in main menu
            if (scrActive == scrStart)
            {
                if (scrInstructions.Enabled)
                {
                    if (UniqueKeyPress(Keys.Escape))
                    {
                        scrActive.Enabled = true;
                        scrInstructions.Hide();
                    }
                }
                else
                {
                    if (UniqueKeyPress(Keys.Enter) || (cursor.Intersects(scrStart.MenuAreas[scrStart.SelectedIndex]) && LeftClicked()))
                    {
                        switch (scrStart.SelectedIndex)
                        {
                            case (int)MENU_OPTIONS.START_GAME:
                                scrActive.Hide();
                                StartGame();
                                scrActive = scrPlay;
                                scrActive.Show();
                                break;

                            case (int)MENU_OPTIONS.INSTRUCTIONS:
                                scrActive.Enabled = false;
                                scrInstructions.Show();
                                break;

                            case (int)MENU_OPTIONS.HIGH_SCORES:
                                frmHighScores.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                                frmHighScores.UpdateScores(currentAccount.Scores());
                                frmHighScores.ShowDialog();
                                break;

                            case (int)MENU_OPTIONS.EXIT:
                                this.Exit();
                                break;
                        }
                    }
                }
            }
            
            // check if player is in game screen
            else if (scrActive == scrPlay)
            {
                if (UniqueKeyPress(Keys.Escape) && !scrPlay.GameOver)
                {
                    // quick pause/unpause
                    if (scrPause.Enabled)
                    {
                        UnPause();
                    }
                    else
                    {
                        Pause();
                    }
                }
                else if (scrPause.Enabled)
                {
                    // choose options in pause menu
                    if (UniqueKeyPress(Keys.Enter) || (cursor.Intersects(scrPause.MenuAreas[scrPause.SelectedIndex]) && LeftClicked()))
                    {
                        switch (scrPause.SelectedIndex)
                        {
                            case (int)PAUSE_OPTIONS.RESUME:
                                UnPause();
                                break;

                            case (int)PAUSE_OPTIONS.LEAVE_GAME:
                                CloseGame();
                                break;
                        }
                    }
                }
                else if (scrPlay.GameOver)
                {
                    // when player has no lifes left
                    scrActive.Enabled = false;
                    scrGameOver.Show();

                    // close game over screen
                    if (UniqueKeyPress(Keys.Enter) || (cursor.Intersects(scrPause.MenuAreas[scrPause.SelectedIndex]) && LeftClicked()))
                    {
                        currentAccount.SaveScore(scrPlay.Score);
                        CloseGame();
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }

        //------------------------------------------------------------------------------------

        private void StartGame()
        {
            scrPlay = new Screen_Play(this, spriteBatch);
            Components.Add(scrPlay);
            scrPlay.Hide();

            scrPause = new scrPopup(this, spriteBatch, Content.Load<Texture2D>("Graphics/Backgrounds/Pause"), scrPlay, Menu.MenuItems(new PAUSE_OPTIONS()));
            Components.Add(scrPause);
            scrPause.Hide();

            scrGameOver = new scrPopup(this, spriteBatch, Content.Load<Texture2D>("Graphics/Backgrounds/GameOver"), scrPlay, Menu.MenuItems(new GAMEOVER_OPTIONS()));
            Components.Add(scrGameOver);
            scrGameOver.Hide();
        }

        private void CloseGame()
        {
            scrGameOver.Dispose();
            scrPause.Dispose();
            scrPlay.Dispose();

            scrActive = scrStart;
            scrActive.Show();
        }

        private void Pause()
        {
            scrActive.Enabled = false;
            scrPause.Show();
        }

        private void UnPause()
        {
            scrPause.Hide();
            scrActive.Enabled = true;
        }

        //------------------------------------------------------------------------------------

        private bool UniqueKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        private bool LeftClicked()
        {
            return (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released);
        }
    }
}
