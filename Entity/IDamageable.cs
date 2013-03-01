using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a game entity that can be damaged. Should be destroyed when 
    /// its health drops to 0 or below.
    /// </summary>
    internal interface IDamageable : IGameObject
    {
        /// <summary>
        /// Faction of this object, either player or enemy. Objects cannot be 
        /// damaged by others of the same faction.
        /// </summary>
        Faction Faction
        {
            get;
        }

        int Health
        {
            get;
        }

        int MaxHealth
        {
            get;
        }

        /// <summary>
        /// Inflicts the specified amount of damage.
        /// </summary>
        /// <param name="amount">Amount of damage to cause.</param>
        void Damage(int amount);
    }
}
