namespace Labrary.RESTful.API.Persistence
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }
        public DbSet<Book>? Books { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Book>().HasKey(b => b.BookId);
            builder.Entity<Book>().Property(b => b.BookId).IsRequired();
            builder.Entity<Book>().Property(b => b.Tematic).HasMaxLength(500);
        }
    }
}
