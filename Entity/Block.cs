using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class Block : GameObject
    {
        public Block()
            : this(0, 0)
        { }

        public Block(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        public Block(Vector2 position)
            : base(new Sprite(GameData.Instance.Content.Load<Texture2D>("block")), true)
        {
            this.Position = position;
        }
    }
}
