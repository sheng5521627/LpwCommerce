using FluentValidation;
using Services.Localization;
using Web.Framework.Validators;
using WebSite.Models.Blogs;

namespace WebSite.Validators.Blogs
{
    public class BlogPostValidator : BaseNopValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessage(localizationService.GetResource("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }}
}