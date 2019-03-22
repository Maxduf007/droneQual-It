using System;

namespace DroneQualIT
{
    [Serializable]
    public class Moto : Vehicule
    {
        static Moto()
        {
            UriPath = $"{BasePath}Camion.png";
        }

        public Moto(int id, int x, int y) : base(id, x, y) { }
    }
}
