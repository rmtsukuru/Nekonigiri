using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    internal class Sprite
    {
        private IList<Texture2D> frames;
        private int framesPerSecond;
        private int currentFrame;
        private double remainingFrameTime;

        public Texture2D Texture
        {
            get
            {
                return frames[currentFrame];
            }
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public Vector2 Origin
        {
            get;
            set;
        }

        public float Rotation
        {
            get;
            set;
        }

        public SpriteEffects SpriteEffects
        {
            get;
            set;
        }

        public float LayerDepth
        {
            get;
            set;
        }

        public Sprite(Texture2D frame) : this(new List<Texture2D>(), 0)
        {
            this.frames.Add(frame);
        }

        public Sprite(IList<Texture2D> frames, int framerate)
        {
            this.frames = frames;
            this.framesPerSecond = framerate;
            this.ResetFrames();
            this.Position = Vector2.Zero;
            this.Scale = 1;
            this.Origin = Vector2.Zero;
            this.Rotation = 0;
            this.SpriteEffects = SpriteEffects.None;
            this.LayerDepth = 0.5f;
        }

        public void ResetFrames()
        {
            this.currentFrame = 0;
            this.ResetTimer();
        }

        private void ResetTimer()
        {
            this.remainingFrameTime = 1.0 / this.framesPerSecond;
        }

        public void Update(GameTime gameTime)
        {
            this.remainingFrameTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (this.remainingFrameTime <= 0)
            {
                this.currentFrame++;
                this.currentFrame %= this.frames.Count;
                this.ResetTimer();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(this.Texture, this.Position, null, Color.White, this.Rotation, this.Origin, this.Scale, this.SpriteEffects, this.LayerDepth);
        }

    }
}
