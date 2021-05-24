// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;

namespace EFModernDA.Domain
{
    /// <summary>
    /// The event attendee.
    /// </summary>
    public class Attendee : Participant
    {
        /// <summary>
        /// Gets or sets the date the guest registered.
        /// </summary>
        public DateTimeOffset DateRegistered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user consented to having their picture taken.
        /// </summary>
        public bool ConsentedToPhotographs { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Session"/> list for the attendee.
        /// </summary>
        public IList<Session> Sessions { get; set; }

        /// <summary>
        /// Provides the string representation.
        /// </summary>
        /// <returns>Name and consent.</returns>
        public override string ToString()
        {
            var consent = ConsentedToPhotographs ? "Yes" : "No";
            return $"Attendee {Name} Consented: {consent}";
        }
    }
}
