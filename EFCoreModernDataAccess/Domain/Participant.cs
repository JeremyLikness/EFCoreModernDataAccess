// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace EFModernDA.Domain
{
    /// <summary>
    /// Base for speakers and attendees.
    /// </summary>
    public abstract class Participant
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string Name => $"{LastName}.{FirstName}";

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString() => Name;
    }
}
