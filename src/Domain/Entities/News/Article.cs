using BlazorHero.CleanArchitecture.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Domain.Entities.News
{
    public class Article : AuditableEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Sumary { get; set; }

        [Column(TypeName = "text")]
        public string ImageDataURL { get; set; }

        public string Author { get; set; }

        public int MainCategoryId { get; set; }

        public virtual ArticleCategory Category { get; set; }

    }
}
