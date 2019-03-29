using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace dronePath
{
    /// <summary>
    /// Logique d'interaction pour PahtDraw.xaml
    /// </summary>
    public partial class PahtDraw : UserControl
    {
        public MainWindow MW;
        public List<Tuple<int, int>> coords = new List<Tuple<int, int>>();

        public PahtDraw(MainWindow mw)
        {
            InitializeComponent();

            MW = mw;

            lbl_id.Content = MW.id;
            lbl_type.Content = MW.type;


        }

        private void Grd_path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = Mouse.GetPosition(Application.Current.MainWindow).X;
            double y = Mouse.GetPosition(Application.Current.MainWindow).Y;

            Rectangle rec = new Rectangle();
            rec.Fill = Brushes.Red;
            rec.Height = 20;
            rec.Width = 20;
            rec.Stroke = Brushes.LightBlue;
            rec.StrokeThickness = 2;

            Canvas.SetZIndex(rec, 10);
            Canvas.SetLeft(rec, x);
            Canvas.SetTop(rec, y);


            grd_path.Children.Add(rec);
            coords.Add(new Tuple<int, int>((int)x, (int)y));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(MW.file_name, true);
            foreach (Tuple<int, int> coord in coords)
            {
                sw.WriteLine(MW.type + "," + MW.id + "," + coord.Item1 + "," + coord.Item2);
            }

            sw.Close();


            FormChoices fc = new FormChoices(MW);

            MW.replace(fc);
        }

        private void sleep_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(MW.file_name, true);
           
           sw.WriteLine("sleep,500");
            

            sw.Close();
        }
    }
}
