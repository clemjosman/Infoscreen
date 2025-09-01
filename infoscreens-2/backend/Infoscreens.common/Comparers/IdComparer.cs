using Infoscreens.Common.Interfaces;
using System.Collections.Generic;

namespace Infoscreens.Common.Comparers
{
    public class IdComparer<T> : EqualityComparer<T> where T : IId
    {
        public IdComparer()
        { }

        public override bool Equals(T item1, T item2)
        {
            return item1.Id == item2.Id;
        }

        public override int GetHashCode(T item)
        {
            return item.Id.GetHashCode();
        }
    }
}
