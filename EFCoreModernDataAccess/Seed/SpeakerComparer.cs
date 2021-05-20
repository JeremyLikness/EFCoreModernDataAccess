using EFModernDA.Domain;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EFModernDA.Seed
{
    public class SpeakerComparer : IEqualityComparer<Speaker>
    {
        public bool Equals(Speaker x, Speaker y) => x.Name == y.Name;

        public int GetHashCode([DisallowNull] Speaker obj) => obj.Name.GetHashCode();
    }
}
