using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.Classes
{
    class Unit_Projectile : Unit
    {
        int angle;
        int damage;

        public int Damage
        {
            get { return damage; }
        }

        // returns angle in radians
        private double Angle
        {
            get { return angle * Math.PI / 180; }
        }

        public Unit_Projectile(Game game, SpriteBatch spriteBatch, Unit_Animated_Player player, int level, int projAngle)
            : base(game, spriteBatch)
        {
            // level refers to projectile level, NOT game level

            // type of projectile
            int type = (int)MathHelper.Clamp(level, 0, 3);
            icon = content.Load<Texture2D>("B_" + type);

            angle = projAngle;
            damage = Settings.Projectile_DamageConstant + level * (int)Settings.Projectile_DamageMultiplier;

            position = new Vector2(player.Position.X + (player.Width - Width) / 2, player.Position.Y - Height);
            speed = Settings.Projectile_SpeedConstant + type * Settings.Projectile_SpeedMultiplier;
        }

        public override void Update(GameTime gameTime)
        {
            // angle measured from standard position
            position.Y -= (float)(Math.Sin(Angle) * speed);
            position.X += (float)(Math.Cos(Angle) * speed);

            // disable once it goes offscreen
            if (!Area.Intersects(Settings.Viewport))
            {
                active = false;
            }
        }
    }
}
