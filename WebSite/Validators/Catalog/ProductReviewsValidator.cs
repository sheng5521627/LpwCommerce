using FluentValidation;
using Services.Localization;
using Web.Framework.Validators;
using WebSite.Models.Catalog;

namespace WebSite.Validators.Catalog
{
    public class ProductReviewsValidator : BaseNopValidator<ProductReviewsModel>
    {
        public ProductReviewsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddProductReview.Title).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Title.Required")).When(x => x.AddProductReview != null);
            RuleFor(x => x.AddProductReview.Title).Length(1, 200).WithMessage(string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200)).When(x => x.AddProductReview != null && !string.IsNullOrEmpty(x.AddProductReview.Title));
            RuleFor(x => x.AddProductReview.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.ReviewText.Required")).When(x => x.AddProductReview != null);
        }
    }
}