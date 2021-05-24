// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;

namespace EFModernDA.Domain
{
    /// <summary>
    /// A session being presented.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Gets or sets the unique identifier of the session.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the session.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the session abstract.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the time the session starts.
        /// </summary>
        public DateTimeOffset SessionStart { get; set; }

        /// <summary>
        /// Gets or sets the time the session ends.
        /// </summary>
        public DateTimeOffset SessionEnd { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Speaker"/> list for the session.
        /// </summary>
        public IList<Speaker> Speakers { get; set; }

        /// <summary>
        /// Gets or sets the session <see cref="Attendee"/> list.
        /// </summary>
        public IList<Attendee> Attendees { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Tag"/> list.
        /// </summary>
        public IList<Tag> Tags { get; set; }

        /// <summary>
        /// The string representation.
        /// </summary>
        /// <returns>The identifier, name, and time slot.</returns>
        public override string ToString() =>
            $"Session {Id}: {Name} [{SessionStart} to {SessionEnd}]";
    }
}
