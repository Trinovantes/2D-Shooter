using Final_Project.ScreenManager;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.Screens
{
    // essentially useless class; used to keep Screen class abstract
    class Screen_Basic : Screen
    {
        public Screen_Basic(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage)
            : base(game, spriteBatch, backgroundImage)
        {

        }

        public Screen_Basic(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage, string[] menuItems)
            : base(game, spriteBatch, backgroundImage, menuItems)
        {

        }
    }
}
