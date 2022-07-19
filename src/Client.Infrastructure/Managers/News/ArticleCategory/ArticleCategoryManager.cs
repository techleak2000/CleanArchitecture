using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Requests.News;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.ArticleCategory
{
    public class ArticleCategoryManager : IArticleCategoryManager
    {
        private readonly HttpClient _httpClient;

        public ArticleCategoryManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ArticleCategoriesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.ArticleCategoriesEndpoints.Export
                : Routes.ArticleCategoriesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<List<GetAllArticleCategoriesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ArticleCategoriesEndpoints.GetAll);
            return await response.ToResult<List<GetAllArticleCategoriesResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditArticleCategoryCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ArticleCategoriesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}