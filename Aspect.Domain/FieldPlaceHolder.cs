using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public enum FieldPlaceHolderEnum
    {
        Grid,
        GridCard
    }
    public partial class FieldPlaceHolder
    {
        public static Guid Grid = new Guid("CC39283F-52C4-4B47-98D6-86E9A95D4940");
        public static Guid GridCard = new Guid("E4E03102-B371-4682-88CE-086B01A05FE8");
        partial void OnLoaded()
        {
            if(this.ID == Grid)
            {
                Value = FieldPlaceHolderEnum.Grid;
            }
            else if(this.ID == GridCard)
            {
                Value = FieldPlaceHolderEnum.GridCard;
            }
        }

        public static Guid GetFieldPlaceHolderID(FieldPlaceHolderEnum value)
        {
            switch (value)
            {
                case FieldPlaceHolderEnum.GridCard:
                    return GridCard;
                case FieldPlaceHolderEnum.Grid:
                    return Grid;
            }
            return Guid.Empty;
        }

        public FieldPlaceHolderEnum Value { get; set; }
    }
}
