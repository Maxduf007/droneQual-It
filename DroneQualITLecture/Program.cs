using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;

namespace DroneQualIT.Lecture
{
    internal static class Program
    {
        private const string Path = "Commandes/";
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

            foreach (string item in Directory.EnumerateFiles(Path, Filter))
                HandleFile(item, item.Split('/').Last());

            var FileWatcher = Watch();
            while(true)
                FileWatcher.WaitForChanged(WatcherChangeTypes.All);
        }

        private static FileSystemWatcher Watch()
        {
            var FileWatcher = new FileSystemWatcher(Path, Filter);
            FileWatcher.Created += (sender, e) => HandleFile(e.FullPath, e.Name);

            Console.WriteLine("En attente d'un nouveau fichier (Commande_*.txt");

            return FileWatcher;
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
                .Select((line, index) => new { Values = line.TrimEnd(',').Split(','), Index = index + 1 })
                .Select(values => Instantiate(values.Values, values.Index))
                .Where(value => !(value is null));
        }
    }
}
