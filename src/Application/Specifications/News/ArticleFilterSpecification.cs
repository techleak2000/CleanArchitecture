using BlazorHero.CleanArchitecture.Application.Specifications.Base;
using BlazorHero.CleanArchitecture.Domain.Entities.News;

namespace BlazorHero.CleanArchitecture.Application.Specifications.News
{
    public class ArticleFilterSpecification : HeroSpecification<Article>
    {
        public ArticleFilterSpecification(string searchString)
        {
            Includes.Add(a => a.Category);
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.Title.Contains(searchString) || p.Description.Contains(searchString) || p.Summary.Contains(searchString) );
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}