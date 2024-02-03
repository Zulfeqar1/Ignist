using System;
using Microsoft.EntityFrameworkCore;
using Ignist.Models;
namespace Ignist.Data
{
    public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
		public DbSet<publications> publications { get; set; }
	}
}

