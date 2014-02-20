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

        private const int LevelWidth = 1920;
        private const int LevelHeight = 480;

        internal const bool Admin = true; // Determines whether debug mode can be turned on and off.
        internal const bool Debug = false;

        const int HudIconPadding = 10;

        internal static Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(WindowWidth / 2, WindowHeight / 2);
            }
        }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Sprite backgroundSprite;
        private SpriteFont hudFont;
        private Texture2D hpBarSprite;
        private Texture2D hpBarUnderlay;
        private Texture2D fullHpBarOverlay;
        private Texture2D highHpBarOverlay;
        private Texture2D mediumHpBarOverlay;
        private Texture2D lowHpBarOverlay;
        private Texture2D currentHpBarOverlay;
        private Sprite onigiriIconSprite;
        private Texture2D cursorImage;

        private Neko player;
        private ICamera camera;

        private IList<IGameObject> gameObjects;

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
            GameData.Instance.Debug = Debug;
            GameData.Instance.Content = this.Content;
            GameData.Instance.game = this;
            GameData.Instance.lastKeyboardState = Keyboard.GetState();
            GameData.Instance.lastMouseState = Mouse.GetState();
            GameData.Instance.CurrentLevel = this;

            this.player = new Neko();
            this.camera = new TargetedCamera(this.player);
            this.player.camera = this.camera;

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
            this.cursorImage = Content.Load<Texture2D>("cursor");

            IList<IGameObject> tiles = LevelMap.LoadTiles(FileLoader.ReadAllText("testlevel.txt"), Tileset.GetDefaultTileset());
            foreach (IGameObject tile in tiles)
            {
                this.gameObjects.Add(tile);
            }

            // TODO: Load this dynamically from map spec file.
            this.Width = OnigiriGame.LevelWidth;
            this.Height = OnigiriGame.LevelHeight;

            this.gameObjects.Add(this.player);

            IList<IGameObject> entities = LevelMap.LoadEntities(FileLoader.LoadXml("testlevel.xml"));
            foreach (IGameObject entity in entities)
            {
                this.gameObjects.Add(entity);
            }

            // Level boundaries
            this.gameObjects.Add(new BlackHole(new Rectangle(0, LevelHeight+70, LevelWidth, 1)));
            this.gameObjects.Add(new InvisibleWall(new Rectangle(-1, -300, 1, LevelHeight + 300 + 70)));
            this.gameObjects.Add(new InvisibleWall(new Rectangle(LevelWidth, -300, 1, LevelHeight + 300 + 70)));
            this.gameObjects.Add(new BlackHole(new Rectangle(0, -301, LevelWidth, 1)));
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

            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && 
                GameData.Instance.lastKeyboardState.IsKeyUp(Keys.OemTilde) && Admin)
            {
                GameData.Instance.Debug = !GameData.Instance.Debug;
            }

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

            this.camera.Update(gameTime);

            GameData.Instance.lastKeyboardState = Keyboard.GetState();
            GameData.Instance.lastMouseState = Mouse.GetState();

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
            this.player.Draw(spriteBatch, gameTime, camera);
            foreach (IGameObject entity in this.gameObjects)
            {
                entity.Draw(spriteBatch, gameTime, camera);
            }
            this.DrawCursor(spriteBatch, gameTime);
            this.DrawHUD(spriteBatch, gameTime);
            if (GameData.Instance.Debug)
            {
                this.DrawDebugHUD(spriteBatch, gameTime);
            }

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

        private void DrawDebugHUD(SpriteBatch spriteBatch, GameTime gameTime)
        {
            string s = "Camera X: " + camera.Position.X + " Y: " + camera.Position.Y;
            spriteBatch.DrawString(hudFont, s, new Vector2(0, 460), Color.Crimson);
        }

        private void DrawBar(SpriteBatch spriteBatch, Texture2D barSprite, Texture2D overlay, Texture2D underlay, Rectangle rect, double fillPercent, Vector2 overlayPos)
        {
            spriteBatch.Draw(underlay, overlayPos, Color.White);
            Rectangle percentFill = new Rectangle(rect.X, rect.Y, (int) (rect.Width * fillPercent), rect.Height);
            spriteBatch.Draw(barSprite, percentFill, Color.White);
            spriteBatch.Draw(overlay, overlayPos, Color.White);
        }

        private void DrawCursor(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(this.cursorImage, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White); 
        }

        #region ILevel Members
        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
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
        #endregion
    }
}
