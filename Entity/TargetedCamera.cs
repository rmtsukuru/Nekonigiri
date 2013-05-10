using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Implements a camera which tracks a specified object.
    /// </summary>
    internal class TargetedCamera : Camera
    {
        private const float Acceleration = .5f;

        private const float MaxVelocity = 10;

        /// <summary>
        /// The width, from the center of the screen, to the edge of the camera's focus.
        /// If the target moves out of the focus area, the camera will adjust velocity
        /// in order to track it.
        /// </summary>
        private const int FocusWidth = 150;

        /// <summary>
        /// The height, from the center of the screen, to the edge of the camera's focus.
        /// If the target moves out of the focus area, the camera will adjust velocity
        /// in order to track it.
        /// </summary>
        private const int FocusHeight = 100;

        private IGameObject target;

        public TargetedCamera(IGameObject target)
        {
            this.target = target;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 position = target.RelativePosition(this);
            if (position.X > OnigiriGame.ScreenCenter.X + FocusWidth)
            {
                this.Velocity = new Vector2(Math.Min(this.Velocity.X + Acceleration, MaxVelocity),
                                            this.Velocity.Y);
            }
            else if (position.X < OnigiriGame.ScreenCenter.Y - FocusWidth)
            {
                this.Velocity = new Vector2(Math.Max(this.Velocity.X - Acceleration, -1 * MaxVelocity),
                                            this.Velocity.Y);
            }
            // If not tracking target, decelerate.
            else if (Math.Abs(this.Velocity.X) > 0)
            {
                float x = (Math.Abs(this.Velocity.X) - Acceleration) * 
                           this.Velocity.X / Math.Abs(this.Velocity.X);
                this.Velocity = new Vector2(x, this.Velocity.Y);
            }

            if (position.Y > OnigiriGame.ScreenCenter.Y + FocusHeight)
            {
                this.Velocity = new Vector2(this.Velocity.X,
                                            Math.Min(this.Velocity.Y + Acceleration, MaxVelocity));
            }
            else if (position.Y < OnigiriGame.ScreenCenter.Y - FocusHeight)
            {
                this.Velocity = new Vector2(this.Velocity.X,
                                            Math.Max(this.Velocity.Y - Acceleration, -1 * MaxVelocity));
            }
            // If not tracking target, decelerate.
            else if (Math.Abs(this.Velocity.Y) > 0) 
            {
                float y = (Math.Abs(this.Velocity.Y) - Acceleration) *
                           this.Velocity.Y / Math.Abs(this.Velocity.Y);
                this.Velocity = new Vector2(this.Velocity.X, y);
            }

            base.Update(gameTime);

            if (this.Position.X < 0)
            {
                this.Position = new Vector2(0, this.Position.Y);
                this.Velocity = new Vector2(0, this.Velocity.Y);
            }
            else if (this.Position.X + OnigiriGame.WindowWidth > GameData.Instance.CurrentLevel.Width)
            {
                float x = GameData.Instance.CurrentLevel.Width - OnigiriGame.WindowWidth;
                this.Position = new Vector2(x, this.Position.Y);
                this.Velocity = new Vector2(0, this.Velocity.Y);
            }

            if (this.Position.Y < 0)
            {
                this.Position = new Vector2(this.Position.X, 0);
                this.Velocity = new Vector2(this.Velocity.X, 0);
            }
            else if (this.Position.Y + OnigiriGame.WindowHeight > GameData.Instance.CurrentLevel.Height)
            {
                float y = GameData.Instance.CurrentLevel.Height - OnigiriGame.WindowHeight;
                this.Position = new Vector2(this.Position.X, y);
                this.Velocity = new Vector2(this.Position.X, y);
            }
        }
    }
}
