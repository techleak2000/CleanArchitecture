namespace BlazorHero.CleanArchitecture.Application.Features.Articles.Queries.GetAllPaged
{
    public class GetAllPagedArticlesResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string Sumary { get; set; }

        public string ImageDataURL { get; set; }

        public int MainCategoryId { get; set; }
    }
}