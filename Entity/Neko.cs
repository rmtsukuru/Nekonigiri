using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nekonigiri
{
    internal class Neko : Damageable
    {
        private const int SpriteFramerate = 5;

        readonly Rectangle DefaultPlayerHitbox = new Rectangle(13, 0, 39, 64);

        const float PlayerStartingX = 300;
        const float PlayerStartingY = 250;
        private const int StartingMaxHealth = 100;
        private const int StartingMaxOnigiri = 50;

        const float PlayerMovementSpeed = 4;
        const float PlayerInertiaAcceleration = 0.3f;
        const float PlayerRunSpeedMultiplier = 2;
        const float PlayerFallSpeed = 8;
        const float PlayerFallAcceleration = 0.7f;
        const float PlayerMaxJumpTime = .3f;
        const float PlayerJumpSpeed = 8;

        private static Sprite standingSprite;
        private static Sprite walkingSprite;
        private static Sprite jumpingSprite;

        Vector2 playerPos;
        Vector2 playerVelocity;
        public Rectangle playerHitbox;
        bool playerFacingRight;
        bool playerMoving;
        bool playerRunning;
        bool playerJumping;
        bool playerFalling;
        double remainingJumpTime;

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

            this.Position = new Vector2(PlayerStartingX, PlayerStartingY);

            this.playerFacingRight = false;
            this.playerMoving = false;
            this.playerRunning = false;
            this.playerJumping = false;
            this.playerFalling = true;
            this.remainingJumpTime = 0;
            this.playerHitbox = DefaultPlayerHitbox;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                float movementSpeed = playerRunning ? PlayerMovementSpeed * PlayerRunSpeedMultiplier : PlayerMovementSpeed;
                playerVelocity.X = movementSpeed;
                this.playerFacingRight = true;
                this.playerMoving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                float movementSpeed = playerRunning ? PlayerMovementSpeed * PlayerRunSpeedMultiplier : PlayerMovementSpeed;
                playerVelocity.X = -1 * movementSpeed;
                this.playerFacingRight = false;
                this.playerMoving = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                this.playerMoving = false;
                if (this.playerVelocity.X > 0)
                {
                    this.playerVelocity.X -= PlayerInertiaAcceleration;
                }
                else if (this.playerVelocity.X < 0)
                {
                    this.playerVelocity.X += PlayerInertiaAcceleration;
                }

                if (Math.Abs(this.playerVelocity.X) < PlayerInertiaAcceleration)
                {
                    this.playerVelocity.X = 0;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !this.playerJumping && !this.playerFalling)
            {
                this.playerJumping = true;
                this.remainingJumpTime = PlayerMaxJumpTime;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Z) && this.playerJumping)
            {
                this.playerJumping = false;
                this.playerFalling = true;
            }
            if (!this.playerJumping)
            {
                this.playerFalling = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                this.playerRunning = true;
            }
            else
            {
                this.playerRunning = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                this.Damage(-1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                this.Damage(1);
            }

            if (playerJumping)
            {
                playerVelocity.Y = -1 * PlayerJumpSpeed;
                this.remainingJumpTime -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this.remainingJumpTime <= 0)
                {
                    this.playerJumping = false;
                    this.playerFalling = true;
                }
            }
            // Gravity
            else if (this.playerFalling)
            {
                if (playerVelocity.Y < PlayerFallSpeed)
                {
                    playerVelocity.Y += PlayerFallAcceleration;
                }
            }

            Rectangle temp = playerHitbox;
            temp.Offset((int)Math.Ceiling(playerPos.X), (int)Math.Ceiling(playerPos.Y));
            Rectangle horizontalHitbox = temp, verticalHitbox = temp;
            horizontalHitbox.Offset((int)Math.Ceiling(playerVelocity.X), 0);
            verticalHitbox.Offset(0, (int)Math.Ceiling(playerVelocity.Y));
            foreach (IGameObject entity in GameData.Instance.Level.objectsCloseTo(this))
            {
                entity.Update(gameTime);

                if (entity.IsCollideable)
                {
                    if (entity.TranslatedHitbox.Intersects(verticalHitbox))
                    {
                        if (playerVelocity.Y >= 0)
                        {
                            this.playerFalling = false;
                        }
                        else
                        {
                            this.playerJumping = false;
                            this.playerFalling = true;
                        }
                        this.playerVelocity.Y = 0;
                    }
                    if (entity.TranslatedHitbox.Intersects(horizontalHitbox))
                    {
                        playerVelocity.X = 0;
                    }
                }
            }

            playerPos = new Vector2(playerPos.X + playerVelocity.X, playerPos.Y + playerVelocity.Y);

            if (playerPos.Y + this.sprite.Texture.Height > OnigiriGame.WindowHeight)
            {
                playerPos.Y = OnigiriGame.WindowHeight - this.sprite.Texture.Height;
                this.playerFalling = false;
            }
            if (playerPos.X + playerHitbox.X < 0)
            {
                playerPos.X = -1 * playerHitbox.X;
            }
            if (playerPos.X + playerHitbox.Width > OnigiriGame.WindowWidth)
            {
                playerPos.X = OnigiriGame.WindowWidth - playerHitbox.Width;
            }

            if (this.playerMoving && this.sprite != walkingSprite)
            {
                this.sprite = walkingSprite;
                this.sprite.ResetFrames();
            }
            else if (!this.playerMoving && this.sprite != standingSprite)
            {
                this.sprite = standingSprite;
                this.sprite.ResetFrames();
            }
            if (this.playerJumping || this.playerFalling && this.sprite != jumpingSprite)
            {
                this.sprite = jumpingSprite;
                this.sprite.ResetFrames();
            }

            if (this.OnigiriCount < 0)
            {
                this.OnigiriCount = 0;
            }
            else if (this.OnigiriCount > this.MaxOnigiri)
            {
                this.OnigiriCount = this.MaxOnigiri;
            }

            this.sprite.Position = playerPos;
            this.sprite.SpriteEffects = this.playerFacingRight ? SpriteEffects.FlipHorizontally
                                                                     : SpriteEffects.None;
            this.sprite.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Destroy()
        {
            GameData.Instance.game.Exit();
            // TODO: Add game over/continue screen to switch to instead.
        }
    }
}
