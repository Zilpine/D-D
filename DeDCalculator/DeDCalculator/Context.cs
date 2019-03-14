using System;
using System.Diagnostics;
using DeDCalculator.Data;
using DeDCalculator.Data.DAL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DeDCalculator
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
	{
		public Context CreateDbContext(string[] args)
		{
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			Debug.WriteLine(basePath);
			string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{envName}.json", true)
				.Build();

			string connectionString = configuration.GetConnectionString("DefaultConnection");
			var builder = new DbContextOptionsBuilder<Context>();

			//builder.UseMySql(connectionString);
			builder.UseSqlite("Data Source=nanana.db"); // .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NaNaNaTest;Trusted_Connection=True;MultipleActiveResultSets=true");
			return new Context(builder.Options);
		}
	}

	public class Context : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public Context(DbContextOptions options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlite("Data Source=nanana.db");
			}
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.UseOpenIddict<Guid>();

			//modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(x => x.UserId);

			//modelBuilder.Entity<SongEntity>().HasIndex(x => x.OriginalFileName).IsUnique();
			//modelBuilder.Entity<DeviceEntity>().HasIndex(x => x.MUID).IsUnique();

			//#region TokenEntities

			//modelBuilder.Entity<TokenEntity>().Property(b => b.Token).HasMaxLength(25);
			//modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Payments)
			//	.WithOne().HasForeignKey(x => x.UserId);
			//modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Tokens)
			//	.WithOne().HasForeignKey(x => x.UserId);

			//#endregion

			//modelBuilder.Entity<SongDetailTraceEntity>()
			//	.HasKey(x => new
			//	{
			//		x.UserDeviceId,
			//		x.SongFragmentId,
			//		x.Date
			//	});

			//modelBuilder.Entity<SongDetailTraceEntity>()
			//	.HasOne(x => x.UserDevice)
			//	.WithMany()
			//	.HasForeignKey(x => x.UserDeviceId);

			//modelBuilder.Entity<SongDetailTraceEntity>()
			//	.HasOne(x => x.SongFragment)
			//	.WithMany()
			//	.HasForeignKey(x => x.SongFragmentId);


			//modelBuilder.Entity<PlaylistEntity>().HasMany<PlaylistSongEntity>().WithOne(x => x.Playlist)
			//	.HasForeignKey(x => x.PlaylistId).OnDelete(DeleteBehavior.Cascade);

			//modelBuilder.Entity<PlaylistSongEntity>().HasMany<SongEntity>().WithOne(x=>x.s)
			//	.HasForeignKey(x => x.SongId).OnDelete(DeleteBehavior.Restrict);
			base.OnModelCreating(modelBuilder);
		}
	}
}
