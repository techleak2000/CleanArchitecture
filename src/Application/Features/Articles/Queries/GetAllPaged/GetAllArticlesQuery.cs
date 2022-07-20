using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Specifications.News;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged
{
    public class GetAllArticlesQuery : IRequest<PaginatedResult<GetAllPagedArticlesResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllArticlesQuery(int pageNumber, int pageSize, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, PaginatedResult<GetAllPagedArticlesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllArticlesQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedArticlesResponse>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {

            Expression<Func<Article, GetAllPagedArticlesResponse>> expression = e => new GetAllPagedArticlesResponse
            {
                Id = e.Id,
                Title = e.Title,
                Summary = e.Summary,
                Description = e.Description,
                MainCategoryId = e.MainCategoryId
            };
            var articleFilterSpec = new ArticleFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Article>().Entities
                   .Specify(articleFilterSpec)
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Article>().Entities
                   .Specify(articleFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;

            }
        }
    }
}