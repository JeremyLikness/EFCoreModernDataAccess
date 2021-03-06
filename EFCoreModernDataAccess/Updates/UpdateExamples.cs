// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EFModernDA.DataAccess;
using EFModernDA.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EFModernDA.Updates
{
    /// <summary>
    /// Examples of EF Core update capabilities.
    /// </summary>
    public static class UpdateExamples
    {
        /// <summary>
        /// Runs the examples.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RunAsync(DbContextOptions<ConferenceContext> options)
        {
            foreach (var example in new Func<ConferenceContext, Task>[]
            {
                SimpleUpdateAsync,
                GraphUpdateAsync,
                DisconnectedUpdateAsync,
            })
            {
                using var context = GetContext(options);
                await example(context);
                Console.ReadLine();
            }

            await ConcurrencyAsync(options);
        }

        /// <summary>
        /// A simple update.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private static async Task SimpleUpdateAsync(ConferenceContext context)
        {
            Console.WriteLine("Changing attendee consent.");
            var attendee = await context.Attendees.FirstAsync();
            Console.WriteLine(attendee);
            context.Snap();
            attendee.ConsentedToPhotographs = !attendee.ConsentedToPhotographs;
            Console.WriteLine($"Changed to: {attendee}");
            context.Snap();
            await context.SaveChangesAsync();
            Console.WriteLine("Updated.");
            context.Snap();
        }

        /// <summary>
        /// Demonstration of updating entities deeper in the object graph.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private static async Task GraphUpdateAsync(ConferenceContext context)
        {
            Console.WriteLine("Getting speakers with tags and sessions.");

            var speakers = await context.Speakers
                .Include(s => s.Tags)
                .ThenInclude(t => t.Sessions)
                .OrderBy(sp => sp.LastName)
                .Take(5)
                .ToListAsync();

            Console.WriteLine("Modifying the graph...");

            var speaker = speakers.First();
            speaker.Bio = $"Bio: {speaker.Bio}";

            var tag = speakers.SelectMany(sp => sp.Tags).First();
            tag.Name = "Different tag";

            var session = speakers.SelectMany(sp => sp.Tags).SelectMany(t => t.Sessions).First();
            session.SessionStart = session.SessionStart.AddMinutes(1);

            Console.WriteLine("Adding a new tag...");
            context.Tags.Add(new Tag { Name = "Empty Tag" });

            Console.WriteLine("Saving...");
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Example of a disconnected (not tracked) update.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private static async Task DisconnectedUpdateAsync(ConferenceContext context)
        {
            Console.WriteLine("Reading a session...");
            var session = await context.Sessions.FirstAsync();
            Console.WriteLine(session);
            context.ChangeTracker.Clear(); // forget about it
            var disconnectedSession = new Session
            {
                Id = session.Id,
                Name = session.Name,
                Description = session.Description,
                SessionStart = session.SessionStart,
                SessionEnd = session.SessionStart.AddMinutes(5), // short session!
            };
            Console.WriteLine("Attaching and saving the disconnected session...");
            context.Update(disconnectedSession);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Example of concurrency conflict.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private static async Task ConcurrencyAsync(DbContextOptions<ConferenceContext> options)
        {
            Console.WriteLine("Acquiring a tag...");
            using var firstUser = GetContext(options);
            var tag = await firstUser.Tags.FirstAsync();
            using var secondUser = GetContext(options);
            var tagDup = await secondUser.FindAsync<Tag>(tag.Id);

            Console.WriteLine("Modifying tag...");
            tag.Name = $"_{tag.Name}_";
            await firstUser.SaveChangesAsync();

            Console.WriteLine("Modifying stale tag...");
            tagDup.Name = $"*{tagDup.Name}*";

            try
            {
                await secondUser.SaveChangesAsync();
                Console.WriteLine("I guess concurrency is broken.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EF Core detected a concurrency issue:");
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Take a snapshot of the <see cref="ChangeTracker"/>.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        private static void Snap(this ConferenceContext context) =>
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        /// <summary>
        /// Gets a <see cref="ConferenceContext"/> instance.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <returns>The <see cref="ConferenceContext"/>.</returns>
        private static ConferenceContext GetContext(DbContextOptions<ConferenceContext> options)
        {
            var builder = new DbContextOptionsBuilder<ConferenceContext>(options);
            var newOptions = builder.LogTo(
                Console.WriteLine,
                new[] { DbLoggerCategory.Database.Command.Name }).Options;
            return new ConferenceContext(newOptions);
        }
    }
}
