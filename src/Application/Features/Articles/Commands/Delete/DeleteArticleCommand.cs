using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.Delete
{
    public class DeleteArticleCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteArticleCommandHandler> _localizer;

        public DeleteArticleCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteArticleCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteArticleCommand command, CancellationToken cancellationToken)
        {
            var article = await _unitOfWork.Repository<Article>().GetByIdAsync(command.Id);
            if (article != null)
            {
                await _unitOfWork.Repository<Article>().DeleteAsync(article);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(article.Id, _localizer["Article Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Article Not Found!"]);
            }
        }
    }
}