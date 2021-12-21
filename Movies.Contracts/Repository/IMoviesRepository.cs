using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Contracts.Repository
{
    public interface IMoviesRepository
    {
        public Task<IList<MovieSummary>> GetSummariesAsync(string searchWord);
        public Task<MovieSummary> GetDetailsAsync(string id);
    }
}
