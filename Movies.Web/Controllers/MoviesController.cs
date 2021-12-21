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

        [HttpGet]
        public IActionResult Index()
        {
            var model = new SearchResultsModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            var summaries = await _moviesRepository.GetSummariesAsync(search);
            
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

        public async Task<IActionResult> Details(string id)
        {
            var details = await _moviesRepository.GetDetailsAsync(id);

            // reusing for simplicity
            var model = new SearchResultModel
            {
                Id = details.Id,
                ImageUrl = details.ImageUrl,
                Year = details.Year,
                Title = details.Title
            };

            return View(model);
        }
    }
}
