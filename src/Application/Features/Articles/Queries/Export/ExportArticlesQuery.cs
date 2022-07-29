using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.Export
{
    public class ExportArticlesQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportArticlesQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportArticlesQueryHandler : IRequestHandler<ExportArticlesQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportArticlesQueryHandler> _localizer;

        public ExportArticlesQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportArticlesQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportArticlesQuery request, CancellationToken cancellationToken)
        {
            var articleFilterSpec = new ArticleFilterSpecification(request.SearchString);
            var articles = await _unitOfWork.Repository<Article>().Entities
                .Specify(articleFilterSpec)
                .ToListAsync( cancellationToken);
            var data = await _excelService.ExportAsync(articles, mappers: new Dictionary<string, Func<Article, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Title"], item => item.Title },
                { _localizer["Summary"], item => item.Summary },
                { _localizer["Description"], item => item.Description },
                { _localizer["Author"], item => item.Author }
            }, sheetName: _localizer["Articles"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}