using Cinema.Infrastructure.Abstract;
using Cinema.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cinema.Serialization;
using Cinema.Model;
using NUnit.Framework;

namespace CinemaTests.ViewModel
{
    [TestFixture]
    public class CinemaHallViewModelTests
    {
        [Test]
        public void Can_Get_All_Movies()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            var movie = new Movie { MovieId = 1, Name = "Фильм", ShowTime = "12:15:00" };
            
            // Act
            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            // Assert
            Assert.IsNotNull(viewModel.Movies);
            Assert.AreEqual(viewModel.Movies.Count, 1);

            Assert.AreEqual(movie.MovieId, viewModel.Movies[0].MovieId);
            Assert.AreEqual(movie.Name, viewModel.Movies[0].Name);
            Assert.AreEqual(movie.ShowTime, viewModel.Movies[0].ShowTime);
        }

        [Test]
        public void Can_Get_Movie_Booked_Seats()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            viewModel.SelectedMovie = new Movie { MovieId = 1, Name = "Фильм", ShowTime = "12:15:00" };

            // Act
            viewModel.MovieChangeCommand.Execute(null);           

            // Assert
            Assert.IsNotNull(viewModel.Hall);
            Assert.AreEqual(viewModel.Hall[0][0].IsOccupied, true);
        }

        [Test]
        public void Can_Book_Seats()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            viewModel.SelectedMovie = new Movie { MovieId = 1, Name = "Фильм", ShowTime = "12:15:00" };

            viewModel.Hall = CinemaHall.InitCinemaHall(10, 10);

            viewModel.Hall[0][0].IsOccupied = false;
            viewModel.Hall[0][0].IsSelected = true;

            // Act
            viewModel.ToBookSeatsCommand.Execute(null);

            // Assert
            Assert.AreEqual(viewModel.SelectedSeats, 0);
            Assert.AreEqual(viewModel.Hall[0][0].IsSelected, false);
            Assert.AreEqual(viewModel.Hall[0][0].IsOccupied, true);
        }

        [Test]
        public void Can_Select_Free_Seat()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            Seat seat = new Seat(1);

            viewModel.SelectedSeats = 0;
            seat.IsOccupied = false;
            seat.IsSelected = false;

            // Act
            viewModel.SeatSelectCommand.Execute(seat);

            // Assert
            Assert.AreEqual(seat.IsOccupied, false);
            Assert.AreEqual(seat.IsSelected, true);
            Assert.AreEqual(viewModel.SelectedSeats, 1);
        }

        [Test]
        public void Cannot_Select_Occupied_Seat()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            Seat seat = new Seat(1);

            viewModel.SelectedSeats = 0;
            seat.IsOccupied = true;
            seat.IsSelected = false;

            // Act
            viewModel.SeatSelectCommand.Execute(seat);

            // Assert
            Assert.AreEqual(seat.IsOccupied, true);
            Assert.AreEqual(seat.IsSelected, false);
            Assert.AreEqual(viewModel.SelectedSeats, 0);
        }

        [Test]
        public void Cannot_Select_Null_Seat()
        {
            // Arrange
            ICinemaApi API = new CinemaAPIUnderTest("Host");

            CinemaHallViewModel viewModel = new CinemaHallViewModel(API);

            Seat seat = new Seat(1);

            viewModel.SelectedSeats = 0;

            // Act
            viewModel.SeatSelectCommand.Execute(null);

            // Assert
            Assert.AreEqual(viewModel.SelectedSeats, 0);
        }

        private class CinemaAPIUnderTest : ICinemaApi
        {
            public string Host { get; }

            public CinemaAPIUnderTest(string host)
            {
                this.Host = host;
            }

            private string Send(string requestType, string request, string content)
            {
                switch (request)
                {
                    case "api/getMovies":
                        return JsonConvert.SerializeObject(
                            (new List<Movie> { new Movie { MovieId = 1, Name = "Фильм", ShowTime = "12:15:00" } }));

                    case "api/getMovieBookedSeats":
                        return JsonConvert.SerializeObject(
                            (new List<Booking> { new Booking { BookingId = 1, MovieId = 1, SeatNum = 10, SeatRow = 1 } }));

                    case "api/toBookSeats":
                        return JsonConvert.SerializeObject(new { result = "ok", Message = "Seats books" });

                    default:
                        return null;
                }
            }

            public async Task<string> SendRequest(string requestType, string request, string content)
            {
                return Send(requestType, request, content);                
            }
        }
    }
}
