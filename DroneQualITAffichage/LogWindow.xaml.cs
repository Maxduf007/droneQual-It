using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DroneQualIT.Affichage
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            mainWindow.ReceivedMessage +=
                (sender, e) => Dispatcher.Invoke(() => WriteLogLine(e));

        }

        private void WriteLogLine(object e)
        {
            switch (e)
            {
                case Vehicule vehicule:
                    txtLog.Text += $"Commande {vehicule}\n"; break;
                case int time:
                    txtLog.Text += $"Sleep de {time} ms\n"; break;
            }
        }

        /*public Message message
       {
           get { return message; }
           set
           {
               message = value;
               OnPropertyChanged();
           }*/

        /*public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged ()
        {

        }*/
    }
}
