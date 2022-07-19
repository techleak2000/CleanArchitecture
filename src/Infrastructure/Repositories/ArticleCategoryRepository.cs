using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories
{
    public class ArticleCategoryRepository : IArticleCategoryRepository
    {
        private readonly IRepositoryAsync<ArticleCategory, int> _repository;

        public ArticleCategoryRepository(IRepositoryAsync<ArticleCategory, int> repository)
        {
            _repository = repository;
        }

       
    }
}