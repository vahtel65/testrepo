using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public interface IUserField
    {
        Guid ID { get; set; }
        DictionaryProperty DictionaryProperty { get; set; }
        DictionaryTree DictionaryTree { get; set; }
        Guid DictionaryTreeID { get; set; }
        int Sequence { get; set; }
        TypeEnum FieldType { get; set; }
    }
}
