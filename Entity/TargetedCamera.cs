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
    internal class TargetedCamera : ICamera
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

        Vector2 Position
        {
            get;
            private set;
        }

        Vector2 Velocity
        {
            get;
            private set;
        }

        TargetedCamera(IGameObject target)
        {
            this.target = target;
        }

        void Update(GameTime gameTime)
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

            this.Position = new Vector2(this.Position.X + this.Velocity.X, 
                                        this.Position.Y + this.Velocity.Y);
        }
    }
}
