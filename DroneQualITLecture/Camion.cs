using System;

namespace DroneQualIT
{
    [Serializable]
    public class Camion : Vehicule
    {
        static Camion()
        {
            UriPath = $"{BasePath}Camion.png";
        }

        public Camion(int id, int x, int y) : base(id, x, y) { }
    }
}
