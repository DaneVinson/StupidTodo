using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class TodosDbContext : DbContext
    {
        private readonly CosmosDBOptions _options;

        public TodosDbContext(CosmosDBOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                            connectionString: _options.ConnectionString,
                            databaseName: _options.DatabaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("todos");
            modelBuilder
                .Entity<Todo>()
                .HasPartitionKey(t => t.UserId);
        }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
