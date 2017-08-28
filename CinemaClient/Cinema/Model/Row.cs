using System.Collections.Generic;

namespace Cinema.Model
{
    public class Row
    {
        public int RowNum { get; private set; }
        public List<Seat> Seats { get; set; }

        public Row(int rowNum)
        {
            RowNum = rowNum;
            Seats = new List<Seat>();
        }

        public Seat this[int i]
        {
            get { return Seats[i]; }
        }
    }
}
