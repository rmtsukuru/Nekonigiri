using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a game level. Currently filler for in-entity collision calculations.
    /// </summary>
    internal interface ILevel
    {
        /// <summary>
        /// Width of this level in pixels.
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Height of this level in pixels.
        /// </summary>
        int Height
        {
            get;
        }

        /// <summary>
        /// Returns a list of entities in the level near the specified game object.
        /// </summary>
        /// <param name="entity">The specified entity.</param>
        /// <returns></returns>
        IList<IGameObject> ObjectsCloseTo(IGameObject entity);

        /// <summary>
        /// Inserts the specified game object into the level.
        /// </summary>
        /// <param name="entity">The specified entity.</param>
        void AddObject(IGameObject entity);
    }
}
