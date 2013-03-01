using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a game object which can cause damage to other entities.
    /// </summary>
    internal interface IDamage : IGameObject
    {
        /// <summary>
        /// Faction of this object, either player or enemy. Objects cannot be 
        /// damaged by others of the same faction.
        /// </summary>
        Faction Faction
        {
            get;
        }

        int DamageOnHit
        {
            get;
        }
    }
}
