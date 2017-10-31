using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using ImageResizer;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Student student = new Student() { Name = "1",Age = 9 };

            StudentValidator validator = new StudentValidator();
            ValidationResult result = validator.Validate(student);
            if (result.IsValid)
            {
                Console.WriteLine("验证成功");
            }
            else
            {
                Console.WriteLine("验证失败");
                foreach (var item in result.Errors)
                {
                    Console.WriteLine(item.ErrorMessage);
                }
            }

            Console.ReadLine();

        }
    }

    public class Student
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
    
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.Name).IsCreditCard();
        }
    }

    public class MyPropertyValidator : PropertyValidator
    {
        public MyPropertyValidator()
            : base("长度为2-10个字符")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            string value = context.PropertyValue as string;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length < 2 || value.Length > 10)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public static class MyValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsCreditCard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MyPropertyValidator());
        }
    }
}
