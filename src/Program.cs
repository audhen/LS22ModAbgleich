using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Configuration;

namespace LS22ModAbgleich
{
    internal class Program
    {
        public static string xmlUrl = null;
        public static string modDownloadBase = null;
        public static string downloadFileName = "serverInfo.xml";
        public static string modPath = null;
        public static ConsoleColor defaultColor = Console.ForegroundColor;

        static void Main(string[] args)
        {
            modPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\FarmingSimulator2022\mods\");
            xmlUrl = ConfigurationManager.AppSettings["serverInfoXmlUrl"];
            // URL zu http://ServerIP:Port/mods
            modDownloadBase = ConfigurationManager.AppSettings["modBaseUrl"];
            CheckMods();
        }

        private static List<Mod> GetServerMods()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(xmlUrl, downloadFileName);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(downloadFileName);
            XmlNode root = doc.DocumentElement;
            XmlNodeList mods = doc.GetElementsByTagName("Mod");
            List<Mod> modsList = new List<Mod>();
            foreach (XmlNode modNode in mods)
            {
                var mod = new Mod();
                mod.Name = modNode.Attributes["name"].Value.ToString();
                mod.ZipName = $"{mod.Name}.zip";
                mod.Hash = modNode.Attributes["hash"].Value.ToString();

                if (mod.Name.StartsWith("pdlc_"))
                {
                    //skip dlcs
                }
                else
                {
                    mod.FileFullName = Path.Combine(modPath, mod.ZipName);
                    modsList.Add(mod);
                }
            }

            return modsList;
        }

        private static void CheckMods()
        {
            var serverMods = GetServerMods();
            Console.WriteLine($"Der Server hat {serverMods.Count} Mods installiert.");

            List<string> modsToDownload = new List<string>();
            int modStep = 1;
            foreach (var mod in serverMods)
            {
                Console.Write($"Prüfe Mod ({modStep}/{serverMods.Count}): {mod.Name}");
                if (File.Exists(mod.FileFullName))
                {
                    if (mod.Hash.ToLower() != FileToLsHash(new FileInfo(mod.FileFullName)).ToLower())
                    {
                        modsToDownload.Add(mod.ZipName);
                    }
                }
                else
                {
                    modsToDownload.Add(mod.ZipName);
                }
                modStep++;
                Console.CursorLeft = 0;
                Console.Write(new string(' ', Console.WindowWidth-1));
                Console.CursorLeft = 0;
            }

            if (modsToDownload.Any())
            {
                WriteYellowLine($"Der Server hat {modsToDownload.Count} neue/aktualisierte Mod(s). Sollen diese heruntergeladen werden?");

                string antwort = null;
                while (antwort != "ja" && antwort != "nein")
                {
                    WriteYellowLine("Bitte antworten: ja/nein/auflisten");
                    antwort = Console.ReadLine().ToLower();

                    if (antwort == "ja")
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            int downloadStep = 1;
                            foreach (var zipName in modsToDownload)
                            {
                                try
                                {
                                    Console.Write($"Lade herunter: {zipName}");
                                    string webPath = $"{modDownloadBase}/{zipName}";
                                    string localPath = $"{modPath}\\{zipName}";
                                    webClient.DownloadFile(webPath, localPath);
                                    Console.CursorLeft = 0;
                                    Console.Write(new string(' ', Console.WindowWidth - 1));
                                    Console.CursorLeft = 0;
                                    Console.Write($"Heruntergeladen: {zipName} ({downloadStep}/{modsToDownload.Count})");
                                    Console.WriteLine();
                                }
                                catch (Exception ex)
                                {
                                    Console.CursorLeft = 0;
                                    Console.Write(new string(' ', Console.WindowWidth - 1));
                                    Console.CursorLeft = 0;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write($"FEHLER: {zipName} ({downloadStep}/{modsToDownload.Count})");
                                    Console.ForegroundColor = defaultColor;
                                    Console.WriteLine();
                                }

                                downloadStep++;
                            }
                        }

                        WriteYellowLine("Alle Mods heruntergeladen. Programm wird damit beendet. Bitte das Fenster von Hand schließen oder irgendeine Taste drücken.");
                        Console.ReadKey();
                        break;
                    }
                    else if(antwort == "nein")
                    {
                        WriteYellowLine("Es wurde nein gewählt. Programm wird damit beendet. Bitte das Fenster von Hand schließen oder irgendeine Taste drücken.");
                        Console.ReadKey();
                        break;
                    }
                    else if(antwort == "auflisten")
                    {
                        WriteYellowLine("Mods die heruntergeladen würden:");
                        modsToDownload.ForEach(mod => Console.WriteLine(mod));
                        WriteYellowLine("Sollen die Mods heruntergeladen werden?");
                    }
                }
            }
            else
            {
                WriteYellowLine("Alle Mods sind bereits aktuell. Programm wird damit beendet. Bitte das Fenster von Hand schließen oder irgendeine Taste drücken.");
                Console.ReadKey();
            }
        }

        private static void WriteYellowLine(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = defaultColor;
        }

        private static string FileToLsHash(FileInfo file)
        {
            byte[] fileContentBytes = File.ReadAllBytes(file.FullName);
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(file.FullName));
            Array.Resize(ref fileContentBytes, fileContentBytes.Length + fileNameBytes.Length);
            Array.Copy(fileNameBytes, 0, fileContentBytes, fileContentBytes.Length - fileNameBytes.Length, fileNameBytes.Length);
            return string.Join(string.Empty, new MD5Cng().ComputeHash(fileContentBytes).Select(x => x.ToString("x2")));
        }
    }
}
