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

        // TODO: Get rid of "player" in all of these freaking names.
        const float StartingX = 300;
        const float StartingY = 130;
        private const int StartingMaxHealth = 100;
        private const int StartingMaxOnigiri = 50;

        private const int OnigiriHealAmount = 10;

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

        public Rectangle playerHitbox;
        bool isFacingRight;
        bool playerMoving;
        bool playerRunning;
        bool playerJumping;
        bool playerFalling;
        double remainingJumpTime;

        public int OnigiriCount
        {
            get;
            set;
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

            this.Position = new Vector2(StartingX, StartingY);
            this.Velocity = Vector2.Zero;

            this.isFacingRight = false;
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

        public override void Touches(IGameObject entity)
        {
            if (entity is HealthPack)
            {
                this.Heal((entity as HealthPack).HealAmount);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Input and Motion
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                float movementSpeed = playerRunning ? PlayerMovementSpeed * PlayerRunSpeedMultiplier : PlayerMovementSpeed;
                Velocity = new Vector2(movementSpeed, Velocity.Y);
                this.isFacingRight = true;
                this.playerMoving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                float movementSpeed = playerRunning ? PlayerMovementSpeed * PlayerRunSpeedMultiplier : PlayerMovementSpeed;
                Velocity = new Vector2(-1 * movementSpeed, Velocity.Y);
                this.isFacingRight = false;
                this.playerMoving = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                this.playerMoving = false;
                if (this.Velocity.X > 0)
                {
                    this.Velocity = new Vector2(Velocity.X - PlayerInertiaAcceleration, Velocity.Y);
                }
                else if (this.Velocity.X < 0)
                {
                    this.Velocity = new Vector2(Velocity.X + PlayerInertiaAcceleration, Velocity.Y);
                }

                if (Math.Abs(this.Velocity.X) < PlayerInertiaAcceleration)
                {
                    this.Velocity = new Vector2(0, Velocity.Y);
                }
            }

            // TODO: Get trigger input for this working.
            if ((Keyboard.GetState().IsKeyDown(Keys.Z) || Keyboard.GetState().IsKeyDown(Keys.Space)) && 
                ((!this.playerJumping && !this.playerFalling) || GameData.Instance.Debug))
            {
                this.playerJumping = true;
                this.remainingJumpTime = PlayerMaxJumpTime;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Z) && GameData.Instance.lastKeyboardState.IsKeyDown(Keys.Z) && this.playerJumping)
            {
                this.playerJumping = false;
                this.playerFalling = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && GameData.Instance.lastKeyboardState.IsKeyDown(Keys.Space) && this.playerJumping)
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

            if (Mouse.GetState().LeftButton.Equals(ButtonState.Pressed) && GameData.Instance.lastMouseState.LeftButton.Equals(ButtonState.Released))
            {
                if (this.OnigiriCount > 0)
                {
                    this.OnigiriCount--;
                    Vector2 displacement = new Vector2(Mouse.GetState().X - Position.X, Mouse.GetState().Y - Position.Y);
                    displacement.Normalize();
                    float speed = ProjectileOnigiri.BaseMovementSpeed;
                    Vector2 velocity = new Vector2(speed * displacement.X, speed * displacement.Y);
                    float x, y;
                    if (velocity.X > 0)
                    {
                        x = this.TranslatedHitbox.Right;
                    }
                    else
                    {
                        x = this.Position.X - ProjectileOnigiri.Width;
                    }
                    if (velocity.Y > 0)
                    {
                        y = this.TranslatedHitbox.Bottom;
                    }
                    else
                    {
                        y = this.Position.Y - ProjectileOnigiri.Height;
                    }
                    Vector2 pos = new Vector2(x, y);
                    pos = new Vector2(pos.X + Velocity.X, pos.Y + Velocity.Y);
                    GameData.Instance.CurrentLevel.AddObject(new ProjectileOnigiri(pos, velocity));
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X) && GameData.Instance.lastKeyboardState.IsKeyUp(Keys.X))
            {
                if (this.OnigiriCount > 0)
                {
                    this.OnigiriCount--;
                    Vector2 pos = this.isFacingRight ? new Vector2(this.TranslatedHitbox.Right, this.Position.Y) 
                                   : new Vector2(this.TranslatedHitbox.Left - ProjectileOnigiri.Width, this.Position.Y);
                    pos = new Vector2(pos.X + Velocity.X, pos.Y + Velocity.Y);
                    GameData.Instance.CurrentLevel.AddObject(new ProjectileOnigiri(pos, this.isFacingRight));
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C) && GameData.Instance.lastKeyboardState.IsKeyUp(Keys.C))
            {
                if (this.OnigiriCount > 0 && this.Health < this.MaxHealth)
                {
                    this.OnigiriCount--;
                    this.Heal(OnigiriHealAmount);
                }
            }

            // Debug commands
            if (GameData.Instance.Debug)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                {
                    this.Damage(-1);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                {
                    this.Damage(1);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    this.OnigiriCount = this.MaxOnigiri;
                }
            }

            // Jump Physics
            if (playerJumping)
            {
                Velocity = new Vector2(Velocity.X, -1 * PlayerJumpSpeed);
                // Moonwalk
                if (!GameData.Instance.Debug)
                {
                    this.remainingJumpTime -= gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (this.remainingJumpTime <= 0)
                {
                    this.playerJumping = false;
                    this.playerFalling = true;
                }
            }
            // Gravity
            else if (this.playerFalling)
            {
                if (Velocity.Y < PlayerFallSpeed)
                {
                    Velocity = new Vector2(Velocity.X, Velocity.Y + PlayerFallAcceleration);
                }
            }

            // Collision Detection
            Rectangle temp = playerHitbox;
            temp.Offset((int)Math.Ceiling(Position.X), (int)Math.Ceiling(Position.Y));
            Rectangle horizontalHitbox = temp, verticalHitbox = temp;
            horizontalHitbox.Offset((int)Math.Ceiling(Velocity.X), 0);
            verticalHitbox.Offset(0, (int)Math.Ceiling(Velocity.Y));
            foreach (IGameObject entity in GameData.Instance.CurrentLevel.ObjectsCloseTo(this))
            {
                if (entity.IsCollideable)
                {
                    if (entity.TranslatedHitbox.Intersects(verticalHitbox))
                    {
                        this.Touches(entity);
                        entity.Touches(this);
                        if (Velocity.Y >= 0)
                        {
                            this.playerFalling = false;
                        }
                        else
                        {
                            this.playerJumping = false;
                            this.playerFalling = true;
                        }
                        this.Velocity = new Vector2(this.Velocity.X, 0);
                    }
                    if (entity.TranslatedHitbox.Intersects(horizontalHitbox))
                    {
                        this.Touches(entity);
                        entity.Touches(this);
                        Velocity = new Vector2(0, this.Velocity.Y);
                    }
                }
            }

            base.Update(gameTime);

            // Set correct sprite
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

            this.sprite.Position = Position;
            this.sprite.SpriteEffects = this.isFacingRight ? SpriteEffects.FlipHorizontally
                                                                     : SpriteEffects.None;
            this.sprite.Update(gameTime);
        }

        public override void Destroy()
        {
            GameData.Instance.game.Exit();
            // TODO: Add game over/continue screen to switch to instead.
        }
    }
}
