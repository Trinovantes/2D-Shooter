using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project.Classes
{
    abstract class Unit : DrawableGameComponent
    {
        protected bool active;
        protected int health;
        protected float speed;

        protected ContentManager content;
        protected SpriteBatch spriteBatch;
        protected Texture2D icon;
        protected Vector2 position;

        //------------------------------------------------------------------------------------

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // properties marked virtual to be overwritten in child classes

        public virtual int Width
        {
            get { return icon.Width; }
        }

        public virtual int Height
        {
            get { return icon.Height; }
        }

        public Rectangle Area
        {
            get { return new Rectangle((int)position.X, (int)position.Y, Width, Height); }
        }

        //------------------------------------------------------------------------------------

        public Unit(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.active = true;
            this.content = new ContentManager(game.Services, Settings.Content_Units);
            this.spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(icon, position, Color.White);
        }
    }
}
