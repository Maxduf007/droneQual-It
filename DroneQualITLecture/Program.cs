﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace DroneQualIT.Lecture
{
    internal static class Program
    {
        private const string Path_Commandes = "Commandes/";
        private const string Path_CommandesTraitees = Path_Commandes + "CommandesTraitees/";
        private const string Filter = "Commande_*.txt";

        private static MessageQueue Queue { get; }
        static Program()
        {
            if (!MessageQueue.Exists(@".\Private$\DroneQualIT"))
                MessageQueue.Create(@".\Private$\DroneQualIT");
            Queue = new MessageQueue(@".\Private$\DroneQualIT", QueueAccessMode.Send)
            {
                Formatter = new BinaryMessageFormatter()
            };
        }

        static void Main(string[] args)
        {
            DisableConsoleQuickEdit.Go();

            if (!Directory.Exists(Path_Commandes))
                Directory.CreateDirectory(Path_Commandes);
            if (!Directory.Exists(Path_CommandesTraitees))
                Directory.CreateDirectory(Path_CommandesTraitees);

            foreach (string item in Directory.EnumerateFiles(Path_Commandes, Filter))
                HandleFile(item, item.Split('/').Last());

            var FileWatcher = Watch();

            /*while(true)
                FileWatcher.WaitForChanged(WatcherChangeTypes.All);*/

            new AutoResetEvent(false).WaitOne();
        }

        private static FileSystemWatcher Watch()
        {
            var FileWatcher = new FileSystemWatcher(Path_Commandes, Filter)
            {
                EnableRaisingEvents = true
            };
            FileWatcher.Created += (sender, e) => BeginHandleFile(e.FullPath, e.Name);
            FileWatcher.Renamed += (sender, e) => BeginHandleFile(e.FullPath, e.Name);

            Console.WriteLine("En attente d'un nouveau fichier (Commande_*.txt)");

            return FileWatcher;
        }

        private static async Task BeginHandleFile(string path, string name)
        {
            await Task.Delay(500);
            HandleFile(path, name);
        }

        private static void HandleFile(string path, string name)
        {
            Console.WriteLine($"Traitement du fichier {name} en cours...");

            foreach (object value in ReadFile(path))
                switch (value)
                {
                    case Vehicule vehicule:
                        Queue.Send(new Message(vehicule, Queue.Formatter)); break;
                    case int time:
                        Queue.Send(new Message(time, Queue.Formatter)); break;
                }

            string newPath = Path_CommandesTraitees + name.Substring(0, name.Length - 3) + "sav";
            if (File.Exists(newPath))
                File.Delete(newPath);
            File.Move(path, newPath);
        }

        static IEnumerable<object> ReadFile(string path)
        {
            object Instantiate(string[] values, int index)
            {
                try
                {
                    if (values.Length < 2)
                        throw new InvalidDataException("Le format de la chaîne d'entrée est invalide.");

                    Console.WriteLine($"Traitement de {values[0]} ({values[1]})...");
                    int id = Convert.ToInt32(values[1]);

                    switch (values[0].ToLowerInvariant())
                    {
                        case "camion":
                            return new Camion(id, Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                        case "moto":
                            return new Moto(id, Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                        case "voiture":
                            return new Voiture(id, Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                        case "sleep":
                            return id;
                        default:
                            throw new InvalidDataException($"{values[0]} est une commande inconnu.");
                    }
                }
                catch (Exception e)
                {
                    // Convert.ToInt32() peut throw "Le format de la chaîne d'entrée est invalide.".
                    Console.WriteLine($"Ligne {index}: {e.Message}");
                    return null;
                }
            }

            return File.ReadAllLines(path)
                .Select((line, index) => new { Line = line, Index = index })
                .Where(line => line.Line.Length > 2 && line.Line.Substring(0, 2) != "//")
                .Select(line => new { Values = line.Line.TrimEnd(',').Split(','), line.Index })
                .Select(values => Instantiate(values.Values, values.Index))
                .Where(value => !(value is null));
        }
    }
}
