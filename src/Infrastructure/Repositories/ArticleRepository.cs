using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IRepositoryAsync<Article, int> _repository;

        public ArticleRepository(IRepositoryAsync<Article, int> repository)
        {
            _repository = repository;
        }
        
    }
}