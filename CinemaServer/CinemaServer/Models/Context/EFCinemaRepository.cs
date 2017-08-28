using CinemaServer.Models.Abstract;
using CinemaServer.Models.Entities;
using System.Linq;

namespace CinemaServer.Models.Context
{
    public class EFCinemaRepository : ICinemaRepository
    {
        private EFDbContext _context = new EFDbContext();

        public IQueryable<Booking> Bookings
        {
            get { return _context.Bookings; }
        }

        public IQueryable<Movie> Movies
        {
            get { return _context.Movies; }
        }

        public void SaveMovie(Movie movie)
        {
            if (movie.MovieId == 0)
            {
                _context.Movies.Add(movie);
            }
            else
            {
                Movie dbMovie = _context.Movies.Find(movie.MovieId);
                if (dbMovie != null)
                {
                    dbMovie.Name = movie.Name;
                    dbMovie.ShowTime = movie.ShowTime;
                }
            }
            _context.SaveChanges();
        }

        public Movie DeleteMovie(int movieId)
        {
            Movie dbMovie = _context.Movies.Find(movieId);
            if (dbMovie != null)
            {
                _context.Movies.Remove(dbMovie);
                _context.SaveChanges();
            }
            return dbMovie;
        }

        public void SaveBooking(Booking booking)
        {
            if (booking.BookingId == 0)
            {
                _context.Bookings.Add(booking);
            }
            else
            {
                Booking dbBooking = _context.Bookings.Find(booking.BookingId);
                if (dbBooking != null)
                {
                    dbBooking.Movie = booking.Movie;
                    dbBooking.SeatNum = booking.SeatNum;
                    dbBooking.SeatRow = booking.SeatRow;
                }
            }
            _context.SaveChanges();
        }
    }
}