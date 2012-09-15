using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public enum FormGridViewEnum
    {
        ConfigurationView,
        ConfigurationTree
    }

    public partial class FormGridView
    {
        public static Guid ConfigurationView = new Guid("FB4D4AB8-A746-4186-82B8-C2AC77AC7E52");
        public static Guid ConfigurationTree = new Guid("D4249A1E-05AF-45DE-96D9-CD715D0466BD");

        partial void OnLoaded()
        {
            if (this.ID == ConfigurationView)
            {
                Value = FormGridViewEnum.ConfigurationView;
            }
            else if (this.ID == ConfigurationTree)
            {
                Value = FormGridViewEnum.ConfigurationTree;
            }
        }

        public static Guid GetFieldPlaceHolderID(FormGridViewEnum value)
        {
            switch (value)
            {
                case FormGridViewEnum.ConfigurationView:
                    return ConfigurationView;
                case FormGridViewEnum.ConfigurationTree:
                    return ConfigurationTree;
            }
            return Guid.Empty;
        }

        public FormGridViewEnum Value { get; set; }
    }
}
