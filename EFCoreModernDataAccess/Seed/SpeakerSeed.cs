// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace EFModernDA.Seed
{
    /// <summary>
    /// Helper class to deserialize speaker data.
    /// </summary>
    public class SpeakerSeed
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the speaker biography.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Gets or sets the ids of the sessions the speaker is presenting.
        /// </summary>
        public int[] SessionIds { get; set; }
    }
}
