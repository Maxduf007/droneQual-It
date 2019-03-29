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

        private void WriteLogLine(Message e)
        {
            Vehicule vehicule = (Vehicule)e.Body;

            txtLog.Text +=  "Commande " + vehicule.ToString() + "\n";

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
