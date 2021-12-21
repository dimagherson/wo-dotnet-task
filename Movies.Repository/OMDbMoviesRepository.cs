using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Movies.Contracts.Repository;

namespace Movies.Repository
{
    public class OMDbMoviesRepository : IMoviesRepository
    {
        private readonly OMDbApiOptions _options;
        private readonly IParser _parser;

        public OMDbMoviesRepository(OMDbApiOptions options, IParser parser)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public async Task<IList<MovieSummary>> GetSummariesAsync(string searchWord)
        {
            var url = $"{_options.Url}?apiKey={_options.Key}";

            if (!string.IsNullOrWhiteSpace(searchWord))
            {
                url = $"{url}&s={searchWord}";
            }

            try
            {
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null; // Soft failure. Alternatively can throw exception
                }

                var result = await response.Content.ReadAsStreamAsync();

                var rawResult = _parser.Parse<OMDbRawResult>(result);

                return rawResult.ToSummaries();
            }
            catch (Exception ex)
            {
                // logging...
                return null;
            }
        }

        private class OMDbRawResult
        {
            public List<Item> Search { get; set; } = new List<Item>();
            public int totalResults { get; set; }
            public bool Response { get; set; }

            public IList<MovieSummary> ToSummaries()
            {
                return Search.Select(x => new MovieSummary
                {
                    Id = x.ImdbID,
                    Title = x.Title,
                    ImageUrl = x.Poster,
                    Year = x.Year
                }).ToList();
            }
        }

        private class Item
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string ImdbID { get; set; }
            public string Type { get; set; }
            public string Poster { get; set; }
        }
    }
}
