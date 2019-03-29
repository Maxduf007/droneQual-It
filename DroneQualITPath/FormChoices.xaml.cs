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
    /// Logique d'interaction pour FormChoices.xaml
    /// </summary>
    public partial class FormChoices : UserControl
    {
        public MainWindow MW;

        public FormChoices(MainWindow mw)
        {
            InitializeComponent();

            MW = mw;
        }


        private void Txt_id_TextChanged(object sender, TextChangedEventArgs e)
        {
            int length = txt_id.Text.Length;

            if (length - 1 >= 0 && !char.IsNumber(txt_id.Text[length-1]))
            {
                string ss = txt_id.Text.Substring(0, length - 1);
                txt_id.Text = ss;
                txt_id.Select(length, 0);
                return;
            }
        }

        private void Btn_send_Click(object sender, RoutedEventArgs e)
        {
            if (txt_id.Text != "")
            {
                int id = int.Parse(txt_id.Text);
                string type = ((ComboBoxItem)cbo_type.SelectedItem).Content.ToString();


                MW.id = id;
                MW.type = type;

                MW.replace(new PahtDraw(MW));
            }
        }
    }
}
