using System;

namespace Cinema.Serialization
{
    [Serializable]
    public class Booking
    {
        public int BookingId { get; set; }
        public int MovieId { get; set; }
        public int SeatNum { get; set; }
        public int SeatRow { get; set; }
    }
}
