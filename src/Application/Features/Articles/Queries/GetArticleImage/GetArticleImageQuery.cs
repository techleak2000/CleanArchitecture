using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetArticleImage
{
    public class GetArticleImageQuery : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public GetArticleImageQuery(int articleId)
        {
            Id = articleId;
        }
    }

    internal class GetArticleImageQueryHandler : IRequestHandler<GetArticleImageQuery, Result<string>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetArticleImageQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetArticleImageQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<Article>().Entities.Where(p => p.Id == request.Id).Select(a => a.ImageDataURL).FirstOrDefaultAsync(cancellationToken);
            return await Result<string>.SuccessAsync(data: data);
        }
    }
}