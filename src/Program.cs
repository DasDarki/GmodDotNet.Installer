using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GmodDotNet.Installer.UI;
using Octokit;

namespace GmodDotNet.Installer
{
    static class Program
    {
        internal static readonly string[] Types = {"Both", "Client", "Server"};

        private static readonly string RepoUsername = "GmodNET";
        private static readonly string RepoName = "GmodDotNet";
        private static readonly GitHubClient Client = new GitHubClient(new ProductHeaderValue("GmodDotNet-Installer"));
        private static IReadOnlyList<Release> _releases;
        
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "GmodDotNet-Installer (c) 2020 github.com/DasDarki [unofficial]";
            Console.CursorVisible = false;
            try
            {
                Console.Write("Loading GmodDotNet-Repository... ");
                using (ProgressBar bar = new ProgressBar())
                    _releases = Client.Repository.Release.GetAll(RepoUsername, RepoName).GetAwaiter().GetResult();
                if (_releases == null || _releases.Count <= 0)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("The GmodDotNet-Repository could not be loaded! ");
                    Console.ResetColor();
                    AwaitAnyKey();
                    return;
                }

                string tag = _releases[0].TagName;
                OSPlatform? platform = GetPlatform();
                if (platform == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Your OS is unsupported! ");
                    Console.ResetColor();
                    AwaitAnyKey();
                    return;
                }
            
                SelectionBox typeSelection = new SelectionBox("Select Installation Type: ", Types);
                int type = typeSelection.Await();
                ValueBrowser tagBrowser = new ValueBrowser("release", "ENTER", "Select GmodDotNet-Release: ", tag,
                    newTag => !string.IsNullOrEmpty(newTag) && IsValidTag(newTag));
                tag = tagBrowser.Await();
                Release release = GetRelease(tag);
                string defaultGmodPath = GModPathFinder.Find(platform.Value);
                ValueBrowser pathBrowser = new ValueBrowser("path", "BROWSE", "Select GMod Path: ", defaultGmodPath, 
                    newPath => !string.IsNullOrEmpty(newPath) && Directory.Exists(Path.Combine(newPath, "garrysmod")));
                string path = pathBrowser.Await();
                if (new Confirmation(platform.Value.ToString(), type, path, tag).Await())
                {
                    Console.Clear();
                    WebInstaller installer = new WebInstaller(path, platform.Value.ToString(), tag, type, release);
                    string error = installer.Start();
                    Console.Clear();
                    if (string.IsNullOrEmpty(error))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("GmodDotNet successfully installed!");
                    }
                    else
                    {
                        Console.Write("Failed to install GmodDotNet: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(error);
                    }
                    
                    Console.ResetColor();
                    AwaitAnyKey();
                }
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("An error occurred!:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
                AwaitAnyKey();
            }
        }

        private static Release GetRelease(string tag)
        {
            foreach (Release release in _releases)
            {
                if (release.TagName == tag)
                {
                    return release;
                }
            }

            return null;
        }

        private static bool IsValidTag(string tag)
        {
            foreach (Release release in _releases)
            {
                if (release.TagName == tag)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AwaitAnyKey()
        {
            Console.WriteLine("Please press any key to exit...");
            Console.ReadKey();
        }
        
        private static OSPlatform? GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            return null;
        }
    }
}