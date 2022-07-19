using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Requests.News;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.Article
{
    public class ArticleManager : IArticleManager
    {
        private readonly HttpClient _httpClient;

        public ArticleManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ArticlesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.ArticlesEndpoints.Export
                : Routes.ArticlesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> GetArticleImageAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.ArticlesEndpoints.GetArticleImage(id));
            return await response.ToResult<string>();
        }

        public async Task<PaginatedResult<GetAllPagedArticlesResponse>> GetArticlesAsync(GetAllPagedArticlesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.ArticlesEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedArticlesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditArticleCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ArticlesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}