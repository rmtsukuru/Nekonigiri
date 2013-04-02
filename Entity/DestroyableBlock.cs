using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class DestroyableBlock : Damageable
    {
        private const int MaxHP = 10;

        public DestroyableBlock()
            : this(0, 0)
        { }

        public DestroyableBlock(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        public DestroyableBlock(Vector2 position)
            : base(new Sprite(GameData.Instance.Content.Load<Texture2D>("iceblock")), Faction.Enemy, MaxHP, true)
        {
            this.Position = position;
        }

        public override void Touches(IGameObject entity)
        {
            // TODO: Figure out if this should do anything.
        }
    }
}
