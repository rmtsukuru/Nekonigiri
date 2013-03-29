using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class ProjectileOnigiri : Damageable, IDamage
    {
        public static readonly float Width = ProjectileOnigiri.FetchSprite().Texture.Width;

        private const float BaseMovementSpeed = 5;
        private const float AngularVelocity = 0.01f; // In radians - about 20 degrees.
        private const int DamageValue = 10;

        private float spin;

        public int DamageOnHit
        {
            get { return ProjectileOnigiri.DamageValue; }
        }
        
        public bool IsFacingRight
        {
            get;
            protected set;
        }

        public ProjectileOnigiri(Vector2 position, bool isFacingRight)
            : base(FetchSprite(), Faction.Player, 1, true)
        {
            this.Position = position;
            this.IsFacingRight = isFacingRight;
            this.Velocity = isFacingRight ? new Vector2(BaseMovementSpeed, 0) : new Vector2(-1 * BaseMovementSpeed, 0);
            this.spin = 0;
            this.sprite.Origin = new Vector2(this.sprite.Texture.Width / 2, this.sprite.Texture.Height / 2);
        }

        private static Sprite FetchSprite()
        {
            return new Sprite(GameData.Instance.Content.Load<Texture2D>("o"));
        }

        public override void Update(GameTime gameTime)
        {
            this.spin += ProjectileOnigiri.AngularVelocity;
            this.spin = this.spin % MathHelper.Pi * 2;
            this.sprite.Rotation = this.spin;
            base.Update(gameTime);
        }

        public override void Touches(IGameObject entity)
        {
            this.Damage(1);
            // TODO: Refactor the lower portion of this to make it accessible to all instances of IDamage.
            if (entity is IDamageable)
            {
                IDamageable target = entity as IDamageable;
                if (target.Faction != this.Faction)
                {
                    target.Damage(this.DamageOnHit);
                }
            }
        }
    }
}
