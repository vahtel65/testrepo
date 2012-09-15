using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.ComponentModel;
using Aspect;
using System.Diagnostics;
using System.Runtime.Serialization;
using Jib.Controls.DataGrid;
using System.Collections.Specialized;
using TreeWithKmh;

namespace EditorKmh
{
    public partial class MainPage : UserControl
    {
        public class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class AdditionalMaterial : INotifyPropertyChanged
        {
            public Guid Id { set; get; }

            public event PropertyChangedEventHandler PropertyChanged;

            private string _material;
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

            public string quantity { set; get; }
            public string unitmeasure { set; get; }
            public string guild { set; get; }
        }

        //public ObservableCollection<AdditionalMaterial> materials = new ObservableCollection<AdditionalMaterial>();

        private ObservableCollection<transfer_add> add_techn_materials = new ObservableCollection<transfer_add>();
        private ObservableCollection<transfer_add> add_him_materials = new ObservableCollection<transfer_add>();
        private ObservableCollection<transfer_add> add_svar_materials = new ObservableCollection<transfer_add>();

        public Guid? OrderID
        {
            get
            {
                IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
                if (!urlparams.Keys.Contains("orderid"))
                {
                    return null;
                }
                else
                {
                    return new Guid(urlparams["orderid"]);
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();

            // Change button saving state
            if (OrderID == null)
            {
                btnKmhSaveBoth.Visibility = Visibility.Collapsed;
                btnKmhSaveOrder.Visibility = Visibility.Collapsed;

                btnAddKmhSaveBoth.Visibility = Visibility.Collapsed;
                btnAddKmhSaveOrder.Visibility = Visibility.Collapsed;
            }

            //Set up some scriptable managed types for access from Javascript.
            HtmlPage.RegisterScriptableObject("myApp", this);

            add_svar_materials.CollectionChanged += add_materials_CollectionChanged;
            add_him_materials.CollectionChanged += add_materials_CollectionChanged;
            add_techn_materials.CollectionChanged += add_materials_CollectionChanged;

            // Init @Additional materials            
            SvarMaterials.ItemsSource = add_svar_materials;
            HimMaterials.ItemsSource = add_him_materials;
            TechnMaterials.ItemsSource = add_techn_materials;

            Loaded += RequestDicts;

            KmhInfo.Text = (string)HtmlPage.Document.GetProperty("title");
        }

        private void add_materials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (sender == this.add_svar_materials)
                {
                    foreach (transfer_add newItem in e.NewItems)
                    {
                        newItem.ste_id = new Guid("61931973-A5BD-40CD-92A6-FA802DE6CE6A");
                    }
                }
                if (sender == this.add_him_materials)
                {
                    foreach (transfer_add newItem in e.NewItems)
                    {
                        newItem.ste_id = new Guid("46A00C26-1768-4521-9A33-88336E65D50C");
                    }
                }
                if (sender == this.add_techn_materials)
                {
                    foreach (transfer_add newItem in e.NewItems)
                    {
                        newItem.ste_id = new Guid("BCE12453-3AB9-4FCB-8FB3-4811A311B764");
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (transfer_add newItem in e.NewItems)
                {
                    newItem.PropertyChanged += new PropertyChangedEventHandler(add_materials_PropertyChanged);
                }
            }
            if (e.OldItems != null)
            {
                foreach (transfer_add oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= new PropertyChangedEventHandler(add_materials_PropertyChanged);
                }
            }
        }

        private void add_materials_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ste_id")
            {
                var changedItem = sender as transfer_add;
                ObservableCollection<transfer_add> oldList = null;
                ObservableCollection<transfer_add> newList = null;

                if (add_techn_materials.Contains(changedItem))
                {
                    oldList = add_techn_materials;
                }
                else if (add_him_materials.Contains(changedItem))
                {
                    oldList = add_him_materials;
                }
                else if (add_svar_materials.Contains(changedItem))
                {
                    oldList = add_svar_materials;
                }
                else 
                {
                    return;
                }

                if (changedItem.ste_id == new Guid("BCE12453-3AB9-4FCB-8FB3-4811A311B764"))
                {
                    newList = this.add_techn_materials;
                } 
                else if (changedItem.ste_id == new Guid("46A00C26-1768-4521-9A33-88336E65D50C"))
                {
                    newList = this.add_him_materials;
                }
                else if (changedItem.ste_id == new Guid("61931973-A5BD-40CD-92A6-FA802DE6CE6A"))
                {
                    newList = this.add_svar_materials;
                }
                else
                {
                    return;
                }

                if (oldList != newList)
                {
                    oldList.Remove(changedItem);
                    newList.Add(changedItem);
                }
            }
        }


        /*public class Request
        {
            public string dictNomenID { get; set; }
            public string orderNumber { get; set; }
        }

        public class Answer
        {
            public transfer d { set; get; }
        }*/

        private transfer _transfer;
        public Dicts _dicts;        

        #region Getting KMH Card (2 time)

        private void RequestKmhCard(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Загрузка КМХ...");

            PostRequest<transfer> post = new PostRequest<transfer>(this.Dispatcher, "/Technology/Service.aspx/RequestKmhCard");
            post.ProcessResponse += this.ProcessKmhCard;
            post.ProcessError += this.ProcessKmhCardError;

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform(string.Format("{{ 'prod_id': '{0}', 'order_id': '{1}' }}", urlparams["prodid"], urlparams.Keys.Contains("orderid") ? urlparams["orderid"] : Guid.Empty.ToString()));
        }

        public void ProcessKmhCard(transfer transfer)
        {
            string pageTitle = (string)HtmlPage.Document.GetProperty("title");
            pageTitle = pageTitle.Replace("%state%", transfer.isprikaz ? "приказной" : "стандартной");
            KmhInfo.Text = pageTitle;
            HtmlPage.Document.SetProperty("title", pageTitle);

            #region Setting enable permissions
            List<KeyValuePair<Guid, Control>> fieldControls = new List<KeyValuePair<Guid, Control>>();
            /*fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("8a61a7c5-c473-43fe-abae-3905ec6b138d"), ctrl_gotov_him));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("57fd91e0-f9fd-45b8-a5a0-68d61deb57ce"), ctrl_gotov_svar));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("9ea4f9b8-f95a-47b9-9dce-1f1eaf91e368"), ctrl_gotov_tech));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("9bcd8e56-990a-4e82-8ec8-e83992ca9d1f"), ctrl_gotov_kmh));*/
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("edf1d520-35ba-4523-b4ce-eaea7e29c500"), ctrl_material));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("fd9a15af-071d-495d-8ede-ef8c43818f85"), unitPVD));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("abb06e98-fa3f-43eb-92b3-c86b8111c7e8"), unitSF));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("367f8d6f-3e66-43bd-843a-d06be8517e1e"), unitUM));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("6f81de3e-52ee-404e-bee6-bf3f3c32086a"), unitRouteButton));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("f727b3af-3b15-4502-a30a-33f032f7d74c"), ctrl_no));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("833676bb-f38f-4162-a65d-a5c434ac954a"), ctrl_sw));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("e81ad870-6c8e-4a3e-9581-0909e1432dc7"), ctrl_stw));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("268b28e2-8844-431c-93eb-6fb407a31944"), ctrl_sd));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("13a97bce-2e03-4021-bca1-abb57eab2264"), ctrl_sp));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("8ca647b7-83ae-4f81-83fb-11f30c7b7529"), ctrl_ss));

            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("529cf0b5-e404-4b66-804a-aee36b0a1d4c"), btnAddKmhSaveStand));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("48c44ed9-bdd3-4305-87d6-1582614b014b"), btnAddKmhSaveOrder));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("529cf0b5-e404-4b66-804a-aee36b0a1d4c"), btnAddKmhSaveBoth));
            fieldControls.Add(new KeyValuePair<Guid, Control>(new Guid("48c44ed9-bdd3-4305-87d6-1582614b014b"), btnAddKmhSaveBoth));

            foreach (var pair in fieldControls)
            {
                if (!transfer.enabled_fields.Contains(pair.Key))
                {
                    pair.Value.IsEnabled = false;
                }
            }

            #endregion

            // ...

            _transfer = transfer;
            gridKmh.DataContext = _transfer;

            RequestAddMaterials(this, new RoutedEventArgs());
        }

        public void ProcessKmhCardError()
        {
            HideGlobalMask();
            MessageBox.Show("Ошибка получения КМХ");
        }

        #endregion

        #region Getting Additional materials

        private void RequestAddMaterials(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Загрузка дополнительных материалов...");

            PostRequest<List<transfer_add>> post = new PostRequest<List<transfer_add>>(this.Dispatcher, "/Technology/Service.aspx/RequestAddMaterials");
            post.ProcessResponse += this.ProcessAddMaterials;

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform(string.Format("{{ 'prod_id': '{0}', 'order_id': '{1}' }}", urlparams["prodid"], urlparams.Keys.Contains("orderid") ? urlparams["orderid"] : Guid.Empty.ToString()));
        }

        public void ProcessAddMaterials(List<transfer_add> list)
        {
            foreach (var item in list)
            {
                item.UMs = _dicts.UMs;
                item.Ss = _dicts.Ss;
                item.STEs = _dicts.STEs;
                
                if (item.ste_id == new Guid("BCE12453-3AB9-4FCB-8FB3-4811A311B764"))
                {
                    this.add_techn_materials.Add(item);
                }
                else if (item.ste_id == new Guid("46A00C26-1768-4521-9A33-88336E65D50C"))
                {
                    this.add_him_materials.Add(item);
                }
                else if (item.ste_id == new Guid("61931973-A5BD-40CD-92A6-FA802DE6CE6A"))
                {
                    this.add_svar_materials.Add(item);
                }
                
                item.UpdateDicts();
            }

            HideGlobalMask();
        }

        public void ProcessAddMaterialsError()
        {
            HideGlobalMask();
            MessageBox.Show("Ошибка получения дополнительных материалов");
        }

        #endregion

        #region Getting Dictionaries

        public class Dicts
        {
            public List<DictItem> PVDs { set; get; }
            public List<DictItem> UMs { set; get; }
            public List<DictItem> SFs { set; get; }
            public List<DictItem> Ss { set; get; }
            public List<DictItem> STEs { set; get; }
        }

        private void RequestDicts(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Загрузка словарей...");

            PostRequest<Dicts> post = new PostRequest<Dicts>(this.Dispatcher, "/Technology/Service.aspx/RequestDicts");
            post.ProcessResponse += this.ProcessDicts;
            post.ProcessError += this.ProcessDictsError;

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform("{ 'dicts': 'pvd,um,sf,s,ste' }");
        }

        public void ProcessDicts(Dicts dicts)
        {
            _dicts = dicts;

            unitPVD.ItemsSource = _dicts.PVDs;
            unitPVD.DisplayMemberPath = "Name";
            unitPVD.SelectedValuePath = "ID";

            unitUM.ItemsSource = _dicts.UMs;
            unitUM.DisplayMemberPath = "Name";
            unitUM.SelectedValuePath = "ID";

            unitSF.ItemsSource = _dicts.SFs;
            unitSF.DisplayMemberPath = "Name";
            unitSF.SelectedValuePath = "ID";

            RequestKmhCard(this, new RoutedEventArgs());
        }

        public void ProcessDictsError()
        {
            HideGlobalMask();
            MessageBox.Show("Ошибка загрузки материалов");
        }

        #endregion

        #region Service functions

        private void Log(string str)
        {
            HtmlPage.Window.Invoke("consolelog", str);
        }

        [ScriptableMember()]
        public void ShowGlobalMask(string message)
        {
            LayoutRoot.BusyContent = message;
            LayoutRoot.IsBusy = true;
        }

        [ScriptableMember()]
        public void HideGlobalMask()
        {
            LayoutRoot.IsBusy = false;
        }

        #endregion

        private void addMaterial(object sender, RoutedEventArgs e)
        {
            transfer_add new_AddMaterial = new transfer_add()
            {
                UMs = this._dicts.UMs,
                Ss = this._dicts.Ss,
                STEs = this._dicts.STEs
            };

            DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
            ObservableCollection<transfer_add> add_materials = dataGrid.ItemsSource as ObservableCollection<transfer_add>;

            add_materials.Add(new_AddMaterial);
            new_AddMaterial.UpdateDicts();
        }

        private void addMultiMaterial(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Загрузка списка дополнительных материалов...");
            HtmlPage.Window.Invoke("SelectMaterial", "-1", "multi");
        }

        private void delMaterial(object sender, RoutedEventArgs e)
        {
            try
            {
                List<object> toDelete = new List<object>();

                DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
                foreach (var gridRow in dataGrid.SelectedItems)
                {
                    toDelete.Add(gridRow);
                }
                
                var add_materials = dataGrid.ItemsSource as ObservableCollection<transfer_add>;

                toDelete.ForEach(it => add_materials.Remove((transfer_add)it));
            }
            catch (Exception except) { MessageBox.Show(except.Message); }
        }

        // Выбор материалов для основной и дополнительной КМХ
        #region selectorMaterials

        private void SelectMaterial(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Загрузка списка материалов...");
            HtmlPage.Window.Invoke("SelectMaterial", "", "");
        }

        private void SelectAdditionalMaterial(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
            int index = dataGrid.SelectedIndex;
            ShowGlobalMask("Загрузка списка дополнительных материалов...");
            HtmlPage.Window.Invoke("SelectMaterial", index.ToString(), "");
        }

        [ScriptableMember()]
        public void SetSelectedMaterial(string id, string materialId, string materialName)
        {
            if (id.Length == 0)
            {
                // set main material
                unitMaterial.Text = materialName;
                unitMaterialID.Text = materialId;
            }
            else
            {
                // set additional material
                DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
                transfer_add add_material = (transfer_add)dataGrid.SelectedItem;// this.add_materials.ElementAt(Convert.ToInt32(id));
                add_material.material = materialName;
                add_material.material_id = new Guid(materialId);
            }
        }

        [ScriptableMember()]
        public void AddMultiMaterials(string packedMaterials)
        {
            var pairs = packedMaterials.Split(';').Where(i => !String.IsNullOrEmpty(i));

            foreach (var pair in pairs)
            {
                Guid materialId = new Guid(pair.Split(':').First());

                String materialName = EditorKmh.Encoding.UTF8.GetString(Convert.FromBase64String(pair.Split(':').Last()));

                transfer_add added = new transfer_add()
            {
                UMs = this._dicts.UMs,
                Ss = this._dicts.Ss,
                STEs = this._dicts.STEs,
                material_id = materialId,
                material = materialName
            };
                DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
                var add_materials = dataGrid.ItemsSource as ObservableCollection<transfer_add>;

                add_materials.Add(added);
                added.UpdateDicts();
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Log(this._transfer.pvd_id.ToString());
        }
        
        [DataContract]
        public class PostResult
        {
            [DataMember]
            public string Message { set; get; }
            [DataMember]
            public int Opcode { set; get; }
            [DataMember]
            public DateTime TimeStamp { set; get; }
        }

        public class SaveKmhCard_PROTO
        {
            public transfer card { set; get; }
            public int saveType { set; get; }
            public Guid order_id { set; get; }
        }

        public class SaveAddMaterials_PROTO
        {
            public List<transfer_add> list { set; get; }
            public Guid prodid { set; get; }
            public int saveType { set; get; }
            public Guid order_id { set; get; }
            public Guid ste_id { set; get; }
        }

        private void SaveKmhCard(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int saveType = Convert.ToInt32(button.Tag as string);           

            ShowGlobalMask("Сохранение...");
            
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveKmhCard");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate (PostResult result)
                {
                    #region Process Response
                    MessageBox.Show("Сохранение выполнено успешно");
                    HideGlobalMask();                    

                    // show modal window with list of orders 
                    // what use this _standart_ (none order) KMH card 
                    Guid order_id = urlparams.Keys.Contains("orderid") ? new Guid(urlparams["orderid"]) : Guid.Empty;
                    if (saveType == 1 || saveType == 3)
                    {
                        KmhOrderApplicability ordersForm = new KmhOrderApplicability();
                        ordersForm._dictNomenID = this._transfer.prod_id;
                        ordersForm.timeStamp = result.TimeStamp;
                        ordersForm.orderID = order_id;
                        ordersForm.Show();
                    }

                    if (_transfer.isprikaz && (saveType == 3 || saveType == 1))
                    {
                        string uri = String.Format("/Technology/EditorKmh.aspx?prodid={0}", new Guid(urlparams["prodid"]));
                        HtmlPage.Window.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute), "_newWindow");
                    }
                    #endregion
                });
            post.ProcessError += this.ProcessSavingError;
            
            post.Perform(new SaveKmhCard_PROTO()
            {
                card = this._transfer,
                saveType = saveType,
                order_id = urlparams.Keys.Contains("orderid") ? new Guid(urlparams["orderid"]) : Guid.Empty
            });
        }

        public void ProcessSavingError()
        {
            MessageBox.Show("Ошибка при сохранении");
            HideGlobalMask();
        }

        /*public void ProcessSaving(PostResult result)
        {            
        }*/

        private void SaveAddMaterials(object sender, RoutedEventArgs e)
        {
            // проверяем установлено ли для всех дополнительных материалов
            // поле "Кто заполнил"
            DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
            var add_materials = dataGrid.ItemsSource as ObservableCollection<transfer_add>;

            if (add_materials.Where(it => Guid.Empty.Equals(it.ste_id)).Count() > 0)
            {
                MessageBox.Show("Для сохранения необходимо заполнить все поля [Кто заполнил]!");
                return;
            }

            ShowGlobalMask("Сохранение...");

            Button button = sender as Button;
            int saveType = Convert.ToInt32(button.Tag as string);

            List<transfer_add> savedMaterials = new List<transfer_add>();

            foreach (var material in add_materials)
            {
                savedMaterials.Add(new transfer_add(material));
            }

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;

            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveAddMaterials");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate (PostResult result)
            {
                #region Process Response
                MessageBox.Show("Сохранение выполнено успешно");
                HideGlobalMask();

                // show modal window with list of orders 
                // what use this _standart_ (none order) KMH card 
                Guid order_id = urlparams.Keys.Contains("orderid") ? new Guid(urlparams["orderid"]) : Guid.Empty;
                if (saveType == 1 || saveType == 3)
                {
                    var speciality = TechnDatesSpeciality.Him;

                    if (add_materials == add_him_materials) speciality = TechnDatesSpeciality.Him;
                    else if (add_materials == add_svar_materials) speciality = TechnDatesSpeciality.Svar;
                    else if (add_materials == add_techn_materials) speciality = TechnDatesSpeciality.Techn;
            
                    KmhOrderApplicability ordersForm = new KmhOrderApplicability()
                    {
                        _dictNomenID = this._transfer.prod_id,
                        timeStamp = result.TimeStamp,
                        orderID = order_id,
                        speciality = speciality
                    };                    
                    ordersForm.Show();
                }

                if (_transfer.isprikaz && (saveType == 3 || saveType == 1))
                {
                    string uri = String.Format("/Technology/EditorKmh.aspx?prodid={0}", new Guid(urlparams["prodid"]));
                    HtmlPage.Window.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute), "_newWindow");
                }                   
                #endregion
            });
            post.ProcessError += this.ProcessSavingError;

            Guid ste_id = Guid.Empty;            
            if (add_materials == add_him_materials)
            {
                ste_id = new Guid("46A00C26-1768-4521-9A33-88336E65D50C");
            }
            else if (add_materials == add_svar_materials)
            {
                ste_id = new Guid("61931973-A5BD-40CD-92A6-FA802DE6CE6A");
            }
            else if (add_materials == add_techn_materials)
            {
                ste_id = new Guid("BCE12453-3AB9-4FCB-8FB3-4811A311B764");
            }
            else
            {
                return;
            }

            post.Perform(new SaveAddMaterials_PROTO()
            {
                list = savedMaterials,
                prodid = this._transfer.prod_id,
                saveType = saveType,
                order_id = urlparams.Keys.Contains("orderid") ? new Guid(urlparams["orderid"]) : Guid.Empty,
                ste_id = ste_id
            });
        }

        private void ShowSelectorWindow(object sender, RoutedEventArgs e)
        {
            List<DictItem> newRoute = new List<DictItem>();
            SelectionRoute selectionRoute = new SelectionRoute(_dicts.Ss.Select(m => m.Name).ToList(), this._transfer.route);
            selectionRoute.Show();
            selectionRoute.Closed += new EventHandler(selectionRoute_Closed);
        }

        void selectionRoute_Closed(object sender, EventArgs e)
        {
            SelectionRoute selectionRoute = sender as SelectionRoute;
            if ((bool)selectionRoute.DialogResult)
            {
                unitRoute.Text = selectionRoute.ResultRoute;
                unitRouteButton.Tag = selectionRoute.ResultRoute;
                this._transfer.route = selectionRoute.ResultRoute;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            MessageBox.Show(((sender as ComboBox).DataContext as transfer_add).material);
        }

        #region Clipboard routines

        public class BufferInsert_PROTO
        {
            public List<transfer_add> inserted;
        };

        private void copyToBuffer(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Копирование в буфер обмена...");

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;

            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Buffer.aspx/Insert");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate(PostResult result)
            {
                if (result.Opcode != 0)
                {
                    MessageBox.Show(result.Message);
                }
                HideGlobalMask();
            });
            post.ProcessError += new PostRequest<PostResult>.ProcessErrorEvent(delegate()
            {
                MessageBox.Show("Ошибка при копировании в буфер");
                HideGlobalMask();
            });

            DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
            post.Perform(new BufferInsert_PROTO() { inserted = dataGrid.SelectedItems.Cast<transfer_add>().ToList() });
        }

        private void insertFromBuffer(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask("Вставка из буфера обмена...");

            PostRequest<List<transfer_add>> post = new PostRequest<List<transfer_add>>(this.Dispatcher, "/Technology/Buffer.aspx/Select");
            post.ProcessResponse += new PostRequest<List<transfer_add>>.ProcessResponseEvent(delegate(List<transfer_add> selected)
            {
                DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;
                var add_materials = dataGrid.ItemsSource as ObservableCollection<transfer_add>;

                foreach (var item in selected)
                {
                    item.UMs = _dicts.UMs;
                    item.Ss = _dicts.Ss;
                    item.STEs = _dicts.STEs;
                    add_materials.Add(item);
                    item.UpdateDicts();
                }
                HideGlobalMask();
            });
            post.ProcessError += new PostRequest<List<transfer_add>>.ProcessErrorEvent(delegate()
            {
                MessageBox.Show("Ошибка при вставке из буфера");
                HideGlobalMask();
            });

            post.Perform("{}");
        }

        #endregion

        private void showAppicability(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (AddTabs.SelectedItem as TabItem).Content as DataGrid;

            if (dataGrid.SelectedItems.Count == 1)
            {
                transfer_add selected = ((transfer_add)dataGrid.SelectedItem);
                if (selected.material_id != null || selected.material_id != Guid.Empty)
                {
                    ShowGlobalMask("Загрузка применяемости...");
                    HtmlPage.Window.Invoke("ShowMaterialAppicability", selected.material_id.ToString());
                }
            }
            else
            {
                MessageBox.Show("Выделите один дополнительный материал");
            }
        }

        private void showMainAppicability(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(unitMaterialID.Text))
            {
                ShowGlobalMask("Загрузка применяемости...");
                HtmlPage.Window.Invoke("ShowMaterialAppicability", unitMaterialID.Text.ToString());
            }
            else
            {
                MessageBox.Show("Выберите основной материал");
            }
        }

        #region Readiness

        public class SaveReadiness_PROTO
        {
            public Guid _dictNomenID { set; get; }
            public Guid OrderArticleID { set; get; }
            public DateTime? gen_date { set; get; }
            public DateTime? him_date { set; get; }
            public DateTime? svar_date { set; get; }
            public DateTime? techn_date { set; get; }

        }

        private void ctrl_gotov_Click(object sender, RoutedEventArgs e)
        {
           /* if (OrderID != null)
            {
                PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/TechDatesSave");
                post.ProcessResponse += delegate(PostResult res)
                {
                };
                post.ProcessError += delegate()
                {
                };

                DateTime? him = ((sender as CheckBox).IsChecked.Value) ? DateTime.Now : DateTime.MinValue;
                DateTime? techn = ((sender as CheckBox).IsChecked.Value) ? DateTime.Now : DateTime.MinValue;                
                DateTime? svar = ((sender as CheckBox).IsChecked.Value) ? DateTime.Now : DateTime.MinValue;
                DateTime? gen = ((sender as CheckBox).IsChecked.Value) ? DateTime.Now : DateTime.MinValue;
                                       
                post.Perform(new SaveReadiness_PROTO()
                {
                    _dictNomenID = this._transfer.prod_id,
                    OrderArticleID = this.OrderID.Value,
                    gen_date = (sender == ctrl_gotov_kmh) ? gen : null,
                    svar_date = (sender == ctrl_gotov_svar) ? svar : null,                    
                    techn_date = (sender == ctrl_gotov_tech) ? techn : null,
                    him_date = (sender == ctrl_gotov_him) ? him : null,
                });
            }*/
        }

        #endregion       

        private void btnShowReadinessForm(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;            
            if (!urlparams.Keys.Contains("orderid"))
            {
                MessageBox.Show("Возможность проставить готовность есть только для приказных составов");
                return;
            }

            var current_row = this._transfer;

            var modalForm = new ReadinessForm()
            {
                product_pn1 = current_row.prod_pn2,
                product_id = new Guid(urlparams["prodid"]),
                order_id = new Guid(urlparams["orderid"]),
                current_date = DateTime.Now,
            };
            modalForm.Show();
        }
    }
}
