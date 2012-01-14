using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.Classes
{
    class Unit_Animated_Player : Unit_Animated
    {
        // player also need animation in y-axis to show 
        // blinking immunity (red toggle) after getting hit

        int yFrames = 2;
        int yFps = 4;

        bool yActive;
        int yCurrentFrameIndex;
        TimeSpan yPrevFrameTime;
        TimeSpan yFrameTime;

        // y-axis animation status
        public bool YActive
        {
            get { return yActive; }
            set { yActive = value; }
        }

        // work in conjunction with powerups
        public void AdjustSpeed(float modifier)
        {
            speed = Settings.Player_Speed * modifier;
        }

        public Unit_Animated_Player(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            int xFrames = 3;
            int xFps = 15;

            icon = content.Load<Texture2D>("Player");
            animationSettings = new AnimationSettings(icon.Width / xFrames, icon.Height / yFrames, xFrames, xFps, false);

            currentFrameIndex = 1;
            prevFrameTime = TimeSpan.Zero;
            frameTime = TimeSpan.FromSeconds((double)xFrames / (double)xFps);

            yCurrentFrameIndex = 1;
            yPrevFrameTime = TimeSpan.Zero;
            yFrameTime = TimeSpan.FromSeconds((double)yFrames / (double)yFps);

            health = Settings.Player_Health;
            position = new Vector2((Settings.Viewport.Width - Width) / 2, (Settings.Viewport.Height - (Height + Settings.Player_Offset)));
            speed = Settings.Player_Speed;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (yActive)
            {
                if (gameTime.TotalGameTime - yPrevFrameTime >= yFrameTime)
                {
                    // time to switch frame
                    yPrevFrameTime = gameTime.TotalGameTime;

                    if (yCurrentFrameIndex % yFrames == 0)
                    {
                        // after last frame, reset to first frame again
                        yCurrentFrameIndex = 1;
                        animationSettings.sourceFrame.Y = 0;
                    }
                    else
                    {
                        // next frame
                        animationSettings.sourceFrame.Y = animationSettings.sourceFrame.Height * yCurrentFrameIndex;
                        yCurrentFrameIndex++;
                    }
                }
            }
            else
            {
                // default if no animation in y-axis
                yCurrentFrameIndex = 1;
                animationSettings.sourceFrame.Y = 0;
            }
        }

        // move player based on commands from play screen
        public void MoveLeft()      { position.X -= speed; }
        public void MoveRight()     { position.X += speed; }
        public void SpeedUp()       { position.Y -= speed; }
        public void SpeedDown()     { position.Y += speed; }
    }
}
