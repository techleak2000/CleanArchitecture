using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll
{
    public class GetAllArticleCategoriesQuery : IRequest<Result<List<GetAllArticleCategoriesResponse>>>
    {
        public GetAllArticleCategoriesQuery()
        {
        }
    }

    internal class GetAllArticleCategoriesCachedQueryHandler : IRequestHandler<GetAllArticleCategoriesQuery, Result<List<GetAllArticleCategoriesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllArticleCategoriesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllArticleCategoriesResponse>>> Handle(GetAllArticleCategoriesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<ArticleCategory>>> getAllArticleCategories = () => _unitOfWork.Repository<ArticleCategory>().GetAllAsync();
            var cateloryList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllArticleCategoriesCacheKey, getAllArticleCategories);
            var mappedArticleCategories = _mapper.Map<List<GetAllArticleCategoriesResponse>>(cateloryList);
            return await Result<List<GetAllArticleCategoriesResponse>>.SuccessAsync(mappedArticleCategories);
        }
    }
}