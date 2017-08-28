using CinemaServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaServer.Models.Abstract
{
    public interface ICinemaRepository
    {
        IQueryable<Booking> Bookings { get; }

        IQueryable<Movie> Movies { get; }

        void SaveMovie(Movie movie);

        Movie DeleteMovie(int movieId);

        void SaveBooking(Booking booking);
    }
}