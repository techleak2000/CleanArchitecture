
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Requests.News;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;  
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.Article;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.News.ArticleCategory;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll;

namespace BlazorHero.CleanArchitecture.Client.Pages.News
{
    public partial class Articles
    {                                   
        [Inject] private IArticleManager ArticleManager { get; set; }
        [Inject] private IArticleCategoryManager ArticleCategoryManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedArticlesResponse> _pagedData;
        private List<GetAllArticleCategoriesResponse> _categories = new();
        private MudTable<GetAllPagedArticlesResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateArticles;        
        private bool _canEditArticles;
        private bool _canDeleteArticles;
        private bool _canExportArticles;
        private bool _canSearchArticles;
        private bool _loaded;           

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateArticles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Articles.Create)).Succeeded;
            _canEditArticles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Articles.Edit)).Succeeded;
            _canDeleteArticles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Articles.Delete)).Succeeded;
            _canExportArticles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Articles.Export)).Succeeded;
            _canSearchArticles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Articles.Search)).Succeeded;

            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllPagedArticlesResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            await LoadArticleCategoriesAsync();
            return new TableData<GetAllPagedArticlesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] {$"{state.SortLabel} {state.SortDirection}"} : new[] {$"{state.SortLabel}"};
            }

            var request = new GetAllPagedArticlesRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await ArticleManager.GetArticlesAsync(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task LoadArticleCategoriesAsync()
        {
            var data = await ArticleCategoryManager.GetAllAsync();
            if (data.Succeeded)
            {
                _categories = data.Data;
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            var response = await ArticleManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Articles).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Articles exported"]
                    : _localizer["Filtered Articles exported"], Severity.Success);
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
                var article = _pagedData.FirstOrDefault(c => c.Id == id);
                if (article != null)
                {
                    parameters.Add(nameof(AddEditArticleModal.AddEditArticleModel), new AddEditArticleCommand
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Description = article.Description,
                        Sumary = article.Sumary,
                        MainCategoryId = article.MainCategoryId
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditArticleModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }
        private async Task InvokeCategoryModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var articleCategory = _categories.FirstOrDefault(c => c.Id == id);
                if (articleCategory != null)
                {
                    parameters.Add(nameof(AddEditArticleCategoryModal.AddEditArticleCategoryModel), new AddEditArticleCategoryCommand
                    {
                        Id = articleCategory.Id,
                        Name = articleCategory.Name,
                        Description = articleCategory.Description,
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditArticleModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await ArticleManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}