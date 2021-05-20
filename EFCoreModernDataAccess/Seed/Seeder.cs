using EFModernDA.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EFModernDA.Seed
{
    public static class Seeder
    {
        const int attendeeCount = 1000;

        private static readonly Random Random = new();

        public static Session[] GetSessions()
        {
            var sessions = new Dictionary<int, Session>();
            ParseSessions(sessions);
            ParseSpeakers(sessions);
            ParseTags(sessions);
            AddAttendees(sessions);
            return sessions.Values.ToArray();
        }

        private static void AddAttendees(Dictionary<int, Session> sessions)
        {
            Console.WriteLine("Creating attendees...");
            Console.WriteLine("Reading last names...");
            var lastnames = GetEmbeddedText("surnames", false)
                .Split(new[] { '\r', '\n' })
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .ToArray();
            var firstNames = sessions.Values.SelectMany(s => s.Speakers)
                .Select(speaker => speaker.FirstName)
                .Distinct()
                .ToArray();

            Console.WriteLine($"Generating {attendeeCount} attendee names...");

            var lastnameEnum = lastnames.AsInfiniteRandomizedEnumerable().GetEnumerator();
            var firstnameEnum = firstNames.AsInfiniteRandomizedEnumerable().GetEnumerator();
            var attendees = new List<Attendee>();
            while (attendees.Count < attendeeCount)
            {
                lastnameEnum.MoveNext();
                firstnameEnum.MoveNext();
                attendees.Add(new Attendee
                {
                    FirstName = firstnameEnum.Current,
                    LastName = lastnameEnum.Current,
                    DateRegistered = DateTime.Now.AddDays(-1 * Random.Next(2, 60)),
                    ConsentedToPhotographs = Random.NextDouble() > 0.1,
                    Sessions = new List<Session>()
                });
            }
            Console.WriteLine("Building attendee schedules...");
            foreach (var attendee in attendees)
            {
                var scheduledSessions = sessions.Values.AsScheduledSessionList();
                foreach (var scheduledSession in scheduledSessions)
                {
                    attendee.Sessions.Add(scheduledSession);
                    scheduledSession.Attendees.Add(attendee);
                }
            }

            Console.WriteLine("Everyone is set!");
        }

        private static void ParseTags(Dictionary<int, Session> sessions)
        {
            Console.WriteLine("Reading tags...");
            var tagList = new List<(Tag, string[])>();
            var tagData = GetEmbeddedText("tags", false);
            foreach (var tagLine in tagData.Split(new[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(tagLine) == false)
                {
                    var tagParts = tagLine.Split(",");
                    var tag = new Tag
                    {
                        Name = tagParts[0].Trim(),
                        Sessions = new List<Session>(),
                        Speakers = new List<Speaker>()
                    };
                    tagList.Add((tag,
                        tagParts.Select(t => t.ToLowerInvariant().Trim()).ToArray()));
                }
            }
            Console.WriteLine($"Loaded {tagList.Count} tags. Processing...");

            var tagComparer = new TagComparer();
            var speakerComparer = new SpeakerComparer();

            var sessionsToProcess = sessions.Values.OfType<object>();
            var speakersToProcess = sessions.Values.SelectMany(s => s.Speakers)
                .Distinct(speakerComparer).OfType<object>();

            foreach (var toProcess in sessionsToProcess.Union(speakersToProcess))
            {
                foreach (var tag in tagList)
                {
                    foreach (var tagVariation in tag.Item2)
                    {
                        if (toProcess is Session session)
                        {
                            ProcessTag(
                                session,
                                session.Name,
                                tagVariation,
                                tag.Item1,
                                session.Tags,
                                tagComparer);

                            ProcessTag(
                                session,
                                session.Description,
                                tagVariation,
                                tag.Item1,
                                session.Tags,
                                tagComparer);
                        }

                        if (toProcess is Speaker speaker)
                        {
                            ProcessTag(
                                speaker,
                                speaker.Bio,
                                tagVariation,
                                tag.Item1,
                                speaker.Tags,
                                tagComparer);
                        }
                    }
                }
            }

            var assignments = tagList.Select(t => t.Item1)
                    .Select(t => t.Sessions.Count + t.Speakers.Count)
                    .Sum();

            Console.WriteLine($"Processed {assignments} tag assignments.");
        }

        private static void ProcessTag(
            object target,
            string valueToCheck,
            string tagValue,
            Tag tag,
            IList<Tag> list,
            IEqualityComparer<Tag> tagComparer)
        {
            if (valueToCheck.ToLowerInvariant().Contains(tagValue))
            {
                if (list.Contains(tag, tagComparer) == false)
                {
                    list.Add(tag);
                    if (target is Session session)
                    {
                        tag.Sessions.Add(session);
                    }
                    else
                    {
                        tag.Speakers.Add(target as Speaker);
                    }
                }
            }
        }

        private static void ParseSpeakers(Dictionary<int, Session> sessions)
        {
            Console.Write("Parsing speakers...");
            var speakerList = JsonSerializer.Deserialize<SpeakerSeed[]>
                (GetEmbeddedText("speakers"));
            foreach (var speakerSeed in speakerList)
            {
                var speaker = new Speaker
                {
                    Bio = speakerSeed.Bio,
                    FirstName = speakerSeed.FirstName,
                    LastName = speakerSeed.LastName,
                    Presentations = new List<Session>(),
                    Tags = new List<Tag>()
                };

                foreach (var id in speakerSeed.SessionIds)
                {
                    if (sessions.ContainsKey(id))
                    {
                        speaker.Presentations.Add(sessions[id]);
                        sessions[id].Speakers.Add(speaker);
                    }
                }
            }

            Console.WriteLine($"Processed {speakerList.Length} speakers.");
        }

        private static void ParseSessions(Dictionary<int, Session> sessions)
        {
            Console.Write("Parsing sessions...");
            var sessionList = JsonSerializer.Deserialize<Session[]>
                (GetEmbeddedText(nameof(sessions)));
            foreach (var session in sessionList)
            {
                var id = session.Id;
                session.Id = 0;
                session.Speakers = new List<Speaker>();
                session.Tags = new List<Tag>();
                session.Attendees = new List<Attendee>();
                sessions.Add(id, session);
            }
            Console.WriteLine($"Parsed {sessions.Count} sessions.");
        }

        private static string GetEmbeddedText(string resource, bool isJson = true)
        {
            var extension = isJson ? "json" : "txt";
            var assembly = typeof(Seeder).Assembly;
            var sessionsResource = $"{typeof(Seeder).Namespace}.{resource}.{extension}";
            using var sessionStream = assembly.GetManifestResourceStream(sessionsResource);
            using var textStream = new StreamReader(sessionStream);
            return textStream.ReadToEnd();
        }

        public static T[] Randomized<T>(this T[] list)
        {
            var input = new List<T>(list);
            var randomizedList = new List<T>();
            while (input.Count > 0)
            {
                var idx = Random.Next(0, input.Count);
                randomizedList.Add(input[idx]);
                input.RemoveAt(idx);
            }

            return randomizedList.ToArray();
        }

        public static IEnumerable<T> AsInfiniteRandomizedEnumerable<T>(this T[] list)
        {
            var shuffled = list.Randomized();
            do
            {
                for (var idx = 0; idx < shuffled.Length; idx++)
                {
                    yield return shuffled[idx];
                }

                shuffled = shuffled.Randomized();
            }
            while (shuffled.Length > 0);
        }

        public static Session[] AsScheduledSessionList(this IEnumerable<Session> src)
        {
            var result = new List<Session>();
            var done = false;
            var start = src.Select(s => s.SessionStart).OrderBy(s => s).First();
            do
            {
                var session = src.Where(s => s.SessionStart == start)
                    .ToArray().Randomized().First();
                result.Add(session);
                if (src.Any(s => s.SessionStart > start))
                {
                    start = src.Where(s => s.SessionStart > start)
                        .Select(s => s.SessionStart)
                        .OrderBy(s => s)
                        .First();
                }
                else
                {
                    done = true;
                }
            }
            while (!done);

            return result.ToArray();
        }       
    }
}
