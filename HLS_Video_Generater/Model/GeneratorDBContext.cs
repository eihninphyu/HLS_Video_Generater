using Microsoft.EntityFrameworkCore;

namespace HLS_Video_Generater.Model
{
    public class GeneratorDBContext :DbContext
    {
        public GeneratorDBContext(DbContextOptions<GeneratorDBContext> options) : base(options) { }
        public DbSet<M3U8Info> M3U8Info { get; set; }
        public DbSet<MediaSegments> MediaSegments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<M3U8Info>().HasMany(x => x.MediaSegments).WithOne(c=>c.M3U8Info);
            base.OnModelCreating(builder);
        }
    }
}
