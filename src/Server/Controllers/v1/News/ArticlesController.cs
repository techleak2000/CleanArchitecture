using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Commands.Delete;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetArticleImage;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.News
{
    public class ArticlesController : BaseApiController<ArticlesController>
    {
        /// <summary>
        /// Get All Articles
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Articles.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var products = await _mediator.Send(new GetAllArticlesQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(products);
        }

        /// <summary>
        /// Get a Article Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Articles.View)]
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetArticleImageAsync(int id)
        {
            var result = await _mediator.Send(new GetArticleImageQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Article
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Articles.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditArticleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Article
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Articles.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteArticleCommand { Id = id }));
        }

        /// <summary>
        /// Search Articles and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Articles.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportArticlesQuery(searchString)));
        }
    }
}