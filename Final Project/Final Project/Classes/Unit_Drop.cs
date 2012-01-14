using Final_Project.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    class Unit_Drop : Unit
    {
        int screenHeight;

        public Unit_Drop(Game game, SpriteBatch spriteBatch, Unit_Enemy sourceEnemy)
            : base(game, spriteBatch)
        {
            speed = sourceEnemy.Speed / 2;
            screenHeight = Settings.Viewport.Height;
        }

        public override void Update(GameTime gameTime)
        {
            position.Y += speed;

            // disable once drop goes offscreen
            if (position.Y >= screenHeight)
            {
                active = false;
            }
        }
    }
}
