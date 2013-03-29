using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a powerup which can be picked up by the player to restore health.
    /// </summary>
    internal class HealthPack : GameObject
    {
        private const int BaseHealAmount = 50;

        public HealthPack()
            : this(Vector2.Zero)
        { }

        public HealthPack(Vector2 position)
            : this(HealthPack.BaseHealAmount, position)
        { }

        public HealthPack(int healAmount, Vector2 position) : base(FetchSprite(), false)
        {
            this.HealAmount = healAmount;
            this.Position = position;
        }

        private static Sprite FetchSprite()
        {
            return new Sprite(GameData.Instance.Content.Load<Texture2D>("healthpack"));
        }

        public int HealAmount
        {
            get;
            private set;
        }

        public override void Touches(IGameObject entity)
        {
            if (entity.GetType() == typeof(Neko))
            {
                this.Destroyed = true;
                // TODO: Update GameObject to use the Destroy() method instead.
            }
        }
    }
}
