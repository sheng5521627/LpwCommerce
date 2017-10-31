using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Framework.Mvc;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(Student model)
        {
            TypeConverter t = TypeDescriptor.GetConverter(typeof(List<int>));
            if (ModelState.IsValid)
            {

            }
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => new Student(), typeof(Student));
            var list = ModelValidatorProviders.Providers.GetValidators(metadata, this.ControllerContext).ToList();
            foreach(var item in list)
            {
                var resluts = item.Validate(new Student()).FirstOrDefault();
            }
            MyValidator v = new Controllers.MyValidator();
            var a= v.Validate(new Controllers.Student());

            return View();
        }

        [HttpPost]
        public ActionResult Login(Student model)
        {
            if (ModelState.IsValid)
            {

            }
            return Content("");
        }
    }

    [Validator(typeof(MyValidator))]
    public class Student
    {
        public string Name { get; set; }
    }

    public class MyValidator : AbstractValidator<Student>
    {
        public MyValidator()
        {
            RuleFor(m => m.Name).NotNull().MinimumLength(2).MaximumLength(5);
        }
    }
}