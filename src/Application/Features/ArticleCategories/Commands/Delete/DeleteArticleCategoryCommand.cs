using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;

namespace BlazorHero.CleanArchitecture.Application.Features.ArticleCategorys.Commands.Delete
{
    public class DeleteArticleCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteArticleCategoryCommandHandler : IRequestHandler<DeleteArticleCategoryCommand, Result<int>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IStringLocalizer<DeleteArticleCategoryCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteArticleCategoryCommandHandler(IUnitOfWork<int> unitOfWork, IArticleRepository articleRepository, IStringLocalizer<DeleteArticleCategoryCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _articleRepository = articleRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteArticleCategoryCommand command, CancellationToken cancellationToken)
        {
            var isArticleCategoryUsed = await _articleRepository.IsArticleCategoryUsed(command.Id);
            if (!isArticleCategoryUsed)
            {
                var category = await _unitOfWork.Repository<ArticleCategory>().GetByIdAsync(command.Id);
                if (category != null)
                {
                    await _unitOfWork.Repository<ArticleCategory>().DeleteAsync(category);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllArticleCategoriesCacheKey);
                    return await Result<int>.SuccessAsync(category.Id, _localizer["ArticleCategory Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["ArticleCategory Not Found!"]);
                }
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
            }
        }
    }
}