using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Final_Project.ScreenManager
{
    public abstract class Screen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected List<GameComponent> components = new List<GameComponent>();

        protected SpriteBatch spriteBatch;
        protected SpriteFont spriteFont;
        protected Texture2D backgroundImage;

        protected Vector2 position;
        protected int width;
        protected int height;

        protected Menu menu;

        //------------------------------------------------------------------------------------

        public Vector2 Position
        {
            get { return position; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int SelectedIndex
        {
            get { return menu.SelectedIndex; }
            set { menu.SelectedIndex = value; }
        }

        public Rectangle[] MenuAreas
        {
            get { return menu.MenuAreas; }
        }

        //------------------------------------------------------------------------------------

        // basic screen
        public Screen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = game.Font;

            position = Vector2.Zero;
            width = game.Window.ClientBounds.Width;
            height = game.Window.ClientBounds.Height;
        }

        // basic screen with a background
        public Screen(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = game.Font;
            this.backgroundImage = backgroundImage;

            position = Vector2.Zero;
            width = game.Window.ClientBounds.Width;
            height = game.Window.ClientBounds.Height;
        }

        // basic screen with a background and menu 
        public Screen(Game game, SpriteBatch spriteBatch, Texture2D backgroundImage, string[] menuItems)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = game.Font;
            this.backgroundImage = backgroundImage;

            position = Vector2.Zero;
            width = game.Window.ClientBounds.Width;
            height = game.Window.ClientBounds.Height;

            menu = new Menu(game, this, spriteBatch, spriteFont, menuItems);
            components.Add(menu);
        }

        public override void Update(GameTime gameTime)
        {
            // update components
            foreach (GameComponent component in components)
            {
                if (component.Enabled)
                {
                    component.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // draw any background textures
            if (backgroundImage != null)
            {
                spriteBatch.Draw(backgroundImage, position, Color.White);
            }

            // draw components
            foreach (GameComponent component in components)
            {
                if (component.Enabled)
                {
                    if (component is DrawableGameComponent)
                    {
                        ((DrawableGameComponent)component).Draw(gameTime);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------

        // show screen and any child components
        public virtual void Show()
        {
            Visible = true;
            Enabled = true;

            if (menu != null)
            {
                SelectedIndex = 0;
            }

            foreach (GameComponent component in components)
            {
                component.Enabled = true;

                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = true;
                }
            }
        }

        // hide screen and any child components
        public virtual void Hide()
        {
            Visible = false;
            Enabled = false;

            foreach (GameComponent component in components)
            {
                component.Enabled = false;

                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = false;
                }
            }
        }
    }
}
