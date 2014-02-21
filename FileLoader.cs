using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Nekonigiri
{
    /// <summary>
    /// Wrapper for the standard C# File class, extends file loading methods to
    /// look in additional directories. 
    /// 
    /// Also includes some utility methods for loading XML.
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

        private static Vector2 GetLevelSizeDirect(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int maxLength = 0;
            foreach (string s in lines)
            {
                if (s.Length > maxLength)
                {
                    maxLength = s.Length;
                }
            }
            return new Vector2(48 * maxLength,  48 * lines.Length);
        }

        public static Vector2 GetLevelSize(string path)
        {
            for (int i = 0; i < Directories.Length; i++)
            {
                if (File.Exists(Directories[i] + path))
                {
                    return GetLevelSizeDirect(Directories[i] + path);
                }
            }
            return GetLevelSizeDirect(path);
        }

        public static XmlDocument LoadXml(string path)
        {
            XmlDocument doc = new XmlDocument();
            for (int i = 0; i < Directories.Length; i++)
            {
                if (File.Exists(Directories[i] + path))
                {
                    doc.Load(Directories[i] + path);
                    return doc;
                }
            }
            doc.Load(path);
            return doc;
        }
    }
}
