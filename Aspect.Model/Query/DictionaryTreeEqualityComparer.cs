using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Model.Query
{
    public class DictionaryTreeEqualityComparer : IEqualityComparer<Aspect.Domain.TreeViewResult>
    {
        private DictionaryTreeEqualityComparer()
        {
        }

        public static DictionaryTreeEqualityComparer Instance
        {
            get
            {
                return new DictionaryTreeEqualityComparer();
            }
        }
        #region IEqualityComparer<GetDictionaryTreeParentsResult> Members

        public bool Equals(Aspect.Domain.TreeViewResult x, Aspect.Domain.TreeViewResult y)
        {
            if (x.ID.Equals(y.ID)) return true;
            return false;
        }

        public int GetHashCode(Aspect.Domain.TreeViewResult obj)
        {
            string s = obj.ID.ToString();
            return s.GetHashCode();
        }

        #endregion
    }

    public class DictionaryTreeResultEqualityComparer : IEqualityComparer<Aspect.Domain.GetDictionaryTreeParentsResult>
    {
        private DictionaryTreeResultEqualityComparer()
        {
        }

        public static DictionaryTreeResultEqualityComparer Instance
        {
            get
            {
                return new DictionaryTreeResultEqualityComparer();
            }
        }
        #region IEqualityComparer<GetDictionaryTreeParentsResult> Members

        public bool Equals(Aspect.Domain.GetDictionaryTreeParentsResult x, Aspect.Domain.GetDictionaryTreeParentsResult y)
        {
            if (x.ID.Equals(y.ID)) return true;
            return false;
        }

        public int GetHashCode(Aspect.Domain.GetDictionaryTreeParentsResult obj)
        {
            string s = obj.ID.ToString();
            return s.GetHashCode();
        }

        #endregion
    }
}
