using System.Collections.Generic;

namespace Movies.Web.Models.Movies
{
    public class SearchResultsModel
    {
        public IList<SearchResultModel> Results { get; set; }
    }
}