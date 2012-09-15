using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class DictionaryUserField : IUserField
    {
        partial void OnLoaded()
        {
            FieldType = this.DictionaryProperty.Type.Value;
        }

        public TypeEnum FieldType { get; set; }
    }
}
