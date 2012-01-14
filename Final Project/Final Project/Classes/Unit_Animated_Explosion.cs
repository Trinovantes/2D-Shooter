using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    class Unit_Animated_Explosion : Unit_Animated
    {
        public Unit_Animated_Explosion(Game game, SpriteBatch spriteBatch, Unit_Enemy sourceEnemy)
            : base(game, spriteBatch)
        {
            int xFrames = 12;
            int xFps = 60;

            icon = content.Load<Texture2D>("Explosion");
            animationSettings = new AnimationSettings(icon.Width / xFrames, icon.Height, xFrames, xFps, true);

            currentFrameIndex = 1;
            prevFrameTime = TimeSpan.Zero;
            frameTime = TimeSpan.FromSeconds((double)xFrames / (double)xFps);

            // center on source enemy
            position.X = (sourceEnemy.Position.X + sourceEnemy.Width / 2) - Width / 2;
            position.Y = (sourceEnemy.Position.Y + sourceEnemy.Height / 2) - Height / 2;
        }
    }
}
