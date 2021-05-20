using EFModernDA.Domain;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EFModernDA.Seed
{
    public class TagComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y) => x?.Name == y?.Name;

        public int GetHashCode([DisallowNull] Tag obj) => obj.Name.GetHashCode();
    }
}
