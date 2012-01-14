using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    // struct to organize settings
    public struct AnimationSettings
    {
        public Rectangle sourceFrame;
        public int frameCount;
        public int fps;
        public bool deleteAtEnd;

        public AnimationSettings(int width, int height, int frameCount, int fps, bool deleteAtEnd)
        {
            this.sourceFrame = new Rectangle(0, 0, width, height);
            this.frameCount = frameCount;
            this.fps = fps;
            this.deleteAtEnd = deleteAtEnd;
        }
    }

    // unit class with built-in animation in the x-axis
    // must specify frames and frame rate in x direction
    abstract class Unit_Animated : Unit
    {
        protected AnimationSettings animationSettings;
        protected int currentFrameIndex;
        protected TimeSpan prevFrameTime;
        protected TimeSpan frameTime;

        // dimensions are frame size rather than full sprite size
        public override int Width
        {
            get { return animationSettings.sourceFrame.Width; }
        }

        public override int Height
        {
            get { return animationSettings.sourceFrame.Height; }
        }

        public Unit_Animated(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            currentFrameIndex = 1;
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - prevFrameTime >= frameTime)
            {
                // time to switch frame
                prevFrameTime = gameTime.TotalGameTime;

                if (currentFrameIndex % animationSettings.frameCount == 0)
                {
                    // reset after last frame
                    currentFrameIndex = 1;
                    animationSettings.sourceFrame.X = 0;

                    // remove after last frame if specified
                    if (animationSettings.deleteAtEnd)
                    {
                        active = false;
                    }
                }
                else
                {
                    // next frame
                    animationSettings.sourceFrame.X = animationSettings.sourceFrame.Width * currentFrameIndex;
                    currentFrameIndex++;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // where to draw sliced frame
            Rectangle frame = new Rectangle(
                (int)position.X, 
                (int)position.Y, 
                animationSettings.sourceFrame.Width, 
                animationSettings.sourceFrame.Height);
            
            spriteBatch.Draw(icon, frame, animationSettings.sourceFrame, Color.White);
        }
    }
}
