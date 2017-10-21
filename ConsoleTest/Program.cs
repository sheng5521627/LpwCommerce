using Core;
using Core.Configuration;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Core.Caching;
using System.Threading;
using Core.ComponentModel;
using Web.Framework;
using Data;
using Core.Domain.Stores;
using Core.Data;
using System.Data.Entity;
using Services.Helpers;
using Core.Domain.Blogs;
using Services.Configuration;
using Services.Events;
using ConsoleTest.Settings;
using ConsoleTest.Stores;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //RemotePost q = new RemotePost();
            //EngineContext.Initialize(false);
            using (var context = new MyDbContext())
            {
                var school = context.Schools.FirstOrDefault();
            }

                Console.ReadLine();
        }
    }
    public class MyDbContext : DbContext
    {
        public MyDbContext()
            : base(nameOrConnectionString: "Data Source=.;Initial Catalog=MyFirst;Integrated Security=True")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>().HasKey(m => m.Id).ToTable("School");
            modelBuilder.Entity<Student>().HasKey(m => m.Id).ToTable("Student").HasRequired(m => m.School).WithMany(m => m.Students).HasForeignKey(m => m.SchoolId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
    }

    public class School
    {
        private ICollection<Student> _students;

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Student> Students
        {
            get { return _students??(_students=new List<Student>()); }
            set { _students = value; }
        }
    }

    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SchoolId { get; set; }

        public virtual School School { get; set; }
    }
}
