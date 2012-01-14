using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.ScreenManager
{
    class scrPopup : Screen
    {
        Screen parentScreen;            // used to calculate absolute center position
        Texture2D backgroundShadow;

        // popup without menu
        public scrPopup(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage, Screen parentScreen, int width = 300, int height = 400)
            : base(game, spriteBatch, backgroundImage)
        {
            this.parentScreen = parentScreen;
            
            this.backgroundImage = backgroundImage;
            this.backgroundShadow = new ContentManager(game.Services, Settings.Content_Background).Load<Texture2D>("Shadow");

            this.width = width;
            this.height = height;
            MeasureScreen();
        }

        // popup with menu
        public scrPopup(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage, Screen parentScreen, string[] menuItems, int width = 300, int height = 400)
            : base(game, spriteBatch, backgroundImage)
        {
            this.parentScreen = parentScreen;

            this.backgroundImage = backgroundImage;
            this.backgroundShadow = new ContentManager(game.Services, Settings.Content_Background).Load<Texture2D>("Shadow");

            this.width = width;
            this.height = height;
            MeasureScreen();

            menu = new Menu(game, this, spriteBatch, spriteFont, menuItems);
            components.Add(menu);
        }
        
        private void MeasureScreen()
        {
            // absolute centering 
            position.X = (parentScreen.Width - width) / 2;
            position.Y = (parentScreen.Height - height) / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            // drawn black shadow underneath
            spriteBatch.Draw(backgroundShadow, parentScreen.Position, Color.White);

            // draw popup background
            base.Draw(gameTime);
        }
    }
}
