using EFModernDA.Seed;
using System;

Console.WriteLine("Hello, DEVintersection!");

// set up
var sessions = Seeder.GetSessions();
Console.WriteLine($"Loaded {sessions.Length} sessions.");