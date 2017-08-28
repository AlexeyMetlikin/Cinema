using System;
using NUnit.Framework;
using Moq;
using CinemaServer.Models.Abstract;
using CinemaServer.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using CinemaServer.Controllers;
using System.Web.Mvc;

namespace CinemaServer.Tests
{
    [TestFixture]
    public class CinemaControllerTests
    {
        [Test]
        public void Can_Show_All_Movies_By_Index()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie> {
                new Movie { MovieId = 1, Name = "T1"},
                new Movie { MovieId = 2, Name = "T2"},
                new Movie { MovieId = 3, Name = "T3"},
            }.AsQueryable());

            var controller = new CinemaController(mock.Object);

            //Act
            var result = (controller.Index().Model as IEnumerable<Movie>).ToArray();

            // Assert 
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("T1", result[0].Name);
            Assert.AreEqual("T2", result[1].Name);
            Assert.AreEqual("T3", result[2].Name);
        }

        [Test]
        public void Can_Delete_Existent_Movie()
        {
            //Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0) },
                new Movie { MovieId = 3, Name = "T3", ShowTime = new TimeSpan(14, 0, 0) },
                new Movie { MovieId = 4, Name = "T4", ShowTime = new TimeSpan(15, 0, 0) },
                new Movie { MovieId = 5, Name = "T5", ShowTime = new TimeSpan(16, 0, 0) }
            }.AsQueryable());

            var controller = new CinemaController(mock.Object);

            //Act           
            var result = controller.DeleteMovie(1);

            //Assert
            mock.Verify(movies => movies.DeleteMovie(It.IsAny<int>()), Times.Once());
            Assert.IsInstanceOf<ActionResult>(result);
        }

        [Test]
        public void Cannot_Delete_Nonexistent_Movie()
        {
            //Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "T1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "T2", ShowTime = new TimeSpan(13, 0, 0)  },
            }.AsQueryable());

            var controller = new CinemaController(mock.Object);

            //Act
            var result = controller.DeleteMovie(3);

            //Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
            Assert.AreEqual((result as HttpNotFoundResult).StatusCode, 404);
        }

        [Test]
        public void Can_Create_Movie()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();

            var controller = new CinemaController(mock.Object);

            // Act
            var movie = controller.CreateMovie().Model as Movie;

            // Assert
            Assert.IsNotNull(movie);
            Assert.AreEqual(0, movie.MovieId);
            Assert.AreEqual(null, movie.Name);
            Assert.AreEqual(new TimeSpan(), movie.ShowTime);
            Assert.AreEqual(0, movie.Bookings.Count);
        }                

        [Test]
        public void Can_Edit_Movie()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(m => m.Movies).Returns(new List<Movie>
            {
                new Movie { MovieId = 1, Name = "P1", ShowTime = new TimeSpan(12, 0, 0) },
                new Movie { MovieId = 2, Name = "P2", ShowTime = new TimeSpan(13, 0, 0) },
                new Movie { MovieId = 3, Name = "P3", ShowTime = new TimeSpan(14, 0, 0) }
            }.AsQueryable());
            
            var controller = new CinemaController(mock.Object);

            // Act
            var movieResult1 = controller.EditMovie(1).Model as Movie;
            var movieResult2 = controller.EditMovie(2).Model as Movie;
            var movieResult3 = controller.EditMovie(3).Model as Movie;

            // Assert
            Assert.AreEqual(1, movieResult1.MovieId);
            Assert.AreEqual(2, movieResult2.MovieId);
            Assert.AreEqual(3, movieResult3.MovieId);
        }

        [Test]
        public void Cannot_Edit_Nonexistent_Movie()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();
            mock.Setup(movie => movie.Movies).Returns(new List<Movie> {
                new Movie { MovieId= 1, Name = "P1"},
                new Movie { MovieId = 2, Name = "P2"},
                new Movie { MovieId = 3, Name = "P3"},
            }.AsQueryable());

            var controller = new CinemaController(mock.Object);

            // Act 
            var movieResult = controller.EditMovie(4).Model;

            // Assert 
            Assert.IsNull(movieResult);
        }

        [Test]
        public void Can_Save_Valid_Movie()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();        
            var controller = new CinemaController(mock.Object);
            var movie = new Movie { MovieId = 1, Name = "Приключения Шурика", ShowTime = new TimeSpan(12, 0, 0) };

            // Act
            var result = controller.EditMovie(movie);

            // Assert
            mock.Verify(m => m.SaveMovie(movie), Times.Once());
        }

        [Test]
        public void Cannot_Save_Invalid_Movie()
        {
            // Arrange
            var mock = new Mock<ICinemaRepository>();        
            var controller = new CinemaController(mock.Object);
            var movie = new Movie { MovieId = 1, Name = "Приключения Шурика", ShowTime = new TimeSpan(12, 0, 0) };

            controller.ModelState.AddModelError("Error", "Error in model");

            // Act
            var result = controller.EditMovie(movie);

            // Assert
            mock.Verify(m => m.SaveMovie(movie), Times.Never());
        }
    }
}
