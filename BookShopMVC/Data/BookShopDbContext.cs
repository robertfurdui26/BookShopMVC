using BookShopMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShopMVC.Data
{
    public class BookShopDbContext : DbContext
    {
        public BookShopDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<BookModel>BookShoop { get; set; }

        public DbSet<Person> PersonBookShoop { get; set;}
    }
}
