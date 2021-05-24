// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using EFModernDA.Domain;
using Microsoft.EntityFrameworkCore;

namespace EFModernDA.DataAccess
{
    /// <summary>
    /// The EF Core <see cref="DbContext"/> implementation of the conference database.
    /// </summary>
    public class ConferenceContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceContext"/> class.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public ConferenceContext(DbContextOptions<ConferenceContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the attendees.
        /// </summary>
        public DbSet<Attendee> Attendees { get; set; }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        public DbSet<Session> Sessions { get; set; }

        /// <summary>
        /// Gets or sets the speakers.
        /// </summary>
        public DbSet<Speaker> Speakers { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Called to configure the EF Core model: <seealso href="https://docs.microsoft.com/ef/core/modeling/"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // right now configuration is TPT. To change to TPH, uncomment:
            // modelBuilder.Entity<Participant>();
            // for more on TPT/TPH: https://docs.microsoft.com/ef/core/modeling/inheritance
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

            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .IsConcurrencyToken();

            base.OnModelCreating(modelBuilder);
        }
    }
}
