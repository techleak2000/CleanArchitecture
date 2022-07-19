using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Requests.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.ArticleCategory
{
    public interface IArticleCategoryManager : IManager
    {
        
        Task<IResult<List<GetAllArticleCategoriesResponse>>> GetAllAsync();


        Task<IResult<int>> SaveAsync(AddEditArticleCategoryCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}