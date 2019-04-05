using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DroneQualIT.Affichage
{
    public partial class MainWindow : Window
    {
        public event EventHandler<Message> ReceivedMessage;

        private const int ImageSize = 20;
        private const int SleepTimeDefault = 25;

        private Task Interpreter { get; set; }
        private ConcurrentQueue<object> ActionQueue { get; } = new ConcurrentQueue<object>();
        private IList<Vehicule> Vehicules { get; } = new List<Vehicule>();
        private IList<Image> Images { get; } = new List<Image>();
        private MessageQueue Queue { get; }

        public MainWindow()
        {
            if (!MessageQueue.Exists(@".\Private$\DroneQualT"))
                MessageQueue.Create(@".\Private$\DroneQualT");

            string everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).Value;

            Queue = new MessageQueue(@".\Private$\DroneQualT", QueueAccessMode.Receive)
            {
                Formatter = new BinaryMessageFormatter()
            };
            Queue.SetPermissions(everyone, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

            InitializeComponent();
            Queue.ReceiveCompleted += OnReceiveCompleted;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) =>
            Queue.BeginReceive();

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = Queue.EndReceive(e.AsyncResult);
            ActionQueue.Enqueue(message.Body);
            Queue.BeginReceive();

            if ((Interpreter?.Status ?? TaskStatus.Canceled) 
                >= TaskStatus.RanToCompletion)
                Interpreter = Task.Run(ExecuteAction);
        }

        private async Task ExecuteAction()
        {
            int sleep = 100;
            if (!ActionQueue.TryDequeue(out object message))
                return;

            switch (message)
            {
                case Vehicule vehicule:
                    Dispatcher.Invoke(() => UpdateVehiculeLocation(vehicule), DispatcherPriority.Input); break;
                case int time:
                    sleep = time; break;
            }

            ReceivedMessage?.Invoke(this, (Message)message);

            await Task.Delay(sleep);
            await ExecuteAction();
        }

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
            {
                Images.Add(new Image
                {
                    Source = vehicule.ToImage(),
                    Height = ImageSize,
                    Width = ImageSize
                });

                canvas.Children.Add(Images.Last());
            }

            Canvas.SetLeft(Images[index], vehicule.X);
            Canvas.SetTop(Images[index], vehicule.Y);

            canvas.UpdateLayout();
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            new LogWindow(this).Show();
    }
}
