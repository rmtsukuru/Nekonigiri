using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    internal abstract class Camera : ICamera
    {
        public Vector2 Position
        {
            get;
            protected set;
        }

        public Vector2 Velocity
        {
            get;
            protected set;
        }

        public virtual void Update(GameTime gameTime)
        {
            this.Position = new Vector2(this.Position.X + this.Velocity.X,
                                        this.Position.Y + this.Velocity.Y);
        }

        public virtual Vector2 absolutePosition(Vector2 relativePosition)
        {
            return this.Position + relativePosition;
        }

        public virtual Vector2 absolutePosition(float x, float y)
        {
            return this.absolutePosition(new Vector2(x, y)); 
        }
    }
}
