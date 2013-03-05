using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Nekonigiri
{
    internal class GameData
    {
        public ContentManager Content
        {
            get;
            set;
        }

        public Game game;

        public ILevel Level
        {
            get;
            set;
        }

        private GameData()
        {

        }

        private static GameData instance;

        public static GameData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameData();
                }
                return instance;
            }
        }
    }
}
