using CinemaServer.Models.Abstract;
using CinemaServer.Models.Entities;
using System.Web.Mvc;
using System.Linq;

namespace CinemaServer.Controllers
{
    public class CinemaController : Controller
    {
        private ICinemaRepository _repository;

        public CinemaController(ICinemaRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            return View(_repository.Movies);
        }

        [HttpGet]
        public ViewResult EditMovie(int MovieId)
        {
            ViewBag.Title = "Редактирование фильма";
            Movie movie = _repository.Movies.FirstOrDefault(m => m.MovieId == MovieId);
            return View(movie);
        }

        [HttpPost]
        public ActionResult EditMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _repository.SaveMovie(movie);
                return RedirectToAction("Index");
            }
            return View("EditMovie", movie);
        }

        [HttpPost]
        public ActionResult DeleteMovie(int MovieId)
        {
            Movie deletedMovie = _repository.DeleteMovie(MovieId);
            if (deletedMovie == null)
            {
                return HttpNotFound();
            }
            TempData["message"] = string.Format("Фильм \"{0}\" удален", deletedMovie.Name);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult CreateMovie()
        {
            ViewBag.Title = "Создание фильма";
            return View("EditMovie", new Movie());
        }
    }
}