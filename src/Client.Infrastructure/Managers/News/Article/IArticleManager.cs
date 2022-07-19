using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Requests.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.Article
{
    public interface IArticleManager : IManager
    {
        Task<PaginatedResult<GetAllPagedArticlesResponse>> GetArticlesAsync(GetAllPagedArticlesRequest request);

        Task<IResult<string>> GetArticleImageAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditArticleCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}