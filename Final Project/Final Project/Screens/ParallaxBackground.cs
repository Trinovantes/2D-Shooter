using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.Screens
{
    class ParallaxBackground : DrawableGameComponent
    {
        float speed;
        int screenHeight;

        SpriteBatch spriteBatch;
        Texture2D background;
        Vector2[] positions;

        public float Speed
        {
            get { return speed; }
        }

        public ParallaxBackground(Game game, SpriteBatch spriteBatch, int background)
            : base(game)
        {
            this.screenHeight = Settings.Viewport.Height;
            this.spriteBatch = spriteBatch;
            this.background = new ContentManager(game.Services, Settings.Content_Background).Load<Texture2D>("Play" + background);

            // initialize abstract backgrounds enough to cover 
            // entire screen and after scrolling
            positions = new Vector2[(screenHeight / this.background.Height) + 2];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(0, (i - 1) * this.background.Height);
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].Y += speed;

                // reset position at end
                if (positions[i].Y >= screenHeight)
                {
                    positions[i].Y = -(this.background.Height + speed); // put back into starting position
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Draw(background, positions[i], Color.White);
            }
        }

        public void SetSpeed(float value)
        {
            speed = value;
        }

        public void IncreaseSpeed()
        {
            speed *= 1.2f;
        }
    }
}
