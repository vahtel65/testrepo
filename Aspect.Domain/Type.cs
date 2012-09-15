using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public enum TypeEnum
    {
        Default,
        Boolean,
        Integer,
        Decimal,
        Datetime,
        IsBeing,
        InList
    }
    public partial class Type
    {
        public static Guid BooleanType = new Guid("BE264A7D-8B3E-46E0-95DF-C8A51A1E2FFF");
        public static Guid IntegerType =  new Guid("F1DDB372-44E2-4C9A-ADB2-A7C326AFA3DC");
        public static Guid DecimalType = new Guid("A669D88E-ED67-42FE-8A18-A54DB3614134");
        public static Guid DateTimeType = new Guid("E48D93BE-C55D-4A62-9D3A-DD48F2BE9CE8");

        partial void OnLoaded()
        {
            if (this.ID == BooleanType)
            {
                Value = TypeEnum.Boolean;
            }
            else if (this.ID == IntegerType)
            {
                Value = TypeEnum.Integer;
            }
            else if (this.ID == DateTimeType)
            {
                Value = TypeEnum.Datetime;
            }
            else if (this.ID == DecimalType)
            {
                Value = TypeEnum.Decimal;
            }
            else
            {
                Value = TypeEnum.Default;
            }
        }
        public TypeEnum Value { get; set; }
    }
}
