using Cinema.Serialization;
using System.Collections.Generic;

namespace Cinema.Model
{
    public class CinemaHall
    {
        private static int _colRows;
        private static int _colSeats;

        public List<Row> Rows { get; set; }

        public CinemaHall()
        {
            Rows = new List<Row>();
        }
        public Row this[int i]
        {
            get { return Rows[i]; }
        }

        public static CinemaHall InitCinemaHall(int colRows, int colSeats)
        {
            _colRows = colRows;
            _colSeats = colSeats;

            CinemaHall cinemaHall = new CinemaHall();
            for (int i = 0; i < colRows; i++)
            {
                Row row = new Row(i + 1);
                for (int j = 0; j < colSeats; j++)
                {
                    row.Seats.Add(new Seat(colSeats - j));
                }
                cinemaHall.Rows.Add(row);
            }
            return cinemaHall;
        }

        public void SetOccupiedSeats(List<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                this[booking.SeatRow - 1][_colSeats - booking.SeatNum].IsOccupied = true;
                this[booking.SeatRow - 1][_colSeats - booking.SeatNum].IsSelected = false;
            }
        }
    }
}
