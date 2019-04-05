using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dronePath
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl ContenuEcran { get; set; }
        public int id;
        public string type;
        public string file_name;

        public MainWindow()
        {
            InitializeComponent();

            FormChoices fc = new FormChoices(this);

            replace(fc);

            file_name = $"Commandes/Commande_Mobile_{Guid.NewGuid().ToString()}.txt";
        }

        public void replace(UserControl uc )
        {
            grd_main.Children.Remove(ContenuEcran);

            // Mettre le nouveau userControl
            ContenuEcran = uc;

            grd_main.Children.Add(ContenuEcran);
        }
    }
}
