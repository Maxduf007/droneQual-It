using System;

namespace DroneQualIT
{
    [Serializable]
    public class Moto : Vehicule
    {
        static Moto()
        {
            UriPath = $"{BasePath}Camion.png";
            Name = nameof(Moto);
        }

        public Moto(int id, int x, int y) : base(id, x, y) { }
    }
}
