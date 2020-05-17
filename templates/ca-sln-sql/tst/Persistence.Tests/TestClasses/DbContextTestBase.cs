using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Sql.Tests.TestClasses
{
	/// <summary>
	/// An abstract base class to facilitate testing with <see cref="DbContext"/>s.
	/// </summary>
	/// <typeparam name="TDbContext"></typeparam>
	public abstract class DbContextTestBase<TDbContext> where TDbContext : DbContext
	{
		/// <summary>
		/// Add data required for testing to the context.
		/// </summary>
		/// <param name="context">The context to seed.</param>
		protected abstract Task SeedData(TDbContext context);

		/// <summary>
		/// Get an instance of <see cref="TDbContext"/> using EntityFrameworkCore's in-memory database.
		/// <para>
		/// Use this context when a relational database is not needed for increased performance.
		/// </para>
		/// </summary>
		/// <param name="databaseName">The name of the database to use with the context. If nothing is provided, a new GUID will be used.</param>
		protected async Task<TDbContext> GetInMemContext(string databaseName = "")
		{
			if (string.IsNullOrEmpty(databaseName))
			{
				databaseName = Guid.NewGuid().ToString();
			}

			var builder = new DbContextOptionsBuilder<TDbContext>();
			builder.UseInMemoryDatabase(databaseName).ConfigureWarnings(w =>
			{
				w.Ignore(InMemoryEventId.TransactionIgnoredWarning);
			});
			var dbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), builder.Options);
			return await InitializeDbContext(dbContext);
		}

		/// <summary>
		/// Get an instance of <see cref="TDbContext"/> using the SQLite in-memory database.
		/// <para>
		/// Use this context when a relational database is needed such as testing foreign key constraints.
		/// </para>
		/// </summary>
		protected async Task<TDbContext> GetSqliteContext()
		{
			var builder = new DbContextOptionsBuilder<TDbContext>();
			builder.UseSqlite("DataSource=:memory:", options => { });
			var dbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), builder.Options);
			dbContext.Database.OpenConnection();
			return await InitializeDbContext(dbContext);
		}

		private async Task<TDbContext> InitializeDbContext(TDbContext context)
		{
			context.Database.EnsureCreated();
			await SeedData(context);
			await context.SaveChangesAsync();
			return context;
		}
	}
}
