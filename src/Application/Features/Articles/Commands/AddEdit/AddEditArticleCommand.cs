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
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;

namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit
{
    public partial class AddEditArticleCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public string Sumary { get; set; }

        public string ImageDataURL { get; set; }

        public int MainCategoryId { get; set; }

        public UploadRequest UploadRequest { get; set; }
    }

    internal class AddEditArticleCommandHandler : IRequestHandler<AddEditArticleCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditArticleCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditArticleCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, IStringLocalizer<AddEditArticleCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _uploadService = uploadService;
        }
        public async Task<Result<int>> Handle(AddEditArticleCommand command, CancellationToken cancellationToken)
        {
            var uploadRequest = command.UploadRequest;
            if (uploadRequest != null)
            {
                uploadRequest.FileName = $"P-{command.Title}{uploadRequest.Extension}";
            }


            if (command.Id == 0)
            {
                var article = _mapper.Map<Article>(command);
                if (uploadRequest != null)
                {
                    article.ImageDataURL = _uploadService.UploadAsync(uploadRequest);
                }
                await _unitOfWork.Repository<Article>().AddAsync(article);
                return await Result<int>.SuccessAsync(article.Id, _localizer["Article Saved"]);
            }
            else
            {
                var article = await _unitOfWork.Repository<Article>().GetByIdAsync(command.Id);
                if (article != null)
                {
                    article.Title = command.Title ?? article.Title;
                    article.Sumary = command.Sumary ?? article.Sumary;
                    article.Description = command.Description ?? article.Description;
                    if (uploadRequest != null)
                    {
                        article.ImageDataURL = _uploadService.UploadAsync(uploadRequest);
                    }
                    article.ImageDataURL = command.ImageDataURL ?? article.ImageDataURL;
                    await _unitOfWork.Repository<Article>().UpdateAsync(article);
                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<int>.SuccessAsync(article.Id, _localizer["Article Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Article Not Found!"]);
                }
            }
        }
    }
}