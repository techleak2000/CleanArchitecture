namespace BlazorHero.CleanArchitecture.Application.Requests.News
{
    public class GetAllPagedArticlesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}