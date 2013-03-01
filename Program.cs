using System;

namespace Nekonigiri
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OnigiriGame game = new OnigiriGame())
            {
                game.Run();
            }
        }
    }
}

