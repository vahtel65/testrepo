using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aspect.Domain;

namespace Aspect.Model
{
    public enum ClassifiacationTypeView
    {
        Standard,
        Custom,
        Dictionary,
        //--
        ConfigurationView,
        ConfigurationTree,
        //--
        NotDefined
    }
    public class Common
    {
        public const string IDColumnTitle = "ID";
        public const string LevelColumnTitle = "Level";

        public static ClassifiacationTypeView GetClassifiacationTypeView(Guid id)
        {
            if (id == FormGridView.ConfigurationTree)
            {
                return ClassifiacationTypeView.ConfigurationTree;
            }
            else if (id == FormGridView.ConfigurationView)
            {
                return ClassifiacationTypeView.ConfigurationView;
            }

            using (CommonDomain domain = new CommonDomain())
            {
                if (domain.ClassificationTrees.Where(c => c.ID == id).Count() > 0)
                {
                    return ClassifiacationTypeView.Standard;
                }
                else if(domain.DictionaryTrees.Where(c => c.ID == id).Count() > 0)
                {
                    return ClassifiacationTypeView.Dictionary;
                }
                else if (domain.CustomClassificationTrees.Where(c => c.ID == id).Count() > 0)
                {
                    return ClassifiacationTypeView.Custom;
                }
            }
            return ClassifiacationTypeView.NotDefined;
        }

        public static ContentDomain GetContentDomain(ClassifiacationTypeView type)
        {
            switch (type)
            {
                case ClassifiacationTypeView.Standard:
                    return new Aspect.Model.ProductDomain.ProductProvider(); 
                case ClassifiacationTypeView.Dictionary:
                    return new Aspect.Model.DictionaryDomain.DictionaryProvider();
                case ClassifiacationTypeView.Custom:
                    return new Aspect.Model.ProductDomain.CustomClassificationProductProvider();
                case ClassifiacationTypeView.ConfigurationView:
                    return new Aspect.Model.ConfigurationDomain.ConfigurationProvider();
                case ClassifiacationTypeView.ConfigurationTree:
                    return new Aspect.Model.ConfigurationDomain.ConfigurationTreeProvider();
            }
            return null;
        }
    }
}
