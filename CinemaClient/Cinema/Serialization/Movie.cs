using System;

namespace Cinema.Serialization
{
    [Serializable]
    public class Movie
    {
        public int MovieId { get; set; }

        public string Name { get; set; }

        public string ShowTime { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}   
