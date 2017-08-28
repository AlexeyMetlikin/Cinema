using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CinemaServer.Models.Entities
{
    public class Booking
    {
        [HiddenInput(DisplayValue = false)]
        public int BookingId { get; set; }

        [Required]
        public int SeatRow { get; set; }

        [Required]
        public int SeatNum { get; set; }

        [Required]
        public int MovieId { get; set; }

        [JsonIgnore]
        public virtual Movie Movie { get; set; }
    }
}