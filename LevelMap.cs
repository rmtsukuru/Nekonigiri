using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Nekonigiri
{
    internal class LevelMap
    {
        private static Dictionary<string, Type> entityTypeIdentifiers;

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
                    IGameObject tile = LoadTile(tilespec[i][j], tileset);
                    if (tile != null)
                    {
                        tile.Position = new Vector2(j * tileset.TileSize.X, i * tileset.TileSize.Y);
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }

        public static IList<IGameObject> LoadEntities(XmlDocument level)
        {
            IList<IGameObject> entities = new List<IGameObject>(level.FirstChild.ChildNodes.Count);

            foreach (XmlNode node in level.FirstChild.ChildNodes)
            {
                switch (node.Name)
                {
                    case "entity":
                        String typeNamespace = "Nekonigiri";
                        Type type = null;
                        List<Type> parameterTypes = new List<Type>();
                        parameterTypes.Add(typeof(Vector2)); // Position
                        List<Object> parameters = new List<Object>();
                        parameters.Add(null);
                        Vector2 position = Vector2.Zero;
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "namespace":
                                    typeNamespace = attribute.Value;
                                    break;

                                case "type":
                                    if (entityTypeIdentifiers.ContainsKey(attribute.Value))
                                    {
                                        type = entityTypeIdentifiers[attribute.Value];
                                    }
                                    else
                                    {
                                        type = Type.GetType(typeNamespace + "." + attribute.Value);
                                    }
                                    break;

                                case "x":
                                    position.X = float.Parse(attribute.Value);
                                    break;

                                case "y":
                                    position.Y = float.Parse(attribute.Value);
                                    break;

                                default:
                                    parameterTypes.Add(typeof(String));
                                    parameters.Add(attribute.Value);
                                    break;
                            }
                        }
                        parameters[0] = position;
                        entities.Add((IGameObject)type.GetConstructor(parameterTypes.ToArray()).Invoke(parameters.ToArray()));
                        break;

                }
            }

            return entities;
        }

        private static IGameObject LoadTile(int tileNumber, Tileset tileset)
        {
            // -1 specifies no tile.
            if (tileNumber <= -1)
            {
                return null;
            }

            int x = tileNumber % (int)tileset.PassabilityMapping[0].Length;
            int y = tileNumber / (int)tileset.PassabilityMapping[0].Length;
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

        static LevelMap()
        {
            entityTypeIdentifiers = new Dictionary<string, Type>();
            entityTypeIdentifiers.Add("health", typeof(HealthPack));
        }
    }
}
