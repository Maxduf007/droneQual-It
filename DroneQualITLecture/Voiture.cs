using System;

namespace DroneQualIT
{
    [Serializable]
    public class Voiture : Vehicule
    {
        static Voiture()
        {
            UriPath = $"{BasePath}Voiture.png";
        }

        public Voiture(int id, int x, int y) : base(id, x, y) {}
    }
}
