// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EFModernDA.Domain;

namespace EFModernDA.Seed
{
    /// <summary>
    /// Comparer for instances of <see cref="Speaker"/>.
    /// </summary>
    public class SpeakerComparer : IEqualityComparer<Speaker>
    {
        /// <summary>
        /// Implementation of equality for <see cref="Speaker"/>.
        /// </summary>
        /// <param name="x">Source <see cref="Speaker"/>.</param>
        /// <param name="y">Target <see cref="Speaker"/>.</param>
        /// <returns>A value indicating whether the speakers have the same name.</returns>
        public bool Equals(Speaker x, Speaker y) => x.Name == y.Name;

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <param name="obj">The <see cref="Speaker"/> to hash.</param>
        /// <returns>The hash code based on the name of the speaker.</returns>
        public int GetHashCode([DisallowNull] Speaker obj) => obj.Name.GetHashCode();
    }
}
