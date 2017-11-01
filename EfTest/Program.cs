using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDbContext dbContext = new MyDbContext();
            bool exists =  dbContext.Database.Exists();
                        

            Console.ReadLine();
        }
    }

    public class A
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
    public class B
    {
        public int Id { get; set; }
        public int Aid { get; set; }
        public int Age { get; set; }
    }
    
    public class MyDbInitialize : IDatabaseInitializer<MyDbContext>
    {
        public void InitializeDatabase(MyDbContext context)
        {

        }
    }
    public class MyDbContext : DbContext
    {
        public MyDbContext()
            : base(nameOrConnectionString: "Data Source=(local);Initial Catalog=nop;Integrated Security=True")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerRole> CustomerRoles { get; set; }
        public DbSet<History> Historys { get; set; }

        public DbSet<Person> Person { get; set; }
        public DbSet<Address> Address { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private ICollection<CustomerRole> _customerRoles;
        public virtual ICollection<CustomerRole> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole>()); }
            set { _customerRoles = value; }
        }
    }

    public class CustomerRole
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private ICollection<Customer> _customers;

        public virtual ICollection<Customer> Customers
        {
            get { return _customers ?? (_customers = new List<Customer>()); }
            set { _customers = value; }
        }
    }

    public class History
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Person Person { get; set; }
    }
}
