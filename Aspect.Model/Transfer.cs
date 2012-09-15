using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections;

namespace Aspect
{
    public enum TechnDatesSpeciality
    {
        Main = 0,
        Him = 1,
        Svar = 2,
        Techn = 3
    }

    public class GuidAttribute : Attribute
    {
        public Guid guidValue = Guid.Empty;

        public GuidAttribute(Guid guidValue)
        {
            this.guidValue = guidValue;
        }

        public GuidAttribute(string guidValue)
        {
            this.guidValue = new Guid(guidValue);
        }
    }

    [DataContract]
    public class transfer_techn_dates
    {
        [DataMember]
        public Guid order_id { get; set; }

        [DataMember]
        public Guid dictnomen_id { get; set; }

        [DataMember]
        public DateTime? svar_date { get; set; }

        [DataMember]
        public DateTime? him_date { get; set; }

        [DataMember]
        public DateTime? techn_date { get; set; }

        [DataMember]
        public DateTime? gen_date { get; set; }

        [DataMember]
        public DateTime add_date { get; set; }
    }

    [DataContract]
    public class transfer_order_article
    {
        public bool ischecked { set; get; }
        [DataMember] 
        public Guid order_id;
        [DataMember] 
        public string year { set; get; }
        [DataMember] 
        public string cco { set; get; }
        [DataMember] 
        public DateTime created { set; get; }
        [DataMember]
        public bool exists_orderkmh { set; get; }
    }

    [DataContract]
    public class transfer_column 
    {        
        [DataMember]
        public Guid uid { get; set; }

        [DataMember]
        public int position { get; set; }
        
        [DataMember]
        public int width { get; set; }

        [DataMember]
        public bool hidden { get; set; }
    }

    public class transfer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Готовности
        [GuidAttribute("9ea4f9b8-f95a-47b9-9dce-1f1eaf91e368")]
        public bool gotov_tech { get; set; }
        [GuidAttribute("57fd91e0-f9fd-45b8-a5a0-68d61deb57ce")]
        public bool gotov_svar { get; set; }
        [GuidAttribute("8a61a7c5-c473-43fe-abae-3905ec6b138d")]
        public bool gotov_him { get; set; }

        [GuidAttribute("211A5B04-4C22-11E1-BBE3-5CC04824019B")]
        private DateTime? _added_date;
        public DateTime? added_date
        {
            get { return _added_date; }
            set
            {
                _added_date = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("added_date"));
            }
        }
        

        // Главная готовность
        [GuidAttribute("9bcd8e56-990a-4e82-8ec8-e83992ca9d1f")]
        public bool gotov_kmh { get; set; }
        public string gotov_date { get; set; }
        public string gotov_fio { get; set; }

        // Норма расхода
        [GuidAttribute("f727b3af-3b15-4502-a30a-33f032f7d74c")]
        public decimal? no { get; set; }

        // Масса заготовки
        [GuidAttribute("833676bb-f38f-4162-a65d-a5c434ac954a")]
        public decimal? sw { get; set; }

        // Масса штамповки
        [GuidAttribute("e81ad870-6c8e-4a3e-9581-0909e1432dc7")]
        public decimal? stw { get; set; }

        // Примечание        
        private string _cmt_org;
        [GuidAttribute("eecce8e3-d788-4930-8d5c-c07109e6ebe7")]
        public string cmt_ogt
        {
            get { return _cmt_org; }
            set
            {
                _cmt_org = String.IsNullOrEmpty(value) ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("cmt_org"));
                }
            }
        }        

        // Количество деталей из заготовки
        private string _sd;
        [GuidAttribute("268b28e2-8844-431c-93eb-6fb407a31944")]
        public string sd
        {
            get { return _sd; }
            set
            {
                _sd = String.IsNullOrEmpty(value) ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sd"));
                }
            }
        }

        // Размер поковки
        private string _sp;
        [GuidAttribute("13a97bce-2e03-4021-bca1-abb57eab2264")]
        public string sp
        {
            get { return _sp; }
            set
            {
                _sp = String.IsNullOrEmpty(value) ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sp"));
                }
            }
        }

        // Размер заготовки
        private string _ss;
        [GuidAttribute("8ca647b7-83ae-4f81-83fb-11f30c7b7529")]
        public string ss
        {
            get { return _ss; }
            set
            {
                _ss = String.IsNullOrEmpty(value) ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ss"));
                }
            }
        }

        #region Dictionary fields

        // Blank Guid's
        public const string UM_BLANK_VALUE = "7018C549-8F11-4ADF-8504-52932003586D";
        public const string SF_BLANK_VALUE = "9340FE45-402D-468C-9AC4-527B99601269";
        public const string PVD_BLANK_VALUE = "C23B8D6F-FFA7-45C2-8F3B-D6CA53566906";


        // Единица измерения
        private Guid _um_id = new Guid(UM_BLANK_VALUE);
        [GuidAttribute("367f8d6f-3e66-43bd-843a-d06be8517e1e")]
        public Guid? um_id
        {
            get { return _um_id; }
            set
            {
                if (value.HasValue)
                {
                    _um_id = value.Value;
                }
                else
                {
                    _um_id = new Guid(UM_BLANK_VALUE); // blank value for um
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("um_id"));
                }
            }

        }
        public string um { get; set; }

        // Форма заготовки 
        private Guid _sf_id = new Guid(SF_BLANK_VALUE);
        [GuidAttribute("abb06e98-fa3f-43eb-92b3-c86b8111c7e8")]
        public Guid? sf_id
        {
            get { return _sf_id; }
            set
            {
                if (value.HasValue)
                {
                    _sf_id = value.Value;
                }
                else
                {
                    _sf_id = new Guid(SF_BLANK_VALUE); // blank value for sf
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sf_id"));
                }
            }

        } 
        public string sf { get; set; }

        // Вид поставки
        private Guid _pvd_id = new Guid(PVD_BLANK_VALUE);
        [GuidAttribute("fd9a15af-071d-495d-8ede-ef8c43818f85")]
        public Guid? pvd_id
        {
            get { return _pvd_id; }
            set
            {
                if (value.HasValue)
                {
                    _pvd_id = value.Value;
                }
                else
                {
                    _pvd_id = new Guid(PVD_BLANK_VALUE); // blank value for pvd
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("pdv_id"));
                }
            }

        }
        public string pvd { get; set; }

        #endregion

        // Наименование материала
        public string material { get; set; }
        [GuidAttribute("edf1d520-35ba-4523-b4ce-eaea7e29c500")]
        public Guid? material_id { get; set; }

        // Маршрут 
        private string _route;
        [GuidAttribute("6f81de3e-52ee-404e-bee6-bf3f3c32086a")]
        public string route
        {
            get { return _route; }
            set
            {
                _route = value == null ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("route"));
                }
            }
        }

        private bool _route_changed;
        public bool route_changed
        {
            get { return _route_changed; }
            set
            {
                _route_changed = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("route_changed"));
            }
        }
        
        // Цех потребитель (!надо расчитать)
        public string route_ref { get; set; }
        
        // Кто заполнил
        public Guid? ste_id { get; set; }
        public string ste { get; set; }

        // Версия
        public bool _isprikaz;
        public bool isprikaz
        {
            get { return _isprikaz; }
            set
            {
                _isprikaz = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("isprikaz"));
                }
            }
        }

        // Актуальность
        public bool actual { get; set; }

        // По дате или по назначения
        public bool user_set { get; set; }

        public Guid unit_id { set; get; }

        #region Product's fields

        // Обозначение узла
        public string unit_pn1 { get; set; }

        // Наименование узла
        public string unit_pn2 { get; set; }

        public Guid prod_id { set; get; }

        // Обозначение детали
        public string prod_pn1 { get; set; }

        // Наименование детали
        public string prod_pn2 { get; set; }

        #endregion

        #region Configuration's fields

        // Количество
        public decimal? count { get; set; }

        // Уровень вхождения
        public decimal? level { get; set; }

        // Номер замены
        public decimal? number_exchange { get; set; }

        // Группа замены
        public decimal? group_exchange { get; set; }

        #endregion

        // Кто и Когда изменил

        private string _last_change_user;
        public string last_change_user
        {
            get { return _last_change_user; }
            set
            {
                _last_change_user = (value == null) ? "" : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("last_change_user"));
            }
        }

        public DateTime? last_change_date { set; get; }

        public List<Guid> enabled_fields = new List<Guid>();
    }

    public class DictItem
    {
        public int Index { set; get; }
        public Guid ID { set; get; }
        public string Name { set; get; }
    }

    public class Dicts
    {
        public List<DictItem> PVDs { set; get; }
        public List<DictItem> UMs { set; get; }
        public List<DictItem> SFs { set; get; }
        public List<DictItem> Ss { set; get; }
    }

    [DataContract]
    public class transfer_ware : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember(Name = "id")]
        public Guid ware_id { get; set; }
        [DataMember(Name = "pn1")]
        public string ware_pn1 { set; get; }
        [DataMember(Name = "pn2")]
        public string ware_pn2 { set; get; }

        private DateTime _created = DateTime.MinValue.ToUniversalTime();
        [DataMember(Name = "c")]
        public DateTime created
        {
            get { return _created; }
            set
            {
                _created = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("created"));
            }
        }
        
        private DateTime? _LastVersCreatedDate;
        [DataMember(Name = "lvcd")]
        public DateTime? LastVersCreatedDate
        {
            get { return _LastVersCreatedDate; }
            set
            {
                _LastVersCreatedDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LastVersCreatedDate"));
            }
        }

        [DataMember(Name = "oye")]
        public int order_year { set; get; }
        [DataMember(Name = "oid")]
        public Guid order_id { set; get; }
        [DataMember(Name = "onu")]
        public string order_number { set; get; }
        [DataMember(Name = "note")]
        public string note { set; get; }
        [DataMember(Name = "au")]
        public string author { set; get; }
        public DateTime? _date { set; get; }
        [DataMember(Name = "dt")]        
        public DateTime? date
        {
            get { return _date; }
            set
            {                
                _date = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("date"));
            }
        }

        [DataMember(Name = "tr")]
        public bool? gotov_tech { set; get; }
        [DataMember(Name = "hr")]
        public bool? gotov_him { set; get; }
        [DataMember(Name = "sr")]
        public bool? gotov_svar { set; get; }

        private DateTime? _gotov_techn_date;
        [DataMember(Name = "td")]
        public DateTime? gotov_techn_date
        {
            get { return _gotov_techn_date; }
            set
            {
                _gotov_techn_date = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("gotov_techn_date"));
            }
        }
        
        private DateTime? _gotov_him_date;
        [DataMember(Name = "hd")]
        public DateTime? gotov_him_date
        {
            get { return _gotov_him_date; }
            set
            {
                _gotov_him_date = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("gotov_him_date"));
            }
        }
        
        private DateTime? _gotov_svar_date;
        [DataMember(Name = "sd")]
        public DateTime? gotov_svar_date
        {
            get { return _gotov_svar_date; }
            set
            {
                _gotov_svar_date = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("gotov_svar_date"));
            }
        }
    }

    public class transfer_route : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }
            }
        }

        public Guid prodNomenID { get; set; }
        public Guid unitNomenID { get; set; }
        public string unit_pn1 { get; set; }
        public string unit_pn2 { get; set; }

        private string _route = "";
        public string route
        {
            get { return _route; }
            set
            {                
                _route = value == null ? "" : value.Trim();
                if (PropertyChanged != null)
                {                    
                    PropertyChanged(this, new PropertyChangedEventArgs("route"));
                }
            }
        }

        private string _comment = "";
        public string comment
        {
            get { return _comment; }
            set
            {
                _comment = value == null ? "" : value.Trim();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("comment"));
                }
            }
        }
        
        public string lastedit_author { get; set; }
        public DateTime? lastedit_date { get; set; }
    }    

    public class transfer_appl
    {
        public Guid prod_id { set; get; }

        public string pn1 { set; get; }
        public string pn2 { set; get; }
        public int count { set; get; }
        public int version { set; get; }
        public bool actual { set; get; }
        public string order_year { set; get; }
        public string order_number { set; get; }
        public bool top_ware { set; get; }
    }

    [DataContract]
    public class transfer_add : INotifyPropertyChanged
    {
        public transfer_add()
        {
            um_id = new Guid("7018C549-8F11-4ADF-8504-52932003586D");
            s_id = new Guid("4D1C53B9-C631-4BC0-B7BB-08E70004E952"); //D0ACF957-BCEE-4514-9171-4246A751C809
            ste_id = Guid.Empty;
        }

        public transfer_add(transfer_add orig)
        {
            this.material_id = orig.material_id;
            this.um_id = orig.um_id;
            this.s_id = orig.s_id;
            this.ste_id = orig.ste_id;
            this.no = orig.no;
        }
        
        // Дополнительный материал
        [DataMember]
        public Guid? material_id { set; get; }
        
        public event PropertyChangedEventHandler PropertyChanged;            

        private string _material;
        [DataMember]
        public string material
        {
            get
            {
                return _material;
            }
            set
            {
                _material = value;
                if (PropertyChanged != null)
                {
                    // notifies wpf about the property change
                    PropertyChanged(this, new PropertyChangedEventArgs("material"));
                }
            }
        }

        public List<DictItem> UMs { set; get; }
        public List<DictItem> Ss { set; get; }
        public List<DictItem> STEs { set; get; }

        public void UpdateDicts()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("um_id"));
            PropertyChanged(this, new PropertyChangedEventArgs("s_id"));
            PropertyChanged(this, new PropertyChangedEventArgs("ste_id"));
        }

        // Норма расхода
        [DataMember]
        public decimal? no { get; set; }

        // Единица измерения
        public Guid? _um_id;
        [DataMember]
        public Guid? um_id 
        {
            get
            {
                return _um_id;
            }
            set
            {
                _um_id = value;                
                if (UMs != null) um = UMs.Single(it => it.ID == _um_id).Name;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("um"));
                    PropertyChanged(this, new PropertyChangedEventArgs("um_id"));
                }
            }
        }
        public string um { get; set; }

        // Цех потребления
        public Guid? _s_id;
        [DataMember]
        public Guid? s_id 
        {
            get
            {
                return _s_id;
            }
            set
            {
                _s_id = value;
                if (Ss != null) s = Ss.Single(it => it.ID == _s_id).Name;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("s"));
                    PropertyChanged(this, new PropertyChangedEventArgs("s_id"));                    
                }
            }
        }
        public string s { set; get; }

        // Кто заполнил        
        private Guid _ste_id = Guid.Empty;
        [DataMember]
        public Guid? ste_id 
        {
            get
            {
                return _ste_id;
            }
            set
            {
                _ste_id = value.HasValue ? value.Value : Guid.Empty;
                if (STEs != null) ste = STEs.Single(it => it.ID == _ste_id).Name;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ste"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ste_id"));
                }
            }
        }
        public string ste { set; get; }

        public override bool Equals(object obj)
        {
 	        if (!(obj is transfer_add)) return false;

            var x = this;
            var y = obj as transfer_add;

            return (x.material_id == y.material_id
                && x.no == y.no
                && x.um_id == y.um_id
                && x.s_id == y.s_id
                && x.ste_id == y.ste_id);
        }      
    }

    public class PostResult
    {
        public DateTime TimeStamp { set; get; }
        public string Message { set; get; }
        public int Opcode { set; get; }    
    }

    /**
     * Universal grid
     **/

    public class UniColumn
    {
        public enum UniType
        {
            String,
            Boolean,
            Decimal,
            ProductMenu
        }

        public UniType uniType;
        public string header;
        public string dataBind;
    }

    [DataContract]
    public class UniTransfer
    {
        [DataMember]
        public List<UniColumn> columns;

        [DataMember]
        public List<object[]> rows;

        public int statusAnswer;
        public string messageAnswer; 
    }
}
