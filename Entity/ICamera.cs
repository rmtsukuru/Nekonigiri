using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a game camera, which displays a portion of the current level
    /// to the player.
    /// </summary>
    internal interface ICamera
    {
        Vector2 Position
        {
            get;
        }

        Vector2 Velocity
        {
            get;
        }

        void Update(GameTime gameTime);

        /// <summary>
        /// Converts from coordinates relative to this camera to absolute
        /// coordinates.
        /// </summary>
        /// <param name="relativePosition">Camera-relative coordinates.</param>
        /// <returns>Absolute coordinates.</returns>
        Vector2 absolutePosition(Vector2 relativePosition);

        /// <summary>
        /// Converts from coordinates relative to this camera to absolute
        /// coordinates.
        /// </summary>
        /// <param name="x">Relative x-coordinate.</param>
        /// <param name="y">Relative y-coordinate.</param>
        /// <returns>Absolute coordinates.</returns>
        Vector2 absolutePosition(float x, float y);
    }
}
