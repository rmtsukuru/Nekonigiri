using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class Neko : Damageable
    {
        private const int SpriteFramerate = 5;

        private const int StartingMaxHealth = 100;
        private const int StartingMaxOnigiri = 50;

        private static Sprite standingSprite;
        private static Sprite walkingSprite;
        private static Sprite jumpingSprite;

        public int OnigiriCount
        {
            get;
            private set;
        }

        public int MaxOnigiri
        {
            get;
            private set;
        }

        public Neko()
            : this(StartingMaxHealth, StartingMaxOnigiri)
        {
        }

        public Neko(int maxHP, int maxOnigiri)
            : this(maxHP, maxHP, maxOnigiri, maxOnigiri)
        {
        }

        public Neko(int maxHP, int currentHP, int maxOnigiri)
            : this(maxHP, currentHP, maxOnigiri, maxOnigiri)
        {
        }

        public Neko(int maxHP, int currentHP, int maxOnigiri, int currentOnigiri)
            : base (Neko.FetchSprite(), Faction.Player, maxHP, currentHP, false)
        {
            this.MaxOnigiri = maxOnigiri;
            this.OnigiriCount = currentOnigiri;
        }

        private static Sprite FetchSprite()
        {
            if (Neko.standingSprite == null)
            {
                ContentManager content = GameData.Instance.Content;
                standingSprite = new Sprite(content.Load<Texture2D>("stand"));
                IList<Texture2D> spriteFrames = new List<Texture2D>();
                spriteFrames.Add(content.Load<Texture2D>("pix"));
                spriteFrames.Add(content.Load<Texture2D>("pixxx"));
                spriteFrames.Add(content.Load<Texture2D>("pixx"));
                spriteFrames.Add(content.Load<Texture2D>("pixxx"));
                walkingSprite = new Sprite(spriteFrames, SpriteFramerate);
                jumpingSprite = new Sprite(content.Load<Texture2D>("j"));
            }

            return Neko.standingSprite;
        }

        public override void Touches(GameObject entity)
        {
            // TODO: Put player collision-y stuff here.
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Move player update logic here.

            base.Update(gameTime);
        }

        public override void Destroy()
        {
            GameData.Instance.game.Exit();
            // TODO: Add game over/continue screen to switch to instead.
        }
    }
}
