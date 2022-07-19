using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.ArticleCategory;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
// using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.Import;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Client.Shared.Components;

namespace BlazorHero.CleanArchitecture.Client.Pages.News
{
    public partial class Categories
    {
        [Inject] private IArticleCategoryManager ArticleCategoryManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllArticleCategoriesResponse> _categories = new();
        private GetAllArticleCategoriesResponse _category = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateCategories;
        private bool _canEditCategories;
        private bool _canDeleteCategories;
        private bool _canExportCategories;
        private bool _canSearchCategories;
        private bool _canImportCategories;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Create)).Succeeded;
            _canEditCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Edit)).Succeeded;
            _canDeleteCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Delete)).Succeeded;
            _canExportCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Export)).Succeeded;
            _canSearchCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Search)).Succeeded;
            _canImportCategories = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArticleCategories.Import)).Succeeded;

            await GetCategoriesAsync();
            _loaded = true;

            HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetCategoriesAsync()
        {
            var response = await ArticleCategoryManager.GetAllAsync();
            if (response.Succeeded)
            {
                _categories = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await ArticleCategoryManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var response = await ArticleCategoryManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Categories).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Categories exported"]
                    : _localizer["Filtered Categories exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _category = _categories.FirstOrDefault(c => c.Id == id);
                if (_category != null)
                {
                    parameters.Add(nameof(AddEditArticleCategoryModal.AddEditArticleCategoryModel), new AddEditArticleCategoryCommand
                    {
                        Id = _category.Id,
                        Name = _category.Name,
                        Description = _category.Description
                        
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditArticleCategoryModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        //private async Task<IResult<int>> ImportExcel(UploadRequest uploadFile)
        //{
        //    var request = new ImportCategoriesCommand { UploadRequest = uploadFile };
        //    var result = await ArticleCategoryManager.ImportAsync(request);
        //    return result;
        //}

        //private async Task InvokeImportModal()
        //{
        //    var parameters = new DialogParameters
        //    {
        //        { nameof(ImportExcelModal.ModelName), _localizer["Categories"].ToString() }
        //    };
        //    Func<UploadRequest, Task<IResult<int>>> importExcel = ImportExcel;
        //    parameters.Add(nameof(ImportExcelModal.OnSaved), importExcel);
        //    var options = new DialogOptions
        //    {
        //        CloseButton = true,
        //        MaxWidth = MaxWidth.Small,
        //        FullWidth = true,
        //        DisableBackdropClick = true
        //    };
        //    var dialog = _dialogService.Show<ImportExcelModal>(_localizer["Import"], parameters, options);
        //    var result = await dialog.Result;
        //    if (!result.Cancelled)
        //    {
        //        await Reset();
        //    }
        //}

        private async Task Reset()
        {
            _category = new GetAllArticleCategoriesResponse();
            await GetCategoriesAsync();
        }

        private bool Search(GetAllArticleCategoriesResponse category)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (category.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return category.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}