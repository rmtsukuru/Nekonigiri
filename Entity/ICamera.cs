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
    }
}
