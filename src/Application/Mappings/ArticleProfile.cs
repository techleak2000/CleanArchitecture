using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Domain.Entities.News;

namespace BlazorHero.CleanArchitecture.Application.Mappings
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<AddEditArticleCommand, Article>().ReverseMap();
        }
    }
}