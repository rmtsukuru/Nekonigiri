using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal abstract class GameObject : IGameObject
    {
        protected Sprite sprite;

        public Vector2 Position
        {
            get;
            protected set;
        }

        public Vector2 Velocity
        {
            get;
            protected set;
        }

        public Rectangle Hitbox
        {
            get;
            protected set;
        }

        public Rectangle TranslatedHitbox
        {
            get
            {
                return new Rectangle((int) (Hitbox.X + Position.X), (int) (Hitbox.Y + Position.Y), 
                                     Hitbox.Width, Hitbox.Height);
            }
        }

        public bool IsCollideable
        {
            get;
            private set;
        }

        public bool Destroyed
        {
            get;
            protected set;
        }

        public GameObject(Sprite sprite, bool collideable)
        {
            this.sprite = sprite;
            this.IsCollideable = collideable;
            this.Position = Vector2.Zero;
            this.Hitbox = new Rectangle(0, 0, sprite.Texture.Width, sprite.Texture.Height);
        }

        /// <summary>
        /// Responds to this object coming in contact with another game object.
        /// </summary>
        /// <param name="entity">The other entity being touched.</param>
        public abstract void Touches(IGameObject entity);

        public virtual void Update(GameTime gameTime)
        {
            this.Position = new Vector2(this.Position.X + this.Velocity.X, this.Position.Y + this.Velocity.Y);
            this.sprite.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.sprite.Position = this.Position;
            this.sprite.Draw(spriteBatch, gameTime);
        }

    }
}
