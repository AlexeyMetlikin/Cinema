using CinemaServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CinemaServer.Models.Context
{
    public class EFDbContext : DbContext
    {
        public EFDbContext()
            :base("CinemaDB")
        {
            Database.SetInitializer(new CinemaDbInit());
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
    }
}