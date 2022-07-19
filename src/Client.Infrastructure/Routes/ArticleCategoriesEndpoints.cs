
namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes
{
    public static class ArticleCategoriesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string GetAll = "api/v1/articlecategories";
        public static string Save = "api/v1/articlecategories";
        public static string Delete = "api/v1/articlecategories";
        public static string Export = "api/v1/articlecategories/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}