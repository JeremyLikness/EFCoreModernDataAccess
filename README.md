# EF Core for Modern Data Access

This application was built in stages to demonstrate how EF Core works. It was built specifically for the "EF Core for Modern Data Access"
presentation at DEVintersection Orlando Spring 2021.

## Get started

To get the final application, simply clone this repository:

`git clone https://github.com/JeremyLikness/EFCoreModernDataAccess.git`

Set your current working directory to the root of the `EFCoreModernDataAccess` project.

`cd EFCoreModernDataAccess/EFCoreModernDataAccess`

Run the project. It will automatically create the SQLite database, seed the data, and run the examples.

`dotnet run`

The database is written as `conference.sqlite`.

## Walk through

The project features tagged commits to show the stages of development.

### Start

`git checkout step00`

This is a bare-bones console application. For a friendly message, build and run it:

`dotnet run`

### Domain

`git checkout step01`

Explore the new `Domain` subfolder and see how the domain is defined. Speakers and attendees are modeled as specialized forms of participants. Speakers and sessions can both have tags (categories).

### Seed data

`git checkout step02`

Run the seed logic:

`dotnet run`

Put a breakpoint after the sessions are created.

### Set up EF Core

`git checkout step03`

Create the database:

`dotnet run`

Use the [SQLite](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) extension in [Visual Studio Code](https://code.visualstudio.com/) to view the generated database: `conference.sqlite`.

Note the pivot or many-to-many tables are implicitly created as "shadow entities." The speakers and attendees have their own tables (table-per-type or TPT). To see what it looks like as table-per-hierarchy (TPH) simply add this to the model configuration:

```csharp
modelBuilder.Entity<Participant>();
```

Re-run and examine the new table structure.

### Simple queries

`git checkout step04`

Run the queries:

`dotnet run`

The questions include:

- What tags are there?
- How many sessions and speakers use each tag?
- What speakers use the "Azure" tag?
- What sessions are being presented by speakers using the “Azure” tag?
- What sessions tagged with "Azure" are being presented by speakers using the “Azure” tag?

The answers:

| Query Name | LINQ | Description |
| --- | --- | --- |
|`GetTagsAsync`|`context.Tags.OrderBy(t => t.Name);`|A simple query showing explicit loading (related entities are not loaded by default).|
|`GetTagsWithRelatedEntitiesAsync`|`var query = context.Tags`<br>`.Include(t => t.Speakers)`<br>`.Include(t => t.Sessions)`<br>`.OrderBy(t => t.Name);`|Explicit loading using the `Includes` extension.|
|`GetTagsWithProjectionAsync`|`context.Tags`<br>`.Include(t => t.Speakers)`<br>`.Include(t => t.Sessions)`<br>`.Select(t => new`<br>`{`<br>`   t.Name,`<br>`   Speakers = t.Speakers.Count,`<br>`   Sessions = t.Sessions.Count`<br>`})`<br>`.OrderBy(t => t.Name);`|Same query using projection to limit to just the properties needed to satisfy the query.|
|`GetSpeakersWithTagAsync`|`context.Speakers`<br>`.Where(s => s.Tags.Any(t => t.Name == "Azure"))`<br>`.Select(s => new`<br>`{`<br>`   s.LastName,`<br>`   s.FirstName`<br>`})`<br>`.OrderBy(s => s.LastName)`<br>`.ThenBy(s => s.FirstName);`|Simple filter on a related entity.|
|`GetSessionsForSpeakersWithTagAsync`|`context.Speakers`<br>`.Include(s => s.Presentations)`<br>`.Where(s => s.Tags.Any(t => t.Name == "Azure"))`<br>`.Select(s => new`<br>`{`<br>`   s.LastName,`<br>`   s.FirstName,`<br>`   Presentations = s.Presentations.Select(p => p.Name)`<br>`})`<br>`.OrderBy(s => s.LastName)`<br>`.ThenBy(s => s.FirstName);`|Filter with explicit loading of related entity.|
|`GetSessionsWithTagForSpeakersWithTagAsync`|`context.Speakers`<br>`.Include(s => s.Presentations)`<br>`.ThenInclude(p => p.Tags)`<br>`.Where(s => s.Tags.Any(t => t.Name == "Azure"))`<br>`.Select(s => new`<br>`{`<br>`   s.LastName,`<br>`   s.FirstName,`<br>`   Presentations = s.Presentations`<br>`      .Where(p => p.Tags.Any(t => t.Name == "Azure"))`<br>`      .Select(p => p.Name)`<br>`})`<br>`.OrderBy(s => s.LastName)`<br>`.ThenBy(s => s.FirstName);`|Example of filtered entity with filtered related entities and explicit loading.|

...and what EF Core generates:

| Query Name | Generated SQL | Description |
| --- | --- | --- |
|`GetTagsAsync`|`SELECT "t"."Id", "t"."Name"`<br>`FROM "Tags" AS "t"`<br>`ORDER BY "t"."Name"`|A simple query showing explicit loading (related entities are not loaded by default).|
|`GetTagsWithRelatedEntitiesAsync`|`SELECT "t"."Id", "t"."Name", "t0"."SpeakersId", "t0"."TagsId", "t0"."Id", "t0"."Bio",`<br>`"t0"."FirstName", "t0"."LastName", "t1"."SessionsId", "t1"."TagsId",`<br>`"t1"."Id", "t1"."Description", "t1"."Name", "t1"."SessionEnd", "t1"."SessionStart"`<br>`FROM "Tags" AS "t"`<br>`LEFT JOIN (`<br>`   SELECT "s"."SpeakersId", "s"."TagsId", "s0"."Id", "s0"."Bio", "s0"."FirstName", "s0"."LastName"`<br>`   FROM "SpeakerTag" AS "s"`<br>`   INNER JOIN "Speakers" AS "s0" ON "s"."SpeakersId" = "s0"."Id"`<br>`) AS "t0" ON "t"."Id" = "t0"."TagsId"`<br>`LEFT JOIN (`<br>`   SELECT "s1"."SessionsId", "s1"."TagsId", "s2"."Id", "s2"."Description",`<br>`   "s2"."Name", "s2"."SessionEnd", "s2"."SessionStart"`<br>`   FROM "SessionTag" AS "s1"`<br>`   INNER JOIN "Sessions" AS "s2" ON "s1"."SessionsId" = "s2"."Id"`<br>`) AS "t1" ON "t"."Id" = "t1"."TagsId"`<br>`ORDER BY "t"."Name", "t"."Id", "t0"."SpeakersId",`<br>`"t0"."TagsId", "t0"."Id", "t1"."SessionsId", "t1"."TagsId", "t1"."Id"`|Explicit loading using the `Includes` extension.|
|`GetTagsWithProjectionAsync`|`SELECT "t"."Name", (`<br>`SELECT COUNT(*)`<br>`FROM "SpeakerTag" AS "s"`<br>`INNER JOIN "Speakers" AS "s0" ON "s"."SpeakersId" = "s0"."Id"`<br>`WHERE "t"."Id" = "s"."TagsId") AS "Speakers", (`<br>`SELECT COUNT(*)`<br>`FROM "SessionTag" AS "s1"`<br>`INNER JOIN "Sessions" AS "s2" ON "s1"."SessionsId" = "s2"."Id"`<br>`WHERE "t"."Id" = "s1"."TagsId") AS "Sessions"`<br>`FROM "Tags" AS "t"`<br>`ORDER BY "t"."Name"`|Same query using projection to limit to just the properties needed to satisfy the query.|
|`GetSpeakersWithTagAsync`|`SELECT "s"."LastName", "s"."FirstName"`<br>`FROM "Speakers" AS "s"`<br>`WHERE EXISTS (`<br>`SELECT 1`<br>`FROM "SpeakerTag" AS "s0"`<br>`INNER JOIN "Tags" AS "t" ON "s0"."TagsId" = "t"."Id"`<br>`WHERE ("s"."Id" = "s0"."SpeakersId") AND ("t"."Name" = 'Azure'))`<br>`ORDER BY "s"."LastName", "s"."FirstName"`|Simple filter on a related entity.|
|`GetSessionsForSpeakersWithTagAsync`|`SELECT "s"."LastName", "s"."FirstName", "s"."Id", "t"."Name", "t"."PresentationsId", "t"."SpeakersId", "t"."Id"`<br>`FROM "Speakers" AS "s"`<br>`LEFT JOIN (`<br>`SELECT "s1"."Name", "s0"."PresentationsId", "s0"."SpeakersId", "s1"."Id"`<br>`FROM "SessionSpeaker" AS "s0"`<br>`INNER JOIN "Sessions" AS "s1" ON "s0"."PresentationsId" = "s1"."Id"`<br>`) AS "t" ON "s"."Id" = "t"."SpeakersId"`<br>`WHERE EXISTS (`<br>`SELECT 1`<br>`FROM "SpeakerTag" AS "s2"`<br>`INNER JOIN "Tags" AS "t0" ON "s2"."TagsId" = "t0"."Id"`<br>`WHERE ("s"."Id" = "s2"."SpeakersId") AND ("t0"."Name" = 'Azure'))`<br>`ORDER BY "s"."LastName", "s"."FirstName", "s"."Id", "t"."PresentationsId", "t"."SpeakersId", "t"."Id"`|Filter with explicit loading of related entity.|
|`GetSessionsWithTagForSpeakersWithTagAsync`|`SELECT "s"."LastName", "s"."FirstName", "s"."Id", "t0"."Name", "t0"."PresentationsId", "t0"."SpeakersId", "t0"."Id"`<br>`FROM "Speakers" AS "s"`<br>`LEFT JOIN (`<br>`SELECT "s1"."Name", "s0"."PresentationsId", "s0"."SpeakersId", "s1"."Id"`<br>`FROM "SessionSpeaker" AS "s0"`<br>`INNER JOIN "Sessions" AS "s1" ON "s0"."PresentationsId" = "s1"."Id"`<br>`WHERE EXISTS (`<br>`SELECT 1`<br>`FROM "SessionTag" AS "s2"`<br>`INNER JOIN "Tags" AS "t" ON "s2"."TagsId" = "t"."Id"`<br>`WHERE ("s1"."Id" = "s2"."SessionsId") AND ("t"."Name" = 'Azure'))`<br>`) AS "t0" ON "s"."Id" = "t0"."SpeakersId"`<br>`WHERE EXISTS (`<br>`SELECT 1`<br>`FROM "SpeakerTag" AS "s3"`<br>`INNER JOIN "Tags" AS "t1" ON "s3"."TagsId" = "t1"."Id"`<br>`WHERE ("s"."Id" = "s3"."SpeakersId") AND ("t1"."Name" = 'Azure'))`<br>`ORDER BY "s"."LastName", "s"."FirstName", "s"."Id", "t0"."PresentationsId", "t0"."SpeakersId", "t0"."Id"`|Example of filtered entity with filtered related entities and explicit loading.|

### Advanced queries

`git checkout step05`

Run the queries:

`dotnet run`

The questions include:

- How many possible combinations of speakers and attendees are there?
- Which attendees share the most sessions together?
- Which attendees share the least sessions together?

Look at `AdvancedQueries.cs` for details.

### Mutations (updates)

`git checkout step06`

Run the updates:

`dotnet run`

Examples include:

- Using the simple logging `LogTo` directive in options configuration
- Using the `ChangeTracker` to show before/after snapshots
- Simple load, manipulate, update
- Graph update (modify properties on related entities and save)
- Disconnected entity (common scenario in Web API implementations) update
- Concurrency detection

## Conclusion

This app is designed to show how to map a domain to the backend database using EF Core. Please provide any feedback, comments, suggestions, or questions you may have.

Regards,

[![Jeremy Likness](https://blog.jeremylikness.com/images/jeremylikness.gif)](https://blog.jeremylikness.com)