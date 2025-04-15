using Microsoft.EntityFrameworkCore;

namespace PromptQuest.Models {
	public class GameStateDbContext:DbContext {
		public GameStateDbContext(DbContextOptions<GameStateDbContext> options) : base(options) { }

		public DbSet<GameState> GameStates { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Enemy> Enemies { get; set; }
		public DbSet<Item> Items { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			//GameStates table
			modelBuilder.Entity<GameState>().HasKey(gs => gs.UserGoogleId);
			modelBuilder.Entity<GameState>().HasOne(gs => gs.Player).WithOne().HasForeignKey<GameState>(gs => gs.PlayerId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<GameState>().HasOne(gs => gs.Enemy).WithOne().HasForeignKey<GameState>(gs => gs.EnemyId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
			//Players table
			modelBuilder.Entity<Player>().HasKey(p => p.PlayerId);
			modelBuilder.Entity<Player>().HasMany(p => p.Items).WithOne().HasForeignKey(i => i.PlayerId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
			//Enemies table
			modelBuilder.Entity<Enemy>().HasKey(e => e.EnemyId);
			//Items table
			modelBuilder.Entity<Item>().HasKey(i => i.ItemId);
		}
	}
}