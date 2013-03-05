using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Nekonigiri
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OnigiriGame : Microsoft.Xna.Framework.Game
    {
        const int WindowWidth = 640;
        const int WindowHeight = 480;

        const int HudIconPadding = 10;

        const int PlayerSpriteFramerate = 5;

        readonly Rectangle DefaultPlayerHitbox = new Rectangle(13, 0, 39, 64);

        const float PlayerStartingX = 300;
        const float PlayerStartingY = 250;
        const int PlayerStartingMaxHealth = 100;
        const int PlayerStartingMaxOnigiri = 50;

        const float PlayerMovementSpeed = 4;
        const float PlayerInertiaAcceleration = 0.3f;
        const float PlayerRunSpeedMultiplier = 2;
        const float PlayerFallSpeed = 8;
        const float PlayerFallAcceleration = 0.7f;
        const float PlayerMaxJumpTime = .3f;
        const float PlayerJumpSpeed = 8;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Sprite backgroundSprite;
        SpriteFont hudFont;
        Texture2D hpBarSprite;
        Texture2D hpBarUnderlay;
        Texture2D fullHpBarOverlay;
        Texture2D highHpBarOverlay;
        Texture2D mediumHpBarOverlay;
        Texture2D lowHpBarOverlay;
        Texture2D currentHpBarOverlay;
        Sprite onigiriIconSprite;
        Sprite currentPlayerSprite;
        Sprite playerStandingSprite;
        Sprite playerWalkingSprite;
        Sprite playerJumpingSprite;

        Vector2 playerPos;
        Vector2 playerVelocity;
        Rectangle playerHitbox;
        int playerHealth;
        int playerMaxHealth;
        int playerOnigiriCount;
        int playerMaxOnigiri;
        bool playerFacingRight;
        bool playerMoving;
        bool playerRunning;
        bool playerJumping;
        bool playerFalling;
        double remainingJumpTime;

        IList<IGameObject> gameObjects;

        public OnigiriGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameData.Instance.Content = this.Content;
            GameData.Instance.game = this;
            
            this.playerPos = new Vector2(PlayerStartingX, PlayerStartingY);
            this.playerVelocity = Vector2.Zero;
            this.playerHitbox = DefaultPlayerHitbox;
            this.playerMaxHealth = PlayerStartingMaxHealth;
            this.playerHealth = this.playerMaxHealth;
            this.playerMaxOnigiri = PlayerStartingMaxOnigiri;
            this.playerOnigiriCount = this.playerMaxOnigiri;
            this.playerFacingRight = false;
            this.playerMoving = false;
            this.playerRunning = false;
            this.playerJumping = false;
            this.playerFalling = true;
            this.remainingJumpTime = 0;

            this.gameObjects = new List<IGameObject>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.backgroundSprite = new Sprite(Content.Load<Texture2D>("background"));
            this.hudFont = Content.Load<SpriteFont>("HUDFont");
            this.hpBarSprite = Content.Load<Texture2D>("bar_fill");
            this.hpBarUnderlay = Content.Load<Texture2D>("bar_underlay");
            this.fullHpBarOverlay = Content.Load<Texture2D>("bar11");
            this.highHpBarOverlay = Content.Load<Texture2D>("bar21");
            this.mediumHpBarOverlay = Content.Load<Texture2D>("bar31");
            this.lowHpBarOverlay = Content.Load<Texture2D>("bar41");
            this.onigiriIconSprite = new Sprite(Content.Load<Texture2D>("o"));

            this.playerStandingSprite = new Sprite(Content.Load<Texture2D>("stand"));
            IList<Texture2D> spriteFrames = new List<Texture2D>();
            spriteFrames.Add(Content.Load<Texture2D>("pix"));
            spriteFrames.Add(Content.Load<Texture2D>("pixxx"));
            spriteFrames.Add(Content.Load<Texture2D>("pixx"));
            spriteFrames.Add(Content.Load<Texture2D>("pixxx"));
            this.playerWalkingSprite = new Sprite(spriteFrames, PlayerSpriteFramerate);
            this.playerJumpingSprite = new Sprite(Content.Load<Texture2D>("j"));
            this.currentPlayerSprite = this.playerStandingSprite;

            this.gameObjects.Add(new Block(300, 432));
            this.gameObjects.Add(new Block(180, 432));
            this.gameObjects.Add(new Block(500, 432));
            this.gameObjects.Add(new Block(548, 432));
            this.gameObjects.Add(new Block(380, 300));
            this.gameObjects.Add(new Block(240, 180));
            this.gameObjects.Add(new Block(50, 50));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

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
                this.playerHealth++;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                this.playerHealth--;
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
            foreach (IGameObject entity in this.gameObjects)
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

            if (playerPos.Y + this.currentPlayerSprite.Texture.Height > WindowHeight)
            {
                playerPos.Y = WindowHeight - this.currentPlayerSprite.Texture.Height;
                this.playerFalling = false;
            }
            if (playerPos.X + playerHitbox.X < 0)
            {
                playerPos.X = -1 * playerHitbox.X;
            }
            if (playerPos.X + playerHitbox.Width > WindowWidth)
            {
                playerPos.X = WindowWidth - playerHitbox.Width;
            }

            if (this.playerMoving && this.currentPlayerSprite != playerWalkingSprite)
            {
                this.currentPlayerSprite = this.playerWalkingSprite;
                this.currentPlayerSprite.ResetFrames();
            }
            else if (!this.playerMoving && this.currentPlayerSprite != playerStandingSprite)
            {
                this.currentPlayerSprite = this.playerStandingSprite;
                this.currentPlayerSprite.ResetFrames();
            }
            if (this.playerJumping || this.playerFalling && this.currentPlayerSprite != playerJumpingSprite)
            {
                this.currentPlayerSprite = this.playerJumpingSprite;
                this.currentPlayerSprite.ResetFrames();
            }

            if (this.playerHealth <= 0)
            {
                this.Exit();
                // TODO: Add game over/continue screen to switch to instead.
            }
            else if (this.playerHealth > this.playerMaxHealth)
            {
                this.playerHealth = this.playerMaxHealth;
            }

            if (this.playerOnigiriCount < 0)
            {
                this.playerOnigiriCount = 0;
            }
            else if (this.playerOnigiriCount > this.playerMaxOnigiri)
            {
                this.playerOnigiriCount = this.playerMaxOnigiri;
            }

            this.currentPlayerSprite.Position = playerPos;
            this.currentPlayerSprite.SpriteEffects = this.playerFacingRight ? SpriteEffects.FlipHorizontally
                                                                     : SpriteEffects.None;
            this.currentPlayerSprite.Update(gameTime);

            this.UpdateHUD(gameTime);

            base.Update(gameTime);
        }

        private void UpdateHUD(GameTime gameTime)
        {
            double healthPercent = (double)this.playerHealth / this.playerMaxHealth;
            if (healthPercent > .75)
            {
                currentHpBarOverlay = fullHpBarOverlay;
            }
            else if (healthPercent > .5)
            {
                currentHpBarOverlay = highHpBarOverlay;
            }
            else if (healthPercent > .25)
            {
                currentHpBarOverlay = mediumHpBarOverlay;
            }
            else
            {
                currentHpBarOverlay = lowHpBarOverlay;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Chocolate);

            spriteBatch.Begin();
            
            this.DrawBackground(spriteBatch, gameTime);
            this.currentPlayerSprite.Draw(spriteBatch, gameTime);
            foreach (IGameObject entity in this.gameObjects)
            {
                entity.Draw(spriteBatch, gameTime);
            }
            this.DrawHUD(spriteBatch, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle screen = new Rectangle(0, 0, WindowWidth, WindowHeight);
            spriteBatch.Draw(backgroundSprite.Texture, screen, Color.White);
        }

        private void DrawHUD(SpriteBatch spriteBatch, GameTime gameTime)
        {
            /*string s = "HP: " + playerHealth + "/" + playerMaxHealth;
            spriteBatch.DrawString(hudFont, s, Vector2.Zero, Color.Turquoise);
            Vector2 slen = hudFont.MeasureString(s);*/
            Rectangle hpRect = new Rectangle(69, 20, 110, 10);
            this.DrawBar(spriteBatch, hpBarSprite, currentHpBarOverlay, hpBarUnderlay, hpRect, (double)playerHealth / playerMaxHealth, Vector2.Zero);
            Vector2 iconPos = new Vector2(currentHpBarOverlay.Width + HudIconPadding, 0);
            spriteBatch.Draw(onigiriIconSprite.Texture, iconPos, Color.White);
            string s = playerOnigiriCount + "/" + playerMaxOnigiri;
            spriteBatch.DrawString(hudFont, s, new Vector2(iconPos.X + onigiriIconSprite.Texture.Width + HudIconPadding, 0), Color.White);
        }

        private void DrawBar(SpriteBatch spriteBatch, Texture2D barSprite, Texture2D overlay, Texture2D underlay, Rectangle rect, double fillPercent, Vector2 overlayPos)
        {
            spriteBatch.Draw(underlay, overlayPos, Color.White);
            Rectangle percentFill = new Rectangle(rect.X, rect.Y, (int) (rect.Width * fillPercent), rect.Height);
            spriteBatch.Draw(barSprite, percentFill, Color.White);
            spriteBatch.Draw(overlay, overlayPos, Color.White);
        }
    }
}
