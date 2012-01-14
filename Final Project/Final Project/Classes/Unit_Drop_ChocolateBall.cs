using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    // this class customize aesthetics
    // main game logic in Screen_Play.cs

    class Unit_Drop_ChocolateBall : Unit_Drop
    {
        public Unit_Drop_ChocolateBall(Game game, SpriteBatch spriteBatch, Unit_Enemy sourceEnemy)
            : base(game, spriteBatch, sourceEnemy)
        {
            icon = content.Load<Texture2D>("ChocolateBall");

            // center on source enemy
            position.X = (sourceEnemy.Position.X + sourceEnemy.Width / 2) - Width / 2;
            position.Y = (sourceEnemy.Position.Y + sourceEnemy.Height / 2) - Height / 2;
        }
    }
}
