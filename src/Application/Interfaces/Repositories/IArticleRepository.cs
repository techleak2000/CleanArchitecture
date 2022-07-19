using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IArticleRepository
    {
        Task<bool> IsArticleCategoryUsed(int categoryId);
    }
}