// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;

namespace EFModernDA.Domain
{
    /// <summary>
    /// A speaker or presenter.
    /// </summary>
    public class Speaker : Participant
    {
        /// <summary>
        /// Gets or sets the speaker's biography.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Session"/> list the speaker is presenting for.
        /// </summary>
        public IList<Session> Presentations { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Tag"/> list for the speaker.
        /// </summary>
        public IList<Tag> Tags { get; set; }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The speaker's name and count of sessions they are presenting at.</returns>
        public override string ToString() =>
            $"Speaker {Name} ({Presentations?.Count} sessions)";
    }
}
