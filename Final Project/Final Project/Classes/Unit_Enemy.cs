using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    class Unit_Enemy : Unit
    {
        int maxDistance;
        int pointValue;

        bool crashed;       // crashing with player => no point gain
        bool destroyed;     // destroyed by projectile => point++

        public int PointValue
        {
            get { return pointValue; }
        }

        public bool Destoryed
        {
            get { return destroyed; }
        }

        public void Crashed()
        {
            crashed = true;
            health = 0;
        }

        public Unit_Enemy(Game game, SpriteBatch spriteBatch, int level)
            : base(game, spriteBatch)
        {
            // level refers to game level

            Random random = new Random();

            // type of enemy
            int type = (int)MathHelper.Clamp(random.Next(0, level + 1), 0, 3);
            icon = content.Load<Texture2D>("E_" + type);

            health = (level + 1) * (int)Math.Pow(Settings.Enemy_HealthMultiplier, type);
            position = new Vector2(random.Next(Settings.Viewport.Width - Width), random.Next(Settings.Viewport.Height - Height) / 4);
            speed = Settings.Enemy_SpeedConstant + type * Settings.Enemy_SpeedMultiplier;

            // misc
            maxDistance = Settings.Viewport.Height;
            pointValue = (type + 1) * (int)Settings.Enemy_PointMultiplier;
        }

        public override void Update(GameTime gameTime)
        {
            position.Y += speed;

            // disable if it went offscreen or no health
            if (Position.Y + Height >= maxDistance || health <= 0)
            {
                active = false;

                // player destroyed enemy if it did not 
                // go offscreen and did not crash with player
                if (!crashed && Position.Y + Height < maxDistance)
                {
                    destroyed = true;
                }
            }
        }
    }
}
