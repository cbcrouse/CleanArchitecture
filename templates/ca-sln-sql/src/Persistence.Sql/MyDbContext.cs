using Microsoft.EntityFrameworkCore;

namespace Persistence.Sql
{
	/// <summary>
	/// The context for communicating with the SQL data store.
	/// </summary>
	public class MyDbContext : DbContext
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="options">
		///     The options to be used by a <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. You normally override
		///     <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" /> or use a <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder`1" />
		///     to create instances of this class and it is not designed to be directly constructed in your application code.
		/// </param>
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

		/// <summary>
		/// Used to query and save instances of <see cref="Domain.Entities.TodoItem"/>.
		/// </summary>
		public DbSet<Domain.Entities.TodoItem> TodoItems { get; set; }

		/// <summary>
		///     Override this method to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
		///     then this method will not be run.
		/// </remarks>
		/// <param name="modelBuilder">
		///     The builder being used to construct the model for this context. Databases (and other extensions) typically
		///     define extension methods on this object that allow you to configure aspects of the model that are specific
		///     to a given database.
		/// </param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Domain.Entities.TodoItem>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Id).IsUnique();
				entity.Property(e => e.Id).ValueGeneratedNever().IsRequired();
				entity.Property(e => e.Description).IsRequired();
				entity.Property(e => e.CreatedOn).IsRequired();
				entity.Property(e => e.CompletedOn).IsRequired(false);
				entity.Property(e => e.DueOn).IsRequired(false);
			});
		}
	}
}
