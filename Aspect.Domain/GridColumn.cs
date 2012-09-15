using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    [Serializable]
    public class EditableGridColumn : GridColumn
    {
        public EditableGridColumn()
        {
            View = GridColumnView.TextBox;
            AllowNULL = false;
            Width = 40;
        }

        public bool AllowNULL { get; set; }

        public GridColumnView View { get; set; }

        public Source DataSource {get;set;}

        public System.Type Type { get; set; }

        public int Size { get; set; }

        public int Width { get; set; }

        public enum GridColumnView
        {
            //Readonly,
            TextBox,
            DropDown,
            CheckBox
        }

        [Serializable]
        public class Source
        {
            /*[NonSerialized]*/
            private object _DataSource;

            public object DataSource 
            { 
                get
                {
                    return _DataSource;
                }
                set
                {
                    _DataSource = value;
                }
            }
            public string ValueField { get; set; }
            public string TextField { get; set; }
        }

        #region editable columns
        public static EditableGridColumn QuantityColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Количество",
                    DataItem = "Quantity",
                    Group = "Состав",
                    Width = 64,
                    View = GridColumnView.TextBox,
                    Type = typeof(decimal),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-a6734e4d874e")
                };
            }
        }

        public static EditableGridColumn PositionColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Поз.",
                    DataItem = "Position",
                    View = GridColumnView.TextBox,
                    Group = "Состав",
                    Type = typeof(string),
                    Width = 30,
                    Size = 10,
                    ID = new Guid("aaacaaac-0000-0000-0000-8e41167ed311")
                };
            }
        }

        public static EditableGridColumn _dictUMColumn
        {
            get
            {
                EditableGridColumn column = new EditableGridColumn()
                {
                    Name = "Ед.изм.",
                    DataItem = "_dictUMID",
                    Group = "Состав",
                    View = GridColumnView.DropDown,
                    Type = typeof(Guid),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-5c6454345f09")
                };
                return column;
            }
        }

        public static EditableGridColumn GroupNumberColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Вар. зам.",
                    DataItem = "GroupNumber",
                    AllowNULL = true,
                    View = GridColumnView.TextBox,
                    Group = string.Empty,
                    Type = typeof(int),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-ff7c96100dad")
                };
            }
        }

        public static EditableGridColumn GroupToChangeColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Гр. зам.",
                    DataItem = "GroupToChange",
                    AllowNULL = true,
                    View = GridColumnView.TextBox,
                    Group = string.Empty,
                    Type = typeof(int),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-e481400af7ce")
                };
            }
        }

        public static EditableGridColumn AutoUpdateColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Обн-ть",
                    DataItem = "AutoUpdate",
                    View = GridColumnView.CheckBox,
                    Group = string.Empty,
                    Type = typeof(bool),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-8f5af4650b00")
                };
            }
        }

        public static EditableGridColumn QuantityInclusiveColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Кол. сод. узлов",
                    DataItem = "QuantityInclusive",
                    View = GridColumnView.TextBox,
                    Group = "Состав",
                    Type = typeof(int),
                    Size = 0,
                    ID = new Guid("aaacaaac-0000-0000-0000-20e975350324")
                };
            }
        }

        public static EditableGridColumn CommentColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Примечание",
                    DataItem = "Comment",
                    View = GridColumnView.TextBox,
                    AllowNULL = true,
                    Group = "Состав",
                    Type = typeof(string),
                    Width = 120,
                    Size = 250,
                    ID = new Guid("aaacaaac-0000-0000-0000-1000e15c90fb")
                };
            }
        }

        public static EditableGridColumn ZoneColumn
        {
            get
            {
                return new EditableGridColumn()
                {
                    Name = "Зона",
                    DataItem = "Zone",
                    Group = string.Empty,
                    AllowNULL = true,
                    View = GridColumnView.TextBox,
                    Type = typeof(string),
                    Size = 10,
                    ID = new Guid("aaacaaac-0000-0000-0000-2000e15c90fb")
                };
            }
        }

        #endregion
    }

    [Serializable]
    public class GridColumn
    {
        public GridColumn()
        {
            ID = Guid.Empty;
            IsDictionary = false;
            GridColumnType = TypeEnum.Default;
        }
        public static GridColumn SetOrder(GridColumn entity, int order)
        {
            entity.Order = order;
            return entity;
        }

        public Guid? ClassificationID { get; set; }

        public TypeEnum GridColumnType { get; set; }

        public bool IsDictionary { get; set; }

        public Guid ID { get; set; }

        public Guid SourceID { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string DataItem { get; set; }

        public string OrderExpression { get; set; }

        public string Group { get; set; }

        public int Order { get; set; }

        #region general columns

        public static GridColumn IdentifierColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Идентификатор",
                    DataItem = "Identifier",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-e201424a4b56")
                };
            }
        }

        public static GridColumn DesignationColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Обозначение",
                    DataItem = "dn_pn1",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-18804b663c79")
                };
            }
        }

        public static GridColumn TitlenameColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Наименование",
                    DataItem = "dn_pn2",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-258a4e137647")
                };
            }
        }

        public static GridColumn SpecColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "",
                    DataItem = "Spec",                    
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-e84c75338997")
                };
            }
        }

        public static GridColumn UserLEColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Пользователь",
                    DataItem = "UserName",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-6227d5a0cdda")
                };
            }
        }

        public static GridColumn DateLEColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Дата изменения",
                    DataItem = "dt_upd",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-16b543b0b7a7")
                };
            }
        }

        public static GridColumn EdIzmColumn
        {
            get
            {
                GridColumn column = new GridColumn()
                {
                    Name = "Ед. изм.",
                    DataItem = "EdIzmName",
                    Group = "Состав",
                    ID = new Guid("aaacaaac-0000-0000-0000-e00c9548e8f2")
                };
                return column;
            }
        }

        /* Наличие состава */
        public static GridColumn NsColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "НС",
                    DataItem = "p_ns",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-f9743c4560f9")
                };
            }
        }

        public static GridColumn MainVersionColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Осн. вер.",
                    DataItem = "MainVersion",
                    Group = string.Empty,
                    GridColumnType = TypeEnum.Boolean,
                    ID = new Guid("aaacaaac-0000-0000-0000-e3969c174134")
                };
            }
        }

        public static GridColumn OrderNumberColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Номер приказа",
                    DataItem = "OrderNumber",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-17c78ebf9859")
                };
            }
        }

        public static GridColumn OrderYearColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Год приказа",
                    DataItem = "OrderYear",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-018ba6ab4806")
                };
            }
        }

        public static GridColumn OrderWeightColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Вес по приказу",
                    DataItem = "OrderWeight",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-e7701e6f2f6f")
                };
            }
        }

        public static GridColumn DictFormatColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Формат",
                    DataItem = "DictFormat",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-808d8d2f9221")
                };
            }
        }

        public static GridColumn VersionColumn
        {
            get
            {
                return new GridColumn()
                {
                    Name = "Версия",
                    DataItem = "Version",
                    Group = string.Empty,
                    ID = new Guid("aaacaaac-0000-0000-0000-7d16ba705d25")
                };
            }
        }

        #endregion


    }
}
