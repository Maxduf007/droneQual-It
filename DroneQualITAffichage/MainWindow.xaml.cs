using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DroneQualIT.Affichage
{
    public partial class MainWindow : Window
    {
        private const int ImageSize = 20;
        private const int SleepTimeDefault = 25;

        private IList<Vehicule> Vehicules { get; } = new List<Vehicule>();
        private IList<Image> Images { get; } = new List<Image>();
        private MessageQueue Queue { get; }

        public MainWindow()
        {
            if (!MessageQueue.Exists(@".\Private$\DroneQualIT"))
                MessageQueue.Create(@".\Private$\DroneQualIT");

            Queue = new MessageQueue(@".\Private$\DroneQualIT", QueueAccessMode.Receive)
            {
                Formatter = new BinaryMessageFormatter()
            };

            InitializeComponent();
            Queue.ReceiveCompleted += OnReceiveCompleted;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) =>
            Task.Run(() => Queue.BeginReceive());

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = Queue.EndReceive(e.AsyncResult);
            
            switch (message.Body)
            {
                case Vehicule vehicule:
                    UpdateVehiculeLocation(vehicule);
                    Queue.BeginReceive();
                    break;
                case int time:
                    Sleep(time, (_sender, _e) => Queue.BeginReceive()); break;
            }
        }

        private void Sleep(int time, EventHandler handler) =>
            new DispatcherTimer(TimeSpan.FromMilliseconds(time), DispatcherPriority.Normal, handler, Dispatcher.CurrentDispatcher);

        private void UpdateVehiculeLocation(Vehicule vehicule)
        {
            if (Vehicules.SingleOrDefault(v => v.Id == vehicule.Id) is Vehicule known)
            {
                known.Move(vehicule.X, vehicule.Y);
                vehicule = known;
            }
            else
                Vehicules.Add(vehicule);

            UpdateDisplay(vehicule);
        }

        private void UpdateDisplay(Vehicule vehicule)
        {
            int index = Vehicules.IndexOf(vehicule);

            if (index == Images.Count)
                Images.Add(new Image
                {
                    Source = vehicule.ToImage(),
                    Height = ImageSize,
                    Width = ImageSize
                });

            Canvas.SetLeft(Images[index], vehicule.X);
            Canvas.SetTop(Images[index], vehicule.Y);
        }
    }
}
