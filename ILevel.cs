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
        /// Returns a list of entities in the level near the specified game object.
        /// </summary>
        /// <param name="entity">The specified entity.</param>
        /// <returns></returns>
        IList<IGameObject> objectsCloseTo(IGameObject entity);
    }
}
