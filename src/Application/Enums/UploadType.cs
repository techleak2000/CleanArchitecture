using System.ComponentModel;

namespace BlazorHero.CleanArchitecture.Application.Enums
{
    public enum UploadType : byte
    {
        [Description(@"Images\Articles")]
        Article,
        [Description(@"Images\Products")]
        Product,

        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }
}