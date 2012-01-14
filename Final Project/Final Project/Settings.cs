using System;
using Final_Project.Screens;
using Microsoft.Xna.Framework;

namespace Final_Project
{
    // stores all/most game values
    // i.e. multipliers, constants, locations

    static class Settings
    {
        static Settings()
        {
            // General Assets

            Accounts = "Accounts/";
            Account_Default = "default.txt";

            Content = "Content/";
            Content_Background = "Content/Graphics/Backgrounds/";
            Content_Units = "Content/Graphics/Units/";
            Content_Projectiles = "Content/Graphics/Projectiles/";

            // Menu

            Menu_xPadding = 20;
            Menu_yPadding = 10;
            Menu_ShadowPadding = 10;

            // Play Screen

            CloudSpeeds[0] = 0.8f;
            CloudSpeeds[1] = 1.0f;
            CloudSpeeds[2] = 1.2f;

            Intervals.Enemy = TimeSpan.FromSeconds(3);
            Intervals.Projectile = TimeSpan.FromSeconds(0.2);
            Intervals.GameLevel = TimeSpan.FromSeconds(30);
            Intervals.OnHitDelay = TimeSpan.FromSeconds(3);
            Intervals.Drop = TimeSpan.FromSeconds(10);

            InfoTitle_Score = "Score";
            InfoTitle_Health = "Life";
            InfoTitle_Level = "Level";
            InfoTitle_Projectile = "Ball Level";
            InfoTitle_Padding = 20;
            InfoTitle_Colour = Color.Black;

            // Units

            /*
             * For most game calculations:
             * 
             * Value = Constant + Base * Multiplier
             * 
             * Base corresponds to uncapped 0-based level
             * such as projectile level or game level.
             * 
             * If no constant located in settings, then 
             * assumed to be 0 and base is changed to 1-based.
             * 
             * Exception:
             * Enemy Health = Base * Multiplier ^ cLevel
             * 
             */

            Player_Health = 3;
            Player_Speed = 8;
            Player_Offset = 40;

            //

            Enemy_HealthMultiplier = 1.5f;
            Enemy_PointMultiplier = 5;

            Enemy_SpeedConstant = 4;
            Enemy_SpeedMultiplier = 0.5f;

            //

            Projectile_MidAngle = 10;
            Projectile_MaxLevel = 4;

            Projectile_DamageConstant = 1;
            Projectile_DamageMultiplier = 2;

            Projectile_SpeedConstant = 10;
            Projectile_SpeedMultiplier = 5;
        }

        //---------------------------------------
        // General Assets
        //---------------------------------------

        public static Rectangle Viewport;

        public static readonly string Accounts;
        public static readonly string Account_Default;

        public static readonly string Content;
        public static readonly string Content_Background;
        public static readonly string Content_Units;
        public static readonly string Content_Projectiles;

        //---------------------------------------
        // Menu
        //---------------------------------------

        public static readonly int Menu_xPadding;
        public static readonly int Menu_yPadding;
        public static readonly int Menu_ShadowPadding;

        //---------------------------------------
        // Play Screen
        //---------------------------------------

        public readonly static float[] CloudSpeeds = new float[3];

        public readonly static Timer Intervals;

        public readonly static string InfoTitle_Score;
        public readonly static string InfoTitle_Health;
        public readonly static string InfoTitle_Level;
        public readonly static string InfoTitle_Projectile;
        public readonly static int InfoTitle_Padding;
        public readonly static Color InfoTitle_Colour;

        //---------------------------------------
        // Units
        //---------------------------------------

        public readonly static float Enemy_HealthMultiplier;
        public readonly static float Enemy_PointMultiplier;

        public readonly static int Enemy_SpeedConstant;
        public readonly static float Enemy_SpeedMultiplier;

        //

        public readonly static int Player_Health;
        public readonly static int Player_Speed;
        public readonly static int Player_Offset;
        
        //

        public readonly static int Projectile_MidAngle;
        public readonly static int Projectile_MaxLevel;

        public readonly static int Projectile_DamageConstant;
        public readonly static float Projectile_DamageMultiplier;

        public readonly static int Projectile_SpeedConstant;
        public readonly static float Projectile_SpeedMultiplier;
    }
}
