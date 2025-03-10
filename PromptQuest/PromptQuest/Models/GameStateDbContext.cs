using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PromptQuest.Models;

namespace PromptQuest.Models {
	public class GameStateDbContext:DbContext {

		private readonly IConfiguration _configuration;

		public GameStateDbContext(DbContextOptions<GameStateDbContext> options, IConfiguration configuration) : base(options) {
			_configuration = configuration;
		}

		public DbSet<GameState> GameStates { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Enemy> Enemies { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options) {
				string connectionString = _configuration["ConnectionString-GameStateDb"]; // Get Connection-String from either usre secrets or AzureKeyVault
				options.UseSqlServer(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<GameState>()
			.HasKey(gs => gs.UserGoogleId); // User Google Id as the primary key.

			modelBuilder.Entity<GameState>()
					.HasOne(gs => gs.Player)
					.WithMany()
					.HasForeignKey(gs => gs.PlayerId)
					.IsRequired(false);  // Allow null values.

			modelBuilder.Entity<GameState>()
					.HasOne(gs => gs.Enemy)
					.WithMany()
					.HasForeignKey(gs => gs.EnemyId)
					.IsRequired(false);  // Allow null values.

			modelBuilder.Entity<Player>()
					.HasKey(gs => gs.PlayerId);

			modelBuilder.Entity<Enemy>()
					.HasKey(gs => gs.EnemyId);
		}
	}
}