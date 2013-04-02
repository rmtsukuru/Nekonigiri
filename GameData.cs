using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Nekonigiri
{
    internal class GameData
    {
        public bool Debug
        {
            get;
            set;
        }

        public ContentManager Content
        {
            get;
            set;
        }

        public Game game;

        public KeyboardState lastKeyboardState
        {
            get;
            set;
        }

        public ILevel CurrentLevel
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
