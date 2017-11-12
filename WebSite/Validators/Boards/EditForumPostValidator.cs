using FluentValidation;
using Services.Localization;
using Web.Framework.Validators;
using WebSite.Models.Boards;

namespace WebSite.Validators.Boards
{
    public class EditForumPostValidator : BaseNopValidator<EditForumPostModel>
    {
        public EditForumPostValidator(ILocalizationService localizationService)
        {            
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}