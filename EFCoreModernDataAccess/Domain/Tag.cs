// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;

namespace EFModernDA.Domain
{
    /// <summary>
    /// A tag or category.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Speaker"/> list who use the tag.
        /// </summary>
        public IList<Speaker> Speakers { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Session"/> list that use the tag.
        /// </summary>
        public IList<Session> Sessions { get; set; }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The tag name with counts for speakers and sessions using the tag.</returns>
        public override string ToString() =>
            $"Tag '{Name}' tagged by {Speakers?.Count} speakers and {Sessions?.Count} sessions";
    }
}
