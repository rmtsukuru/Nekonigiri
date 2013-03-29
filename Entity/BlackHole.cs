using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Represents an invisible wall that destroys all it touches.
    /// </summary>
    internal class BlackHole : InvisibleWall
    {
        public BlackHole(Rectangle hitbox)
            : base(hitbox)
        { }

        public override void Touches(IGameObject entity)
        {
            if (entity is IDamageable)
            {
                (entity as IDamageable).Destroy();
            }
        }
    }
}
