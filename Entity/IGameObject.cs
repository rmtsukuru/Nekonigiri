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
            set;
        }

        /// <summary>
        /// Gets this game object's position relative to a camera.
        /// </summary>
        /// <param name="camera">The specified camera.</param>
        /// <returns>The relative position coordinates.</returns>
        Vector2 RelativePosition(ICamera camera);

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
        /// Indicates whether this game object has been destroyed (so as to 
        /// determine when to remove it from the game objects list).
        /// </summary>
        bool Destroyed
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
