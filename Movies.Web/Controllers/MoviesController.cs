using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Contracts.Repository;
using Movies.Web.Models.Movies;

namespace Movies.Web.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesController(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository ?? throw new ArgumentNullException(nameof(moviesRepository));
        }

        public async Task<IActionResult> Index()
        {
            var summaries = await _moviesRepository.GetSummariesAsync("Avengers");
            
            var model = new SearchResultsModel();
            model.Results = summaries.Select(s => new SearchResultModel
            {
                Id = s.Id,
                ImageUrl = s.ImageUrl,
                Year = s.Year,
                Title = s.Title
            }).ToList();

            return View(model);
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
