using EFModernDA.DataAccess;
using EFModernDA.Queries;
using EFModernDA.Seed;
using Microsoft.EntityFrameworkCore;
using System;

Console.WriteLine("Hello, DEVintersection!");

// set up
var sessions = Seeder.GetSessions();
Console.WriteLine($"Loaded {sessions.Length} sessions.");

// ======================================================================
// database
// note: for production apps, consider using migrations
// https://docs.microsoft.com/ef/core/managing-schemas/migrations/
// never use EnsureDeleted/Created for production, this is for demo only!

var options = new DbContextOptionsBuilder<ConferenceContext>()
    .UseSqlite("Data Source=conference.sqlite")
    .Options;

using var context = new ConferenceContext(options);
context.Database.EnsureDeleted();
context.Database.EnsureCreated();

Console.WriteLine("Created the database. Now seeding the data...");

context.AddRange(sessions);
await context.SaveChangesAsync();

Console.WriteLine("Database loaded.");

// ======================================================================
// simple queries
await SimpleQueries.RunAsync(options);
