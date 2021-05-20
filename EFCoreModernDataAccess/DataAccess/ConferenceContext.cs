using EFModernDA.Domain;
using Microsoft.EntityFrameworkCore;

namespace EFModernDA.DataAccess
{
    public class ConferenceContext : DbContext
    {
        public ConferenceContext(DbContextOptions<ConferenceContext> options) :
            base(options)
        {
        }

        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Speakers)
                .WithMany(sp => sp.Presentations);

            modelBuilder.Entity<Session>()
                .HasMany(s => s.Attendees)
                .WithMany(a => a.Sessions);

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Sessions)
                .WithMany(s => s.Tags);

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Speakers)
                .WithMany(sp => sp.Tags);

            base.OnModelCreating(modelBuilder);
        }
    }
}
