namespace BlazorHero.CleanArchitecture.Application.Requests.News
{
    public class GetAllPagedArticleCategoriesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}