using EFModernDA.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EFModernDA.Queries
{
    public static class SimpleQueries
    {
        public static async Task RunAsync(DbContextOptions<ConferenceContext> options)
        {
            using var context = new ConferenceContext(options);

            await GetTagsAsync(context);
            await GetTagsWithRelatedEntitiesAsync(context);
            await GetTagsWithProjectionAsync(context);
            await GetSpeakersWithTagAsync(context);
            await GetSessionsForSpeakersWithTagAsync(context);
            await GetSessionsWithTagForSpeakersWithTagAsync(context);
        }

        private static async Task GetTagsAsync(ConferenceContext context)
        {
            var query = context.Tags.OrderBy(t => t.Name);
            Console.WriteLine(query.ToQueryString());
            foreach (var tag in await query.ToListAsync())
            {
                Console.WriteLine(tag);
            }
            Console.ReadLine();
        }

        private static async Task GetTagsWithRelatedEntitiesAsync(ConferenceContext context)
        {
            var query = context.Tags
                .Include(t => t.Speakers)
                .Include(t => t.Sessions)
                .OrderBy(t => t.Name);

            Console.WriteLine(query.ToQueryString());
            foreach (var tag in await query.ToListAsync())
            {
                Console.WriteLine(tag);
            }
            Console.ReadLine();
        }

        private static async Task GetTagsWithProjectionAsync(ConferenceContext context)
        {
            var query = context.Tags
                .Include(t => t.Speakers)
                .Include(t => t.Sessions)
                .Select(t => new
                {
                    t.Name,
                    Speakers = t.Speakers.Count,
                    Sessions = t.Sessions.Count
                })
                .OrderBy(t => t.Name);

            Console.WriteLine(query.ToQueryString());
            foreach (var tag in await query.ToListAsync())
            {
                Console.WriteLine($"{tag.Name} with {tag.Speakers} speakers and {tag.Sessions} sessions");
            }
            Console.ReadLine();
        }

        private static async Task GetSpeakersWithTagAsync(ConferenceContext context)
        {
            var query = context.Speakers
                .Where(s => s.Tags.Any(t => t.Name == "Azure"))
                .Select(s => new
                {
                    s.LastName,
                    s.FirstName
                })
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName);
            Console.WriteLine(query.ToQueryString());
            var speakers = 0;
            foreach (var speaker in await query.ToListAsync())
            {
                speakers++;
                Console.WriteLine($"{speaker.LastName}, {speaker.FirstName}");
            }
            Console.WriteLine($"{speakers} speakers.");
            Console.ReadLine();
        }

        private static async Task GetSessionsForSpeakersWithTagAsync(ConferenceContext context)
        {
            var query = context.Speakers
                .Include(s => s.Presentations)
                .Where(s => s.Tags.Any(t => t.Name == "Azure"))
                .Select(s => new
                {
                    s.LastName,
                    s.FirstName,
                    Presentations = s.Presentations.Select(p => p.Name)
                })
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName);
            Console.WriteLine(query.ToQueryString());
            var speakers = 0;
            var sessions = 0;
            foreach (var speaker in await query.ToListAsync())
            {
                speakers++;
                Console.WriteLine($"{speaker.LastName}, {speaker.FirstName}");
                foreach (var presentation in speaker.Presentations.OrderBy(p => p))
                {
                    sessions++;
                    Console.WriteLine($"\t{presentation}");
                }
            }
            Console.WriteLine($"{speakers} speakers and {sessions} sessions.");
            Console.ReadLine();
        }

        private static async Task GetSessionsWithTagForSpeakersWithTagAsync(ConferenceContext context)
        {
            var query = context.Speakers
                .Include(s => s.Presentations)
                .ThenInclude(p => p.Tags)
                .Where(s => s.Tags.Any(t => t.Name == "Azure"))
                .Select(s => new
                {
                    s.LastName,
                    s.FirstName,
                    Presentations = s.Presentations
                        .Where(p => p.Tags.Any(t => t.Name == "Azure"))
                        .Select(p => p.Name)
                })
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName);
            Console.WriteLine(query.ToQueryString());
            var speakers = 0;
            var sessions = 0;
            foreach (var speaker in await query.ToListAsync())
            {
                speakers++;
                Console.WriteLine($"{speaker.LastName}, {speaker.FirstName}");
                foreach (var presentation in speaker.Presentations.OrderBy(p => p))
                {
                    sessions++;
                    Console.WriteLine($"\t{presentation}");
                }
            }
            Console.WriteLine($"{speakers} speakers and {sessions} sessions.");
            Console.ReadLine();
        }
    }
}
