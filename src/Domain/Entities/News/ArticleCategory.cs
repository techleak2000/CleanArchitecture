using BlazorHero.CleanArchitecture.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorHero.CleanArchitecture.Domain.Entities.News
{
    public class ArticleCategory : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "text")]
        public string ImageDataURL { get; set; }
    }
}
