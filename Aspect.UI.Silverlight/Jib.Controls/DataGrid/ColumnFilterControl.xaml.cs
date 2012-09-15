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
using System.Windows.Shapes;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Jib.Controls.DataGrid
{
    /// <summary>
    /// The ColumnFilterHeader contains the logic needed to support column filter.  This control resides in the header template for each column.
    /// </summary>
    public partial class ColumnFilterControl  : UserControl, INotifyPropertyChanged
    {
        //When the control is configured by calling ResetFilterValues() we create a property accessor.  This accessor is used when 
        //we need to get the distinct list of values for the column.
        private Func<object, object> _boundColumnPropertyAccessor = null;

        #region Properties
        /// <summary>
        /// This is the list of avl FilterOperations.  This is maintained by the control and set given the property type the control is bound to
        /// </summary>
        public ObservableCollection<FilterOperationItem> FilterOperations { get; private set; }

        /// <summary>
        /// This list contains the distinct values determined by the _boundColumnPropertyAccessor for each item in the orginal source
        /// of the grid.
        /// </summary>
        public ObservableCollection<CheckboxComboItem> DistinctPropertyValues { get; private set; }

        /// <summary>
        /// When a predicate changes on any of the ColumnFilterHeaders for the grid.  A new predicate has to be created for the PagedCollectionView
        /// of the grid.  This is a simple check the grid uses to see if it needs to have the control generate a new predicate
        /// </summary>
        public bool HasPredicate { get { return FilterText.Length > 0 || DistinctPropertyValues.Where(d => d.IsChecked).Count() > 0; } }

        /// <summary>
        /// The FilterColumnInfo has all the information about the column-binding which the ColumnFilterHeader is associated with.
        /// </summary>
        public FilterColumnInfo FilterColumnInfo { get; set; }

        /// <summary>
        /// The associated Column for the ColumnFilterHeader
        /// </summary>
        public DataGridColumn Column { get; set; }

        /// <summary>
        /// The associated Grid for the ColumnFilterHeader
        /// </summary>
        public JibGrid Grid { get; set; }

        /// <summary>
        /// When the user selects a distinct property, they can no longer type in a custom filter.  
        /// This property is bound to the textbox readonly property and is set when there is a change to the DistinctPropertyValues collection
        /// </summary>
        public bool FilterReadOnly
        {
            get { return DistinctPropertyValues.Where(i => i.IsChecked).Count() > 0; }
        }

        private string _FilterText = string.Empty;
        /// <summary>
        /// The user can enter a distinct filter value.  This property holds the user entered text
        /// </summary>
        public string FilterText
        {
            get { return _FilterText; }
            set
            {
                if (value != _FilterText)
                {
                    _FilterText = value;
                    OnPropertyChanged("FilterText");
                }
            }
        }


        private FilterOperationItem _SelectedFilterOperation;
        /// <summary>
        /// Selected Filter Operation
        /// </summary>
        public FilterOperationItem SelectedFilterOperation
        {
            get { return _SelectedFilterOperation; }
            set
            {
                if (value != _SelectedFilterOperation)
                {
                    _SelectedFilterOperation = value;
                    OnPropertyChanged("SelectedFilterOperation");
                }
            }
        }
        #endregion

        #region Dependent Property
        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(object), typeof(ColumnFilterControl), new PropertyMetadata("", new PropertyChangedCallback(OnHeaderContentChanged)));
        public static void OnHeaderContentChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            // Get reference to self
            ColumnFilterControl source = (ColumnFilterControl)sender;

            // Add Handling Code
            string newValue = (string)args.NewValue;
        }
        /// <summary>
        /// Since the ColumnFilterHeader is defined in the style for the datagrid, the only way to associate the correct column with the correct
        /// ColumnFilterHeader is to compare this HeaderContent to the columns HeaderContent.
        /// </summary>
        public object HeaderContent
        {
            get
            {
                return (string)GetValue(HeaderContentProperty);
            }
            set
            {
                SetValue(HeaderContentProperty, value);
            }
        }
        #endregion

        #region IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        #endregion

        public ColumnFilterControl()
        {
            DistinctPropertyValues = new ObservableCollection<CheckboxComboItem>();
            FilterOperations = new ObservableCollection<FilterOperationItem>();
            InitializeComponent();
            this.DataContext = this;
            //By default this control is hidden.  This is so that columns that we don't see filter   options for columns
            //which we can't filter
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Reset the avl filter operations given the property type.  This is called each time the control is configured by calling ResetFilterValues
        /// </summary>
        private void ConfigureFilterOptions()
        {
            FilterOperations.Clear();
            if (FilterColumnInfo.PropertyType != null)
            {
                if (TypeHelper.IsStringType(FilterColumnInfo.PropertyType))
                {
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.Contains, "Содержит", "/Jib.Controls;component/Images/Contains.png"));
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.StartsWith, "Начинается с", "/Jib.Controls;component/Images/StartsWith.png"));
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.EndsWith, "Заканчивается на", "/Jib.Controls;component/Images/EndsWith.png"));
                }
                FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.Equals, "Равно", "/Jib.Controls;component/Images/Equal.png"));
                if (TypeHelper.IsNumbericType(FilterColumnInfo.PropertyType))
                {
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.GreaterThan, "Больше чем", "/Jib.Controls;component/Images/GreaterThan.png"));
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.GreaterThanEqual, "Больше или равно", "/Jib.Controls;component/Images/GreaterThanEqual.png"));
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.LessThan, "Меньше чем", "/Jib.Controls;component/Images/LessThan.png"));
                    FilterOperations.Add(new FilterOperationItem(Enums.FilterOperation.LessThanEqual, "Меньше или равно", "/Jib.Controls;component/Images/LessThanEqual.png"));
                }
                SelectedFilterOperation = FilterOperations[0];
            }
        }

        //Since the filter textbox is defined in the style, we can only get a reference to it when it loads.  Interactions with the
        //textbox are handled though databinding.  Because of this the datacontext needs to be set to this class.
        private void txtFilter_Loaded(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).DataContext = this;
        }

        // We want to update the FilterText property each time the text is changed.  This is not normal binding behavior
        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            FilterText = ((TextBox)sender).Text;
        }

        //In order to make the grid more performant (sp?) we shouldn't load the distinct value list until the user wants to use it.
        //this method fires before we show the distinct options.  In this method we use the property accessor and get the distinct list.
        private void cbDistinctProperties_DropDownOpened(object sender, EventArgs e)
        {
            if (_boundColumnPropertyAccessor != null)
            {
                if (DistinctPropertyValues.Count == 0)
                {
                    List<object> result = new List<object>();
                    foreach (var i in Grid.OrginalSource)
                    {
                        object value = _boundColumnPropertyAccessor(i);
                        if (value != null)
                            if (result.Where(o => o.ToString() == value.ToString()).Count() == 0)
                                result.Add(value);
                    }
                    //should we throw an exception if we can't order the items?  prob not.
                    try
                    {
                        result.Sort();
                    }
                    catch
                    {
                        if (System.Diagnostics.Debugger.IsLogging())
                            System.Diagnostics.Debugger.Log(0, "Warning", "There is no default compare set for the object type");
                    }
                    foreach (var obj in result)
                    {
                        var item = new CheckboxComboItem()
                        {
                            Description = GetFormattedValue(obj),
                            Tag = obj,
                            IsChecked = false
                        };
                        item.PropertyChanged += new PropertyChangedEventHandler(filter_PropertyChanged);
                        DistinctPropertyValues.Add(item);
                    }
                }
            }
        }

        //When the user user selects a distinct value.  The FilterText should be disables for custom entry. 
        void filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var list = DistinctPropertyValues.Where(i => i.IsChecked).ToList();
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var i in DistinctPropertyValues.Where(i => i.IsChecked))
                    sb.AppendFormat("{0}{1}", sb.Length > 0 ? "," : "", i);
                FilterText = sb.ToString();
            }
            else
            {
                FilterText = string.Empty;
            }
            OnPropertyChanged("FilterReadOnly");
        }

        //For each distinct value, we need to get the converter if one is defined for the property and get the visible value
        private string GetFormattedValue(object obj)
        {
            if (FilterColumnInfo.Converter != null)
                return FilterColumnInfo.Converter.Convert(obj, typeof(string), FilterColumnInfo.ConverterParameter, FilterColumnInfo.ConverterCultureInfo).ToString();
            else
                return obj.ToString();
        }

        //In reality there are two different methods for generating the predicate for the column.  If the user enters a custom filter, then we return the single predicate
        //if the user has selected multiple values out of the distinct list, we need to return a predicate that or each selected item.
        internal Predicate<object> GeneratePredicate()
        {
            Predicate<object> predicate = null;
            if (DistinctPropertyValues.Where(i => i.IsChecked).Count() > 0)
            {
                foreach (var item in DistinctPropertyValues.Where(i => i.IsChecked))
                {
                    if (predicate == null)
                        predicate = GenerateFilterPredicate(FilterColumnInfo.PropertyPath, item.Tag.ToString(), Grid.FilterType, FilterColumnInfo.PropertyType, SelectedFilterOperation);
                    else
                        predicate = predicate.Or(GenerateFilterPredicate(FilterColumnInfo.PropertyPath, item.Tag.ToString(), Grid.FilterType, FilterColumnInfo.PropertyType.UnderlyingSystemType, SelectedFilterOperation));
                }
            }
            else
            {
                predicate = GenerateFilterPredicate(FilterColumnInfo.PropertyPath, FilterText, Grid.FilterType, FilterColumnInfo.PropertyType.UnderlyingSystemType, SelectedFilterOperation);
            }
            return predicate;
        }

        /// <summary>
        /// This method creates the predicate for the given operation and text
        /// </summary>
        /// <param name="propertyName">The property which the header column is bound to</param>
        /// <param name="filterValue">The value for the predicate.</param>
        /// <param name="objType">The object type of the grid itemsource</param>
        /// <param name="propType">They type for the property</param>
        /// <param name="filterItem">The type of filter (i.e. Contains, Equals...)</param>
        /// <returns>The predicate</returns>
        protected Predicate<object> GenerateFilterPredicate(string propertyName, string filterValue, Type objType, Type propType, FilterOperationItem filterItem)
        {
            ParameterExpression objParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "x");
            UnaryExpression param = System.Linq.Expressions.Expression.TypeAs(objParam, objType);
            var prop = System.Linq.Expressions.Expression.Property(param, propertyName);
            var val = System.Linq.Expressions.Expression.Constant(filterValue);

            switch (filterItem.FilterOption)
            {
                case Enums.FilterOperation.Contains:
                    return ExpressionHelper.GenerateGeneric(prop, val, propType, objParam, "Contains");
                case Enums.FilterOperation.EndsWith:
                    return ExpressionHelper.GenerateGeneric(prop, val, propType, objParam, "EndsWith");
                case Enums.FilterOperation.StartsWith:
                    return ExpressionHelper.GenerateGeneric(prop, val, propType, objParam, "StartsWith");
                case Enums.FilterOperation.Equals:
                    return ExpressionHelper.GenerateEquals(prop, filterValue, propType, objParam);
                case Enums.FilterOperation.GreaterThanEqual:
                    return ExpressionHelper.GenerateGreaterThanEqual(prop, filterValue, propType, objParam);
                case Enums.FilterOperation.LessThanEqual:
                    return ExpressionHelper.GenerateLessThanEqual(prop, filterValue, propType, objParam);
                case Enums.FilterOperation.GreaterThan:
                    return ExpressionHelper.GenerateGreaterThan(prop, filterValue, propType, objParam);
                case Enums.FilterOperation.LessThan:
                    return ExpressionHelper.GenerateLessThan(prop, filterValue, propType, objParam);
                default:
                    throw new ArgumentException("Could not decode Search Mode.  Did you add a new value to the enum, or send in Unknown?");
            }

        }

        /// <summary>
        /// ResetFilterValues is called when the column is bound ColumnFilterHeader
        /// </summary>
        /// <param name="filterColumnInfo">Information about the column binding object</param>
        internal void ResetFilterValues(FilterColumnInfo filterColumnInfo)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            if (Column != null)
            {
                foreach (var i in DistinctPropertyValues.Where(i => i.IsChecked))
                    i.IsChecked = false;
                DistinctPropertyValues.Clear();
                FilterText = string.Empty;
                _boundColumnPropertyAccessor = null;
                FilterColumnInfo = filterColumnInfo;

                if (FilterColumnInfo.PropertyPath.Length > 0)
                {
                    if (FilterColumnInfo.PropertyPath.Contains('.'))
                        throw new ArgumentException(string.Format("This version of the grid does not support a nested property path such as '{0}'.  Please make a first-level property for filtering and bind to that.", FilterColumnInfo.PropertyPath));

                    this.Visibility = System.Windows.Visibility.Visible;
                    ParameterExpression arg = System.Linq.Expressions.Expression.Parameter(typeof(object), "x");
                    System.Linq.Expressions.Expression expr = System.Linq.Expressions.Expression.Convert(arg, Grid.FilterType);
                    expr = System.Linq.Expressions.Expression.Property(expr, Grid.FilterType, FilterColumnInfo.PropertyPath);
                    System.Linq.Expressions.Expression conversion = System.Linq.Expressions.Expression.Convert(expr, typeof(object));
                    _boundColumnPropertyAccessor = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(conversion, arg).Compile();
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                }
                ConfigureFilterOptions();
            }
        }
    }
}
