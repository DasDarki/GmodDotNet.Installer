using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using GmodDotNet.Installer.UI;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using Octokit;

namespace GmodDotNet.Installer
{
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    internal class WebInstaller
    {
        private readonly Release _release;
        private readonly string _path;
        private readonly string _platform;
        private readonly string _tag;
        private readonly int _type;

        internal WebInstaller(string path, string platform, string tag, int type, Release release)
        {
            _path = Path.Combine(path, "garrysmod\\lua");
            _release = release;
            _platform = platform.ToLower();
            _tag = tag;
            _type = type;
        }

        internal string Start()
        {
            string binPath = Path.Combine(FormatDir("bin"), "gmoddotnet-bin.zip");
            if (File.Exists(binPath))
                File.Delete(binPath);
            string binUrl = GetAssetUrl(GetBinFile());
            if (string.IsNullOrEmpty(binUrl))
                return "No bin asset found";
            Console.Clear();
            Download(binUrl, binPath);
            Console.WriteLine();
            Console.Write("Installing bin asset... ");
            using (ProgressBar installBar = new ProgressBar())
            {
                if (_platform == "windows")
                {
                    ExtractZip(installBar, binPath);
                }
                else
                {
                    ExtractTarGz(installBar, binPath);
                }

                switch (Program.Types[_type])
                {
                    case "Client":
                        string clCleanup = Path.Combine(FormatDir("bin"), "gmsv_dotnet_" + GetPlatformAbbr() + ".dll");
                        if(File.Exists(clCleanup))
                            File.Delete(clCleanup);
                        break;
                    case "Server":
                        string svCleanup = Path.Combine(FormatDir("bin"), "gmcl_dotnet_" + GetPlatformAbbr() + ".dll");
                        if(File.Exists(svCleanup))
                            File.Delete(svCleanup);
                        break;
                }
                
                File.Delete(binPath);
                installBar.Report(1);
                installBar.Stop();
            }
            
            Console.WriteLine();
            if (Program.Types[_type] == "Client" || Program.Types[_type] == "Both")
            {
                string outPath = Path.Combine(FormatDir("autorun\\client"), "gmod-dot-net-client.lua");
                if(File.Exists(outPath))
                    File.Delete(outPath);
                string url = GetAssetUrl($"gmod-dot-net-lua-client.{_tag}.lua");
                if (string.IsNullOrEmpty(url))
                    return "No client-side lua asset found";
                Download(url, outPath);
                Console.WriteLine();
            }

            if (Program.Types[_type] == "Server" || Program.Types[_type] == "Both")
            {
                string outPath = Path.Combine(FormatDir("autorun\\server"), "gmod-dot-net-server.lua");
                if(File.Exists(outPath))
                    File.Delete(outPath);
                string url = GetAssetUrl($"gmod-dot-net-lua-server.{_tag}.lua");
                if (string.IsNullOrEmpty(url))
                    return "No server-side lua asset found";
                Download(url, outPath);
                Console.WriteLine();
            }
            
            Console.Clear();
            return null;
        }

        private void ExtractZip(ProgressBar installBar, string binPath)
        {
            FastZip fastZip = new FastZip();
            installBar.Report(0.1);
            fastZip.ExtractZip(binPath, FormatDir("bin"), null);
            installBar.Report(0.5);
        }

        private void ExtractTarGz(ProgressBar installBar, string binPath)
        {
            using Stream gzipStream = new GZipInputStream(File.OpenRead(binPath));
            using TarArchive archive = TarArchive.CreateInputTarArchive(gzipStream, null);
            installBar.Report(0.1);
            archive.ExtractContents(FormatDir("bin"));
            installBar.Report(0.5);
        }
        
        private string GetAssetUrl(string fileName)
        {
            foreach (ReleaseAsset asset in _release.Assets)
            {
                if (asset.Name == fileName)
                    return asset.BrowserDownloadUrl;
            }

            return null;
        }

        private string GetBinFile()
        {
            return $"gmod-dot-net-{_platform}.{_tag}" + (_platform == "windows" ? ".zip" : ".tar.gz");
        }

        private string FormatDir(string dir)
        {
            string path = Path.Combine(_path, dir);
            Directory.CreateDirectory(path);
            return path;
        }

        private string GetPlatformAbbr()
        {
            switch (_platform)
            {
                case "windows":
                    return "win64";
                case "linux":
                    return "linux64";
                case "osx":
                    return "osx64";
            }
            return null;
        }

        private static void Download(string url, string outputFile)
        {
            Console.Write($"Downloading {url}... ");
            using WebClient client = new WebClient();
            ProgressBar bar = new ProgressBar();
            bool finished = false;
            client.DownloadProgressChanged += (sender, args) =>
            {
                bar.Report(args.ProgressPercentage / 100d);
            };
            client.DownloadFileCompleted += (sender, args) =>
            {
                finished = true;
            };
            client.DownloadFileAsync(new Uri(url), outputFile);
            while (!finished)
            {
                Thread.Sleep(10);
            }
            bar.Stop();
        }
    }
}