// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EFModernDA.Domain;

namespace EFModernDA.Seed
{
    /// <summary>
    /// Comparison logic for tags.
    /// </summary>
    public class TagComparer : IEqualityComparer<Tag>
    {
        /// <summary>
        /// Tag quality.
        /// </summary>
        /// <param name="x">The source <see cref="Tag"/>.</param>
        /// <param name="y">The target <see cref="Tag"/>.</param>
        /// <returns>A value indicating whether the names match.</returns>
        public bool Equals(Tag x, Tag y) => x?.Name == y?.Name;

        /// <summary>
        /// Gets the <see cref="Tag"/> hash code.
        /// </summary>
        /// <param name="obj">The target <see cref="Tag"/>.</param>
        /// <returns>The hash code of the name.</returns>
        public int GetHashCode([DisallowNull] Tag obj) => obj.Name.GetHashCode();
    }
}
