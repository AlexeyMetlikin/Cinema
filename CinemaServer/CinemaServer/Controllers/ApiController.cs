using CinemaServer.Models.Abstract;
using CinemaServer.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CinemaServer.Controllers
{
    public class ApiController : Controller
    {
        private ICinemaRepository _repository;

        public ApiController(ICinemaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public void GetMovies()
        {
            try
            {
                var jsonMovies = JsonConvert.SerializeObject(_repository.Movies);
                WriteJsonResponse(jsonMovies, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                WriteJsonResponse(JsonConvert.SerializeObject(new { code = 403, Message = e.Message }), (int)HttpStatusCode.Forbidden);
            }
        }

        [HttpPost]
        public void GetMovieBookedSeats()
        {
            try
            {
                byte[] jsonMovie = new byte[Request.ContentLength];
                Request.InputStream.Read(jsonMovie, 0, jsonMovie.Length);

                int movieId = getMovieIdFromJson(Encoding.UTF8.GetString(jsonMovie));
                var movie = getMovieById(movieId);
                var bookedSeats = _repository.Bookings.Where(b => b.MovieId == movie.MovieId);
                var jsonBookedSeats = JsonConvert.SerializeObject(bookedSeats);
                WriteJsonResponse(jsonBookedSeats, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                WriteJsonResponse(JsonConvert.SerializeObject(new { code = 400, Message = e.Message }), (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public void ToBookSeats()
        {
            try
            {
                byte[] jsonBookings = new byte[Request.ContentLength];
                Request.InputStream.Read(jsonBookings, 0, jsonBookings.Length);

                var bookings = SerializeBookings(System.Text.Encoding.UTF8.GetString(jsonBookings));

                foreach (var booking in bookings)
                {
                    if (_repository.Bookings.
                        FirstOrDefault(b => b.MovieId == booking.MovieId &&
                                            b.SeatNum == booking.SeatNum &&
                                            b.SeatRow == booking.SeatRow) != null)
                    {
                        WriteJsonResponse(JsonConvert.SerializeObject(new { code = 403, Message = "Seat already booked", Booking = booking }), (int)HttpStatusCode.Forbidden);
                        return;
                    }
                }

                foreach (var booking in bookings)
                {
                    _repository.SaveBooking(booking);
                }

                WriteJsonResponse(JsonConvert.SerializeObject(new { result = "ok", Message = "Seats books" }), (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                WriteJsonResponse(JsonConvert.SerializeObject(new { code = 400, Message = e.Message }), (int)HttpStatusCode.BadRequest);
            }
        }

        private int getMovieIdFromJson(string jsonMovieId)
        {
            Regex regex = new Regex("\\{\"MovieId\":[0-9]*\\}");
            if (!regex.IsMatch(jsonMovieId))
            {
                throw new Exception("Invalid request. Movie id must be an integer");
            }
            return int.Parse(jsonMovieId.Split(':')[1].Split('}')[0]);
        }

        private Movie getMovieById(int movieId)
        {
            var movie = _repository.Movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movie == null)
            {
                throw new Exception("Invalid request. No movie with such id");
            }
            return movie;
        }

        private Booking[] SerializeBookings(string jsonBookings)
        {
            var bookings = new List<Booking>();
            try
            {
                bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);
                return bookings.ToArray();
            }
            catch
            {
                return SerializeOneBooking(jsonBookings);
            }
        }

        private Booking[] SerializeOneBooking(string jsonBooking)
        {
            try
            {
                var booking = JsonConvert.DeserializeObject<Booking>(jsonBooking);
                if (booking == null)
                {
                    throw new Exception("Json string is invalid");
                }
                return (new List<Booking> { booking }).ToArray();
            }
            catch
            {
                throw new Exception("Json string is invalid");
            }
        }

        private void WriteJsonResponse(string jsonResponse, int statusCode)
        {
            Response.Clear();
            Response.StatusCode = statusCode;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(jsonResponse);
            Response.End();
        }
    }
}