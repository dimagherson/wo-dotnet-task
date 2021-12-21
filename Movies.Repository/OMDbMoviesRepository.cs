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
            var url = GetUrl();

            if (!string.IsNullOrWhiteSpace(searchWord))
            {
                url = $"{url}&s={searchWord}";
            }

            var rawResult = await FetchAsync<OMDbRawResult>(url);

            return rawResult?.ToSummaries();
        }

        public async Task<MovieSummary> GetDetailsAsync(string id)
        {
            var url = GetUrl();

            if (!string.IsNullOrWhiteSpace(id))
            {
                url = $"{url}&i={id}";
            }

            var rawResult = await FetchAsync<OMDbRawItemResult>(url);

            return rawResult?.ToSummary();
        }

        private string GetUrl()
        {
            return $"{_options.Url}?apiKey={_options.Key}";
        }

        private async Task<T> FetchAsync<T>(string url)
        {
            try
            {
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return default; // Soft failure. Alternatively can throw exception
                }

                var result = await response.Content.ReadAsStreamAsync();

                var rawResult = _parser.Parse<T>(result);

                return rawResult;
            }
            catch (Exception ex)
            {
                // logging...
                return default;
            }
        }

        private class OMDbRawResult
        {
            public List<OMDbRawItemResult> Search { get; set; } = new List<OMDbRawItemResult>();
            public int totalResults { get; set; }
            public bool Response { get; set; }

            public IList<MovieSummary> ToSummaries()
            {
                return Search.Select(x => x.ToSummary()).ToList();
            }
        }

        // cheating a bit, reusing this in 'details' for simplicity
        private class OMDbRawItemResult
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string ImdbID { get; set; }
            public string Type { get; set; }
            public string Poster { get; set; }

            public MovieSummary ToSummary()
            {
                return new MovieSummary
                {
                    Id = ImdbID,
                    Title = Title,
                    ImageUrl = Poster,
                    Year = Year
                };
            }
        }
    }
}
