using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace CinemaServer.Models.Entities
{
    public class Movie
    {
        public Movie()
        {
            Bookings = new HashSet<Booking>();
        }

        [HiddenInput(DisplayValue = false)]
        public int MovieId { get; set; }

        [Required (ErrorMessage ="Требуется заполнить поле \"Название\"")]
        [Display(Name = "Название")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Name { get; set; }

        [Required (ErrorMessage = "Требуется заполнить поле \"Время показа\"")]
        [Display(Name = "Время показа")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan ShowTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}