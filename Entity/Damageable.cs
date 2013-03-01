using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a game entity that can be damaged. For IDamageable code reuse.
    /// </summary>
    internal abstract class Damageable : GameObject, IDamageable
    {
        /// <summary>
        /// Faction of this object, either player or enemy. Objects cannot be 
        /// damaged by others of the same faction.
        /// </summary>
        public Faction Faction
        {
            get;
            private set;
        }

        public int Health
        {
            get;
            protected set;
        }

        public int MaxHealth
        {
            get;
            protected set;
        }

        /// <summary>
        /// Instantiates a new damageable with full health.
        /// </summary>
        /// <param name="faction">The faction of the entity.</param>
        /// <param name="maxHP">The entity's maximum health.</param>
        public Damageable(Sprite sprite, Faction faction, int maxHP, bool collideable)  
            : this(sprite, faction, maxHP, maxHP, collideable)
        {

        }

        public Damageable(Sprite sprite, Faction faction, int maxHP, int currentHP, bool collideable) 
            : base(sprite, collideable)
        {
            this.Faction = faction;
            this.MaxHealth = maxHP;
            this.Health = currentHP;
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add appropriate update behavior here.
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Inflicts the specified amount of damage.
        /// </summary>
        /// <param name="amount">Amount of damage to cause.</param>
        public void Damage(int amount)
        {
            this.Health -= amount;
            if (this.Health <= 0)
            {
                this.Destroy();
            }
            else if (this.Health > this.MaxHealth)
            {
                this.Health = this.MaxHealth;
            }
        }

        /// <summary>
        /// Destroys this entity, as its health has run out.
        /// </summary>
        public abstract void Destroy();
    }
}
