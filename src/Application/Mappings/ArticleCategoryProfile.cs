using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Domain.Entities.News;

namespace BlazorHero.CleanArchitecture.Application.Mappings
{
    public class ArticleCategoryProfile : Profile
    {
        public ArticleCategoryProfile()
        {
            CreateMap<AddEditArticleCategoryCommand, ArticleCategory>().ReverseMap();
        }
    }
}