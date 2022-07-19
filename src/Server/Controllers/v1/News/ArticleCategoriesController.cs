using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.GetById;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ArticleCategorys.Commands.Delete;
//using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Queries.Export;
//using BlazorHero.CleanArchitecture.Application.Features.ArticleCategories.Commands.Import;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.News
{
    public class ArticleCategoriesController : BaseApiController<ArticleCategoriesController>
    {
        /// <summary>
        /// Get All ArticleCategories
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ArticleCategories.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllArticleCategoriesQuery());
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.ArticleCategories.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetArticleCategoryByIdQuery() { Id = id });
            return Ok(brand);
        }

        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ArticleCategories.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditArticleCategoryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ArticleCategories.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteArticleCategoryCommand { Id = id }));
        }

        ///// <summary>
        ///// Search ArticleCategories and Export to Excel
        ///// </summary>
        ///// <param name="searchString"></param>
        ///// <returns></returns>
        //[Authorize(Policy = Permissions.ArticleCategories.Export)]
        //[HttpGet("export")]
        //public async Task<IActionResult> Export(string searchString = "")
        //{
        //    return Ok(await _mediator.Send(new ExportArticleCategoriesQuery(searchString)));
        //}

        ///// <summary>
        ///// Import ArticleCategories from Excel
        ///// </summary>
        ///// <param name="command"></param>
        ///// <returns></returns>
        //[Authorize(Policy = Permissions.ArticleCategories.Import)]
        //[HttpPost("import")]
        //public async Task<IActionResult> Import(ImportArticleCategoriesCommand command)
        //{
        //    return Ok(await _mediator.Send(command));
        // }
    }
}