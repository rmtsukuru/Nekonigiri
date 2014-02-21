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

        /// <summary>
        /// The width, from the center of the screen, to the edge of the camera's focus.
        /// If the target moves out of the focus area, the camera will adjust velocity
        /// in order to track it.
        /// </summary>
        private const int FocusWidth = 80;

        /// <summary>
        /// The height, from the center of the screen, to the edge of the camera's focus.
        /// If the target moves out of the focus area, the camera will adjust velocity
        /// in order to track it.
        /// </summary>
        private const int FocusHeight = 70;

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
                this.Position = new Vector2(target.Position.X - OnigiriGame.WindowWidth / 2 - FocusWidth,
                                            this.Position.Y);
            }
            else if (position.X < OnigiriGame.ScreenCenter.X - FocusWidth)
            {
                this.Position = new Vector2(target.Position.X - OnigiriGame.WindowWidth / 2 + FocusWidth,
                                            this.Position.Y);
            }

            if (position.Y > OnigiriGame.ScreenCenter.Y + FocusHeight)
            {
                this.Position = new Vector2(this.Position.X, target.Position.Y - 
                                            OnigiriGame.WindowHeight / 2 - FocusHeight);
            }
            else if (position.Y < OnigiriGame.ScreenCenter.Y - FocusHeight)
            {
                this.Position = new Vector2(this.Position.X, target.Position.Y -
                                            OnigiriGame.WindowHeight / 2 + FocusHeight);
            }

            base.Update(gameTime);

            if (this.Position.X < 0)
            {
                this.Position = new Vector2(0, this.Position.Y);
            }
            else if (this.Position.X + OnigiriGame.WindowWidth > GameData.Instance.CurrentLevel.Width)
            {
                float x = GameData.Instance.CurrentLevel.Width - OnigiriGame.WindowWidth;
                this.Position = new Vector2(x, this.Position.Y);
            }

            if (this.Position.Y < 0)
            {
                this.Position = new Vector2(this.Position.X, 0);
            }
            else if (this.Position.Y + OnigiriGame.WindowHeight > GameData.Instance.CurrentLevel.Height)
            {
                float y = GameData.Instance.CurrentLevel.Height - OnigiriGame.WindowHeight;
                this.Position = new Vector2(this.Position.X, y);
            }
        }
    }
}
