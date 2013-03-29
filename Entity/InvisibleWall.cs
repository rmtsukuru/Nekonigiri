using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    /// <summary>
    /// Represents an invisible, but impassable region in the gameworld.
    /// Has a hitbox, but no sprite.
    /// </summary>
    internal class InvisibleWall : GameObject
    {
        public InvisibleWall(Rectangle hitbox) : base(new Sprite(GameData.Instance.Content.Load<Texture2D>("blank")), true)
        {
            this.Hitbox = hitbox;
        }

        public override void Touches(IGameObject entity)
        {
            // Do nothing - it's just an invisible wall.
        }
    }
}
