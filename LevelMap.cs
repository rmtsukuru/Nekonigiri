using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    internal class LevelMap
    {
        public static IList<IGameObject> LoadTiles(String tilespec, Tileset tileset)
        {
            string[] lines = tilespec.Split(new string[]{Environment.NewLine}, StringSplitOptions.None);
            int[][] tiles = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                tiles[i] = new int[lines[i].Length];
                for (int j = 0; j < lines[i].Length; j++)
                {
                    tiles[i][j] = MapCharToTileValue(lines[i][j]);
                }
            }
            return LoadTiles(tiles, tileset);
        }

        public static IList<IGameObject> LoadTiles(int[][] tilespec, Tileset tileset)
        {
            IList<IGameObject> tiles = new List<IGameObject>(tilespec.Length * tilespec[0].Length);
            for (int i = 0; i < tilespec.Length; i++)
            {
                for (int j = 0; j < tilespec[i].Length; j++)
                {
                    tiles.Add(LoadTile(tilespec[i][j], tileset, new Vector2(tilespec.Length, 
                                                                       tilespec[0].Length)));
                }
            }
            return tiles;
        }

        private static IGameObject LoadTile(int tileNumber, Tileset tileset, Vector2 mapSize)
        {
            int x = tileNumber % (int)mapSize.X;
            int y = tileNumber / (int)mapSize.X;
            return tileset.GetTile(x, y);
        }

        /// <summary>
        /// Maps the specified character to its corresponding tile number.
        /// </summary>
        /// <param name="ch">The specified character.</param>
        /// <returns>The tile value corresponding to the specified character.</returns>
        public static int MapCharToTileValue(char ch)
        {
            // Map space to -1.
            if (ch == ' ')
            {
                return -1;
            }
            // Map A-Z to 0-25.
            else if (ch >= 'A' && ch <= 'Z') 
            {
                return ch - 'A';
            }
            // Map a-z to 26-51.
            else if (ch >= 'a' && ch <= 'z')
            {
                return ch - 'a' + 26;
            }
            // Map !-@ (from ASCII table - this includes numbers) to 52-83.
            else if (ch >= '!' && ch <= '@')
            {
                return ch - '!' + 52;
            }
            // Default to 0.
            else
            {
                return 0;
            }
        }
    }
}
