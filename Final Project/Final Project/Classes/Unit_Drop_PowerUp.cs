using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
namespace Final_Project.Classes
{
    // this class customize aesthetics
    // main game logic in Screen_Play.cs
    
    public enum POWERUP
    {
        YELLOW,
        GREEN,
        BLUE,
        RED
    }

    class Unit_Drop_PowerUp : Unit_Drop
    {
        POWERUP powerUp;

        public POWERUP PowerUp
        {
            get { return powerUp; }
        }

        public Unit_Drop_PowerUp(Game game, SpriteBatch spriteBatch, Unit_Enemy sourceEnemy, int type)
            : base(game, spriteBatch, sourceEnemy)
        {
            // type of powerup
            powerUp = (POWERUP)type;
            icon = content.Load<Texture2D>("P_" + type);

            // center on source enemy
            position.X = (sourceEnemy.Position.X + sourceEnemy.Width / 2) - Width / 2;
            position.Y = (sourceEnemy.Position.Y + sourceEnemy.Height / 2) - Height / 2;
        }
    }
}
