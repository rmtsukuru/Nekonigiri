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
    internal class OnigiriGame : Microsoft.Xna.Framework.Game, ILevel
    {
        internal const int WindowWidth = 640;
        internal const int WindowHeight = 480;

        const int HudIconPadding = 10;

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

        Neko player;

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
            GameData.Instance.lastKeyboardState = Keyboard.GetState();
            GameData.Instance.CurrentLevel = this;

            this.player = new Neko();

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

            this.gameObjects.Add(this.player);
            this.gameObjects.Add(new Block(300, 432));
            this.gameObjects.Add(new Block(180, 432));
            this.gameObjects.Add(new Block(500, 432));
            this.gameObjects.Add(new Block(548, 432));
            this.gameObjects.Add(new Block(380, 300));
            this.gameObjects.Add(new Block(240, 180));
            this.gameObjects.Add(new Block(50, 50));
            this.gameObjects.Add(new HealthPack(new Vector2(245, 147)));

            // Level boundaries
            this.gameObjects.Add(new InvisibleWall(new Rectangle(0, WindowHeight, WindowWidth, 1)));
            this.gameObjects.Add(new InvisibleWall(new Rectangle(-1, -300, 1, WindowHeight + 300)));
            this.gameObjects.Add(new InvisibleWall(new Rectangle(WindowWidth, -300, 1, WindowHeight + 300)));
            this.gameObjects.Add(new InvisibleWall(new Rectangle(0, -301, WindowWidth, 1)));
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

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(gameTime);

                // Check if this object is touching any other(s) and evaluate accordingly.
                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].TranslatedHitbox.Intersects(gameObjects[j].TranslatedHitbox))
                    {
                        gameObjects[i].Touches(gameObjects[j]);
                        gameObjects[j].Touches(gameObjects[i]);
                    }
                }
                if (gameObjects[i].Destroyed)
                {
                    gameObjects.RemoveAt(i);
                    i--; // Reset index so that next entry is not skipped.
                }
            }

            GameData.Instance.lastKeyboardState = Keyboard.GetState();

            this.UpdateHUD(gameTime);

            base.Update(gameTime);
        }

        private void UpdateHUD(GameTime gameTime)
        {
            double healthPercent = (double)this.player.Health / this.player.MaxHealth;
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
            this.player.Draw(spriteBatch, gameTime);
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
            this.DrawBar(spriteBatch, hpBarSprite, currentHpBarOverlay, hpBarUnderlay, hpRect, (double)player.Health / player.MaxHealth, Vector2.Zero);
            Vector2 iconPos = new Vector2(currentHpBarOverlay.Width + HudIconPadding, 0);
            spriteBatch.Draw(onigiriIconSprite.Texture, iconPos, Color.White);
            string s = player.OnigiriCount + "/" + player.MaxOnigiri;
            spriteBatch.DrawString(hudFont, s, new Vector2(iconPos.X + onigiriIconSprite.Texture.Width + HudIconPadding, 0), Color.White);
        }

        private void DrawBar(SpriteBatch spriteBatch, Texture2D barSprite, Texture2D overlay, Texture2D underlay, Rectangle rect, double fillPercent, Vector2 overlayPos)
        {
            spriteBatch.Draw(underlay, overlayPos, Color.White);
            Rectangle percentFill = new Rectangle(rect.X, rect.Y, (int) (rect.Width * fillPercent), rect.Height);
            spriteBatch.Draw(barSprite, percentFill, Color.White);
            spriteBatch.Draw(overlay, overlayPos, Color.White);
        }

        public IList<IGameObject> ObjectsCloseTo(IGameObject entity)
        {
            return gameObjects;
            // TODO: Refactor this so it actually just gets nearby objects and is thus efficient.
        }

        public void AddObject(IGameObject entity)
        {
            this.gameObjects.Add(entity);
        }
    }
}
