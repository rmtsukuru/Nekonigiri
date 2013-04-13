using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class Tile : GameObject
    {
        public Tile(Texture2D texture, Rectangle sourceRect, Rectangle? hitbox)
            : base(new Sprite(texture), hitbox != null)
        {
            this.sprite.SourceRectangle = sourceRect;
            this.Hitbox = hitbox != null ? (Rectangle)hitbox : Rectangle.Empty;
        }

        public override void Touches(IGameObject entity)
        {
            // Do nothing. Right?
        }
    }
}
