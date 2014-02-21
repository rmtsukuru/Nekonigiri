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
        public static readonly float Height = ProjectileOnigiri.FetchSprite().Texture.Height;

        internal const float BaseMovementSpeed = 5;
        private const float AngularVelocity = 0.01f; // In radians - about 20 degrees.
        private const int DamageValue = 10;

        private float spin;

        public int DamageOnHit
        {
            get { return ProjectileOnigiri.DamageValue; }
        }

        public ProjectileOnigiri(Vector2 position, bool isFacingRight)
            : this(position, FetchVelocityFromDirection(isFacingRight))
        {
            
        }

        public ProjectileOnigiri(Vector2 position, Vector2 velocity)
            : base(FetchSprite(), Faction.Player, 1, true)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.spin = 0;
            this.sprite.Origin = new Vector2(this.sprite.Texture.Width / 2, this.sprite.Texture.Height / 2);
            Vector2 offset = Vector2.Multiply(this.sprite.Origin, -1);
            this.Hitbox = new Rectangle((int)offset.X, (int)offset.Y, this.sprite.Texture.Width, 
                                                            this.sprite.Texture.Height);
        }

        private static Vector2 FetchVelocityFromDirection(bool isFacingRight)
        {
            if (isFacingRight)
            {
                return new Vector2(BaseMovementSpeed, 0);
            }
            else
            {
                return new Vector2(-1 * BaseMovementSpeed, 0);
            }
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

            if (entity is Neko)
            {
                (entity as Neko).OnigiriCount++;
            }
        }
    }
}
