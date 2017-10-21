using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>());
            //RemotePost q = new RemotePost();
            //EngineContext.Initialize(false);
            using (var context = new MyDbContext())
            {
                Person p = new EfTest.Person() { Name = "蓝平旺" };
                Address a = new Address() { Name = "深圳" };
                p.Address = a;
                context.Person.Add(p);
                context.SaveChanges();
            }
            Console.Write("success");
            Console.ReadLine();
        }
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
            : base(nameOrConnectionString: "Data Source=.;Initial Catalog=MyTest;Integrated Security=True;")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Person>().ToTable("Person").HasKey(m => m.Id).HasRequired(m => m.Address).WithRequiredPrincipal(m=>m.Person);
            modelBuilder.Entity<Address>().ToTable("Address").HasKey(m => m.Id);
            
            modelBuilder.Entity<Customer>()
                .HasKey(m => m.Id).ToTable("Customer")
                .HasMany(m => m.CustomerRoles)
                .WithMany().Map(m => m.ToTable("customer_roles"));
            modelBuilder.Entity<CustomerRole>().HasKey(m => m.Id).ToTable("CustomerRole").HasMany(m => m.Customers).WithMany(m=>m.CustomerRoles);

            modelBuilder.Entity<History>().ToTable("History").HasKey(m => m.Id).HasRequired(m => m.Customer).WithMany().HasForeignKey(m => m.CustomerId);


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

        public Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Person Person { get; set; }

        public int PersonId { get; set; }
    }
}
