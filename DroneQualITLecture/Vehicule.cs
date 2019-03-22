using System;
using System.Windows.Media.Imaging;

namespace DroneQualIT
{
    [Serializable]
    public abstract class Vehicule
    {
        protected const string BasePath = "pack://application:,,,/Resources/Images/";
        protected static string UriPath { get; set; }

        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Vehicule(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }

        public BitmapImage ToImage() =>
            new BitmapImage(new Uri(UriPath, UriKind.Absolute));
    }
}
