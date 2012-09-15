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
using System.Windows.Navigation;
using System.Collections.ObjectModel;

namespace EditorKmh
{
    public partial class GGridCustomizationPage : Page
    {
        public class People
        {
            public static ObservableCollection<Person> GetListOfPeople()
            {
                ObservableCollection<Person> ppl = new ObservableCollection<Person>();
                ppl.Add(new Person() { Firstname = "1", Lastname = "Цех М1" });
                ppl.Add(new Person() { Firstname = "2", Lastname = "Цех М2" });
                ppl.Add(new Person() { Firstname = "3", Lastname = "Сборочный цех" });
                ppl.Add(new Person() { Firstname = "4", Lastname = "Цех УТК" });
                return ppl;
            }        
        }

       

        public class Person
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string FullName
            {
                get
                {
                    return string.Concat(Lastname);
                }
            }
        }

        ObservableCollection<Person> toList = new ObservableCollection<Person>();        

        public GGridCustomizationPage()
        {
            InitializeComponent();

            MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
            };

            Loaded += new RoutedEventHandler(GGridCustomizationPage_Loaded);
        }

        void GGridCustomizationPage_Loaded(object sender, RoutedEventArgs e)
        {
            listBox1.ItemsSource = People.GetListOfPeople();
            listBox2.ItemsSource = this.toList;
        }

        public string GetGuildsList()
        {
            string res = "";
            foreach(Person person in toList)
            {
                if (res.Length == 0)
                {
                    res =  person.FullName;
                }
                else
                {
                    res += String.Format(" -> {0}", person.FullName);
                }
                
            }
            return res;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public delegate void CloseSelectorRouteHandle(bool applying);
        public event CloseSelectorRouteHandle OnCloseSelectorRoute;

        private void closeSelectorRoute(object sender, RoutedEventArgs e)
        {
            OnCloseSelectorRoute(false);
        }

        private void applySelectorRoute(object sender, RoutedEventArgs e)
        {
            OnCloseSelectorRoute(true);
        }
    }
}
