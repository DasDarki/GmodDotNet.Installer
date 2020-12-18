using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace GmodDotNet.Installer
{
    internal static class GModPathFinder
    {
        internal static string Find(OSPlatform platform)
        {
            if (platform == OSPlatform.Windows)
            {
                var steam32 = "SOFTWARE\\VALVE\\";
                var steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";
                var key32 = Registry.LocalMachine.OpenSubKey(steam32);
                var key64 = Registry.LocalMachine.OpenSubKey(steam64);
                if (key32 != null && !string.IsNullOrEmpty(key32.ToString()))
                {
                    foreach (var k32SubKey in key32.GetSubKeyNames())
                    {
                        using var subKey = key32.OpenSubKey(k32SubKey);
                        if (subKey != null)
                        {
                            string path = subKey.GetValue("InstallPath")?.ToString();
                            if(string.IsNullOrEmpty(path)) continue;
                            string gmodPath = Path.Combine(path, @"steamapps\common\GarrysMod");
                            if (Directory.Exists(Path.Combine(gmodPath, "garrysmod")))
                                return gmodPath;
                        }
                    }
                }

                if (key64 != null && !string.IsNullOrEmpty(key64.ToString()))
                {
                    foreach (var k64SubKey in key64.GetSubKeyNames())
                    {
                        using var subKey = key64.OpenSubKey(k64SubKey);
                        if (subKey != null)
                        {
                            string path = subKey.GetValue("InstallPath")?.ToString();
                            if(string.IsNullOrEmpty(path)) continue;
                            string gmodPath = Path.Combine(path, @"steamapps\common\GarrysMod");
                            if (Directory.Exists(Path.Combine(gmodPath, "garrysmod")))
                                return gmodPath;
                        }
                    }
                }
            }

            return null;
        }
    }
}