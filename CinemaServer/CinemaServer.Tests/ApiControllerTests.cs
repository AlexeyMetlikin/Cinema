using System;
using NUnit.Framework;
using Moq;
using CinemaServer.Models.Abstract;
using CinemaServer.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using CinemaServer.Controllers;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using System.Web.Routing;
using System.Text;
using System.Net;

namespace CinemaServer.Tests
{
    [TestFixture]
    public class ApiControllerTests
    {
        [Test]
        public void Can_Get_Movies_List()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();

            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mockCinema = new Mock<ICinemaRepository>();
            var moviesInit = new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) }
            }.ToArray();
            mockCinema.Setup(m => m.Movies).Returns(moviesInit.AsQueryable());

            var controller = new ApiController(mockCinema.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            //Act           
            controller.GetMovies();

            var moviesResult = JsonConvert.DeserializeObject<Movie[]>(output.ToString());

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, mockHttpResponse.Object.StatusCode);
            for(int i = 0; i < moviesInit.Length; i++)
            {
                Assert.AreEqual(moviesInit[i].MovieId, moviesResult[i].MovieId);
                Assert.AreEqual(moviesInit[i].Name, moviesResult[i].Name);
                Assert.AreEqual(moviesInit[i].ShowTime, moviesResult[i].ShowTime);
            }
        }

        [Test]
        public void Can_Book_Few_Seats_With_Valid_Request_Content()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();

            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0) }
            }.AsQueryable());

            var bookings = new List<Booking>
            {
                new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 },
                new Booking { SeatNum = 2, SeatRow = 2, MovieId = 1 }
            };

            byte[] content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bookings));
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var waitingResult = JsonConvert.SerializeObject(new { result = "ok", Message = "Seats books" });
            //Act           
            controller.ToBookSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, mockHttpResponse.Object.StatusCode);
            mock.Verify(b => b.SaveBooking(It.IsAny<Booking>()), Times.Exactly(2));
            Assert.AreEqual("application/json; charset=utf-8", mockHttpResponse.Object.ContentType);
            Assert.AreEqual(output.ToString(), waitingResult);

            output.Close();
        }

        [Test]
        public void Can_Book_One_Seat_With_Valid_Request_Content()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();

            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mockCinema = new Mock<ICinemaRepository>();
            mockCinema.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0) }
            }.AsQueryable());

            var booking = new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 };

            byte[] content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(booking));
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mockCinema.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var waitingResult = JsonConvert.SerializeObject(new { result = "ok", Message = "Seats books" });

            //Act           
            controller.ToBookSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, mockHttpResponse.Object.StatusCode);
            mockCinema.Verify(b => b.SaveBooking(It.IsAny<Booking>()), Times.Once);
            Assert.AreEqual("application/json; charset=utf-8", mockHttpResponse.Object.ContentType);
            Assert.AreEqual(output.ToString(), waitingResult);

            output.Close();
        }

        [Test]
        public void Cannot_Book_Occupied_Seat()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpRequest = new Mock<HttpRequestBase>();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var booking = new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 };

            var mockCinema = new Mock<ICinemaRepository>();
            mockCinema.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0) }
            }.AsQueryable());

            mockCinema.Setup(m => m.Bookings).Returns(new List<Booking>
            {
                booking
            }.AsQueryable());

            byte[] content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(booking));
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mockCinema.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var waitingResult = JsonConvert.SerializeObject(new { code = 403, Message = "Seat already booked", Booking = booking });

            //Act           
            controller.ToBookSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.Forbidden, mockHttpResponse.Object.StatusCode);
            mockCinema.Verify(b => b.SaveBooking(It.IsAny<Booking>()), Times.Never);
            Assert.AreEqual("application/json; charset=utf-8", mockHttpResponse.Object.ContentType);
            Assert.AreEqual(output.ToString(), waitingResult);

            output.Close();
        }

        [Test]
        public void Cannot_Book_Seat_With_Null_Request_Content()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpRequest = new Mock<HttpRequestBase>();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            //Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0) }
            }.AsQueryable());

            byte[] content = new byte[0];
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            //Act           
            controller.ToBookSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, mockHttpResponse.Object.StatusCode);
            Assert.AreEqual("{\"code\":400,\"Message\":\"Json string is invalid\"}", output.ToString());
        }

        [Test]
        public void Can_Get_Booked_Seats_From_Existent_Movie()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpRequest = new Mock<HttpRequestBase>();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) }
            }.AsQueryable());

            var booking = new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 };
            mock.Setup(m => m.Bookings).Returns(new List<Booking>
            {
                booking
            }.AsQueryable());

            var jsonMovie = JsonConvert.SerializeObject(new { MovieId = 1 });

            byte[] content = Encoding.UTF8.GetBytes(jsonMovie);
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);            

            var controller = new ApiController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            //Act           
            controller.GetMovieBookedSeats();

            var bookedSeats = JsonConvert.DeserializeObject<Booking[]>(mockHttpResponse.Object.Output.ToString());

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, mockHttpResponse.Object.StatusCode);
            Assert.AreEqual(booking.BookingId, bookedSeats[0].BookingId);
            Assert.AreEqual(booking.SeatNum, bookedSeats[0].SeatNum);
            Assert.AreEqual(booking.SeatRow, bookedSeats[0].SeatRow);
        }

        [Test]
        public void Cannot_Get_Booked_Seats_With_Invalid_Movie_Id()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpRequest = new Mock<HttpRequestBase>();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) }
            }.AsQueryable());

            var booking = new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 };
            mock.Setup(m => m.Bookings).Returns(new List<Booking>
            {
                booking
            }.AsQueryable());

            var jsonMovie = JsonConvert.SerializeObject(new { MovieId = "Id" });

            byte[] content = Encoding.UTF8.GetBytes(jsonMovie);
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var waitingResponse = JsonConvert.SerializeObject(new { code = 400, Message = "Invalid request. Movie id must be an integer" });
            //Act           
            controller.GetMovieBookedSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, mockHttpResponse.Object.StatusCode);
            Assert.AreEqual(waitingResponse, mockHttpResponse.Object.Output.ToString());
        }

        [Test]
        public void Cannot_Get_Booked_Seats_From_Nonexistent_Movie()
        {
            //Arrange
            StringWriter output = new StringWriter();

            var mockHttpRequest = new Mock<HttpRequestBase>();

            var mockHttpResponse = new Mock<HttpResponseBase>();
            mockHttpResponse.SetupProperty(x => x.ContentType);
            mockHttpResponse.SetupProperty(x => x.StatusCode);
            mockHttpResponse.SetupGet(x => x.Output).
                Returns(output);
            mockHttpResponse.Setup(x => x.End()).
                Callback(() => output.Flush());
            mockHttpResponse.Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(s => output.Write(s));

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockHttpResponse.Object);

            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) }
            }.AsQueryable());

            var booking = new Booking { SeatNum = 1, SeatRow = 2, MovieId = 1 };
            mock.Setup(m => m.Bookings).Returns(new List<Booking>
            {
                booking
            }.AsQueryable());

            var jsonMovie = "[{\"MovieId\":2}]";

            byte[] content = Encoding.UTF8.GetBytes(jsonMovie);
            Stream stream = new MemoryStream(content);

            mockHttpRequest.Setup(x => x.InputStream).Returns(stream);
            mockHttpRequest.Setup(x => x.ContentLength).Returns(content.Length);

            var controller = new ApiController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var waitingResponse = JsonConvert.SerializeObject(new { code = 400, Message = "Invalid request. No movie with such id" });

            //Act           
            controller.GetMovieBookedSeats();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, mockHttpResponse.Object.StatusCode);
            Assert.AreEqual(waitingResponse, mockHttpResponse.Object.Output.ToString());
        }
    }
}