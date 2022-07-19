using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetById
{
    public class GetArticleCategoryByIdQuery : IRequest<Result<GetArticleCategoryByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetArticleCategoryByIdQueryHandler : IRequestHandler<GetArticleCategoryByIdQuery, Result<GetArticleCategoryByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetArticleCategoryByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetArticleCategoryByIdResponse>> Handle(GetArticleCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var articleCategory = await _unitOfWork.Repository<ArticleCategory>().GetByIdAsync(query.Id);
            var mappedArticleCategory = _mapper.Map<GetArticleCategoryByIdResponse>(articleCategory);
            return await Result<GetArticleCategoryByIdResponse>.SuccessAsync(mappedArticleCategory);
        }
    }
}