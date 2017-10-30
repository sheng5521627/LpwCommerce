using FluentValidation;
using FluentValidation.Results;
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
            Student student = new Student() { Name = "111111",Age = 9 };

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
            RuleFor(s => s.Name).NotEmpty().MinimumLength(5).MaximumLength(10).WithName("字符串不能为空，长度为5-10个字");
            RuleFor(s => s.Age).NotEmpty().LessThanOrEqualTo(10).GreaterThanOrEqualTo(8).WithName("年龄范围：8-10");
        }
    }
}
