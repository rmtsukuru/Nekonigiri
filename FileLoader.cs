using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nekonigiri
{
    /// <summary>
    /// Wrapper for the standard C# File class, extends file loading methods to
    /// look in additional directories.
    /// </summary>
    internal class FileLoader
    {
        private static readonly string[] Directories = new string[] { "../../../Content/levels/" };

        public static String ReadAllText(string path)
        {
            for (int i = 0; i < Directories.Length; i++)
            {
                if (File.Exists(Directories[i] + path))
                {
                    return File.ReadAllText(Directories[i] + path);
                }
            }
            return File.ReadAllText(path);
        }
    }
}
