using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    /// <summary>
    /// Represents an object appearing in the game world.
    /// </summary>
    internal interface IGameObject
    {
        Vector2 Position
        {
            get;
        }

        Vector2 Velocity
        {
            get;
        }

        /// <summary>
        /// Retrieves this game object's hitbox in relative coordinates.
        /// </summary>
        Rectangle Hitbox
        {
            get;
        }

        /// <summary>
        /// Retrieves this game object's hitbox with coordinates translated by 
        /// its position.
        /// </summary>
        Rectangle TranslatedHitbox
        {
            get;
        }

        /// <summary>
        /// Specifies whether this game object prevents being moved through 
        /// (I.E. can be collided with).
        /// </summary>
        bool IsCollideable
        {
            get;
        }

        /// <summary>
        /// Responds to this object coming in contact with another game object.
        /// </summary>
        /// <param name="entity">The other entity being touched.</param>
        void Touches(IGameObject entity);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
