using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekonigiri
{
    /// <summary>
    /// Represents a tileset. This includes the image containing the tiles
    /// themselves, and a mapping from each tile to determine its passability
    /// (I.E. whether it is collideable). 
    /// 
    /// Passability is determined as follows: -1 gives a totally passable tile,
    /// 0 gives a totally impassable tile, any higher value gives a tile with
    /// some degree of passability in-between those two extremes, as defined
    /// in <code>TileSet.GetPassabilityHitbox()</code>.
    /// </summary>
    internal class Tileset
    {
        private Texture2D bitmap;
        private Vector2 tileSize;
        private int[][] passabilityMapping;

        /// <summary>
        /// Instantiates a new tileset with the specified properties.
        /// </summary>
        /// <param name="bitmapFilename">Filename of bitmap, used to load
        /// from content pipeline.</param>
        /// <param name="passabilityMapping">2D array of numbers specifying
        /// passability of each title. Must be rectangular and have the correct
        /// number of tiles.</param>
        public Tileset(string bitmapFilename, int[][] passabilityMapping)
        {
            this.bitmap = GameData.Instance.Content.Load<Texture2D>(bitmapFilename);
            this.passabilityMapping = passabilityMapping;
            this.tileSize = new Vector2(this.bitmap.Width / passabilityMapping.Length,
                                  this.bitmap.Height / passabilityMapping[0].Length);
        }

        public Tile GetTile(int x, int y)
        {
            Rectangle sourceRect = new Rectangle(x * (int)tileSize.X, y * (int)tileSize.Y, (int)tileSize.X, (int)tileSize.Y);
            return new Tile(bitmap, sourceRect, GetPassabilityHitbox(passabilityMapping[x][y], tileSize));
        }

        public Tile GetTile(Vector2 pos)
        {
            return GetTile((int)pos.X, (int)pos.Y);
        }

        /// <summary>
        /// Returns the hitbox associated with the specified passability.
        /// </summary>
        /// <param name="passability">The passability value to check.</param>
        /// <returns>A hitbox, or null if there is no hitbox associated with 
        /// this passability.</returns>
        public static Rectangle? GetPassabilityHitbox(int passability, Vector2 tileSize)
        {
            switch (passability)
            {
                case -1:
                    return null;

                case 0:
                    return new Rectangle(0, 0, (int)tileSize.X, (int)tileSize.Y);

                default:
                    return new Rectangle(0, 0, (int)tileSize.X, (int)tileSize.Y);
            }
        }
    }
}
