// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EFModernDA.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EFModernDA.Queries
{
    /// <summary>
    /// More advanced queries.
    /// </summary>
    public static class AdvancedQueries
    {
        /// <summary>
        /// Run the examples.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RunAsync(DbContextOptions<ConferenceContext> options)
        {
            using var context = GetContext(options);
            await HowManyPossibleCombinationsOfSpeakersAndAttendeesAsync(context);
            await WhichAttendeesShareTheMostOrLeastSessionsAsync(context);
        }

        /// <summary>
        /// Cross-join example.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        private static async Task HowManyPossibleCombinationsOfSpeakersAndAttendeesAsync(ConferenceContext context)
        {
            var combinations = context.Attendees
                                .Join(
                                    context.Speakers,
                                    a => 1,
                                    s => 1,
                                    (a, s) => new
                                    {
                                        attendeeId = a.Id,
                                        speakerId = s.Id,
                                    });
            var count = await combinations.CountAsync();
            Console.WriteLine($"There are {count} possible speaker/attendee combinations.");
            Console.ReadLine();
        }

        /// <summary>
        /// More complicated query with cross-join and grouping.
        /// </summary>
        /// <param name="context">The <see cref="ConferenceContext"/>.</param>
        /// <returns>A <see cref="Task"/>/.</returns>
        private static async Task WhichAttendeesShareTheMostOrLeastSessionsAsync(ConferenceContext context)
        {
            var pairs = (from a1 in context.Attendees
                         from a2 in context.Attendees
                         where a1.Id != a2.Id
                         select new
                         {
                             a1 = a1.Id,
                             a1LastName = a1.LastName,
                             a1FirstName = a2.FirstName,
                             a2 = a2.Id,
                             a2LastName = a2.LastName,
                             a2FirstName = a2.FirstName,
                             sessionCount = a1.Sessions.Select(s => s.Id)
                             .Intersect(a2.Sessions.Select(s => s.Id)).Count(),
                         }).OrderByDescending(shared => shared.sessionCount)
                     .Take(5);

            Console.WriteLine("Which attendees share the most sessions (approach #1)?");

            foreach (var answer in await pairs.ToListAsync())
            {
                Console.Write($"{answer.a1LastName}, {answer.a1FirstName} and ");
                Console.Write($"{answer.a2LastName}, {answer.a2FirstName} share ");
                Console.WriteLine($"{answer.sessionCount} sessions.");
            }

            Console.ReadLine();

            var answers =
                context.Attendees
                    .SelectMany(
                    a => a.Sessions,
                    (a, session) =>
                        new
                        {
                            a.Id,
                            a.LastName,
                            a.FirstName,
                            sessionId = session.Id,
                        })
                 .Join(
                    context.Attendees
                        .SelectMany(
                        a => a.Sessions,
                        (a, session) =>
                        new
                        {
                            a.Id,
                            a.LastName,
                            a.FirstName,
                            sessionId = session.Id,
                        }),
                    a1 => a1.sessionId,
                    a2 => a2.sessionId,
                    (a1, a2) => new
                    {
                        a1Id = a1.Id,
                        a1LastName = a1.LastName,
                        a1FirstName = a1.FirstName,
                        a2Id = a2.Id,
                        a2LastName = a2.LastName,
                        a2FirstName = a2.FirstName,
                        a1.sessionId,
                    })
                 .Where(result => result.a1Id != result.a2Id)
                 .GroupBy(a => new
                 {
                     a.a1Id,
                     a.a1FirstName,
                     a.a1LastName,
                     a.a2Id,
                     a.a2FirstName,
                     a.a2LastName,
                 })
                 .Select(g => new
                 {
                     g.Key.a1Id,
                     g.Key.a1FirstName,
                     g.Key.a1LastName,
                     g.Key.a2Id,
                     g.Key.a2FirstName,
                     g.Key.a2LastName,
                     count = g.Count(),
                 });

            Console.WriteLine("Which attendees share the most sessions (approach #2)?");

            foreach (var answer in await answers.OrderByDescending(g => g.count).Take(5).ToListAsync())
            {
                Console.Write($"{answer.a1LastName}, {answer.a1FirstName} and ");
                Console.Write($"{answer.a2LastName}, {answer.a2FirstName} share ");
                Console.WriteLine($"{answer.count} sessions.");
            }

            Console.ReadLine();

            Console.WriteLine("Which attendees share the least sessions?");

            foreach (var answer in await answers.OrderBy(g => g.count).Take(5).ToListAsync())
            {
                Console.Write($"{answer.a1LastName}, {answer.a1FirstName} and ");
                Console.Write($"{answer.a2LastName}, {answer.a2FirstName} share ");
                Console.WriteLine($"{answer.count} sessions.");
            }

            Console.ReadLine();
        }

        private static ConferenceContext GetContext(DbContextOptions<ConferenceContext> options)
        {
            var builder = new DbContextOptionsBuilder<ConferenceContext>(options);
            var newOptions = builder.LogTo(Console.WriteLine).Options;
            return new ConferenceContext(newOptions);
        }
    }
}
