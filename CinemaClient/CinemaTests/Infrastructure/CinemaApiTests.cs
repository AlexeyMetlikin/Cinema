using Cinema.Infrastructure.Abstract;
using Cinema.Infrastructure.Entities;
using Cinema.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CinemaTests.Infrastructure
{
    [TestFixture]
    public class CinemaApiTests
    {
        [Test]
        public void Can_Send_Valid_Get_Request_And_Get_Response()
        {
            ICinemaApi API = new CinemaApi("http://localhost:9090/");

            string request = "api/getMovies";

            string response = API.SendRequest("GET", request, null).Result;

            var movies = JsonConvert.DeserializeObject<List<Movie>>(response);

            Assert.IsNotNull(response);
            Assert.IsNotNull(movies);
        }
        
        [Test]
        public void Can_Send_Valid_Post_Request_And_Get_Response()
        {
            ICinemaApi API = new CinemaApi("http://localhost:9090/");

            string request = "api/getMovieBookedSeats";

            var content = JsonConvert.SerializeObject(new { MovieId = 1 });
            string jsonBookings = API.SendRequest("POST", request, content).Result;

            var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);

            Assert.IsNotNull(bookings);
        }

        [Test]
        public void Cannot_Send_Invalid_Request_And_Get_Response()
        {
            ICinemaApi API = new CinemaApi("http://localhost:9090/");

            string request = "api/invalidRequest";

            try
            {
                var response = API.SendRequest("GET", request, null).Result;
            }
            catch (AggregateException aggregateException)
            {                
                Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(HttpRequestException));
            }
        }
    }
}
