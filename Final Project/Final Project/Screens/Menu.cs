using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Final_Project.ScreenManager
{
    public class Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string[] menuItems;
        int selectedIndex;

        Screen parentScreen;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState mouseState;

        ContentManager content;
        Texture2D menuSprite;
        Texture2D backgroundShadow;
        Color stateNormal = Color.White;
        Color stateActive = Color.Black;

        Vector2 position;
        Rectangle[] menuAreas;
        int width;
        int height;

        int xPadding = Settings.Menu_xPadding;
        int yPadding = Settings.Menu_yPadding;
        int ShadowPadding = Settings.Menu_ShadowPadding;

        //------------------------------------------------------------------------------------

        // return/change selected index
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;

                if (selectedIndex < 0)
                {
                    // go to end
                    selectedIndex = menuItems.Length - 1;
                }
                else if (selectedIndex >= menuItems.Length)
                {
                    // back to first
                    selectedIndex = 0;
                }
            }
        }

        // array of menu items' areas
        public Rectangle[] MenuAreas
        {
            get { return menuAreas; }
        }

        //------------------------------------------------------------------------------------

        public Menu(Game game, Screen activeScreen, SpriteBatch spriteBatch, SpriteFont spriteFont, string[] menuItems)
            : base(game)
        {
            this.parentScreen = activeScreen;
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.menuItems = menuItems;

            content = new ContentManager(game.Services, Settings.Content_Background);
            menuSprite = content.Load<Texture2D>("Menu");
            backgroundShadow = content.Load<Texture2D>("Shadow");

            position = Vector2.Zero;
            menuAreas = new Rectangle[menuItems.Length];
            MeasureMenu();
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle cursor = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            Rectangle menuArea = new Rectangle((int)position.X,(int)position.Y,width,height);

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            
            if (cursor.Intersects(menuArea))
            {
                for (int i = 0; i < menuAreas.Length; i++)
                {
                    if (cursor.Intersects(menuAreas[i]))
                    {
                        SelectedIndex = i;
                    }
                }
            }
            else
            {
                if (UniqueKeyPress(Keys.Down))
                {
                    SelectedIndex++;
                }
                else if (UniqueKeyPress(Keys.Up))
                {
                    SelectedIndex--;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle sourceLocation;   // source rectange to draw from menu sprite
            Vector2 textPosition;       // location of text

            Rectangle ShadowLocation = new Rectangle(
                (int)menuAreas[0].X - ShadowPadding, 
                (int)menuAreas[0].Y - ShadowPadding, 
                menuAreas[0].Width + 2 * ShadowPadding, 
                menuAreas.Length * menuAreas[0].Height +  2 * ShadowPadding);

            spriteBatch.Draw(backgroundShadow, ShadowLocation, Color.White);

            for (int i = 0; i < menuItems.Length; i++)
            {
                sourceLocation = new Rectangle(0, (i == selectedIndex) ? 40 : 0, menuAreas[i].Width, menuAreas[i].Height);
                textPosition = new Vector2(menuAreas[i].X + xPadding, menuAreas[i].Y + yPadding);

                spriteBatch.Draw(menuSprite, menuAreas[i], sourceLocation, Color.White);
                spriteBatch.DrawString(spriteFont, menuItems[i], textPosition, (i == selectedIndex) ? stateActive : stateNormal);
            }
        }

        //------------------------------------------------------------------------------------

        // calculate size and position of menu on screen
        private void MeasureMenu()
        {
            Vector2 fontSize;
            int itemHeight = spriteFont.LineSpacing + yPadding * 2;

            // loop through each menu item to determine menu max dimensions
            foreach (string item in menuItems)
            {
                fontSize = spriteFont.MeasureString(item);

                if (fontSize.X > width)
                {
                    width = (int)fontSize.X;
                }

                height += itemHeight;
            }
            
            // absolute center
            position.X = (parentScreen.Width - width) / 2;
            position.Y = (parentScreen.Height - height) / 2;
            position = Vector2.Add(position, parentScreen.Position);

            // calculate exact rectangles of menu items
            for (int i = 0; i < menuAreas.Length; i++)
            {
                menuAreas[i].X = (int)position.X - xPadding;
                menuAreas[i].Y = (int)position.Y - yPadding + i * itemHeight;
                menuAreas[i].Width = width + xPadding * 2;
                menuAreas[i].Height = itemHeight;
            }
        }

        // check if given key is an unique (new) command
        private bool UniqueKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }
        
        // returns human friendly array of menu options from an enum
        public static string[] MenuItems(Enum menuEnum)
        {
            string[] menuItems = Enum.GetNames(menuEnum.GetType());

            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(menuItems[i].Replace("_", " ").ToLower());
            }

            return menuItems;
        }
    }
}
