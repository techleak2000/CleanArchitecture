using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.News;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;

namespace BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit
{
    public partial class AddEditArticleCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        public string ImageDataURL { get; set; }
        

    }

    internal class AddEditArticleCategoryCommandHandler : IRequestHandler<AddEditArticleCategoryCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditArticleCategoryCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditArticleCategoryCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditArticleCategoryCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditArticleCategoryCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var category = _mapper.Map<ArticleCategory>(command);
                await _unitOfWork.Repository<ArticleCategory>().AddAsync(category);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllArticleCategoriesCacheKey);
                return await Result<int>.SuccessAsync(category.Id, _localizer["ArticleCategory Saved"]);
            }
            else
            {
                var category = await _unitOfWork.Repository<ArticleCategory>().GetByIdAsync(command.Id);
                if (category != null)
                {
                    category.Name = command.Name ?? category.Name;
                    category.Description = command.Description ?? category.Description;
                    await _unitOfWork.Repository<ArticleCategory>().UpdateAsync(category);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllArticleCategoriesCacheKey);
                    return await Result<int>.SuccessAsync(category.Id, _localizer["ArticleCategory Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["ArticleCategory Not Found!"]);
                }
            }
        }
    }
}