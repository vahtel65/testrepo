using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class Product
    {
        private static Guid versionPropertyID = new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03");
        private static Guid mainVersionPropertyID = new Guid("BBE170B0-28E4-4738-B365-1038B03F4552");
        private static Guid orderNumberPropertyID = new Guid("9A38E338-DD60-4636-BFE3-6A98BAF8AE87");
        private static Guid orderYearPropertyID = new Guid("2CCD4FF3-6D43-4A35-9784-969FAB46B5CC");

        partial void OnLoaded()
        {
            this.PublicName = this._dictNomen == null ? "" : this._dictNomen.superpole;

            ProductProperty prop;
            prop = this.ProductProperties.FirstOrDefault(p => p.PropertyID == versionPropertyID);
            Version = prop == null ? string.Empty : prop.Value;

            prop = this.ProductProperties.FirstOrDefault(p => p.PropertyID == mainVersionPropertyID);
            MainVersion = prop == null ? string.Empty : prop.Value;

            prop = this.ProductProperties.FirstOrDefault(p => p.PropertyID == orderNumberPropertyID);
            OrderNumber = prop == null ? string.Empty : prop.Value;

            prop = this.ProductProperties.FirstOrDefault(p => p.PropertyID == orderYearPropertyID);
            OrderYear = prop == null ? string.Empty : prop.Value;
        }
        public string Version { get; set; }

        public string MainVersion { get; set; }
        public string OrderNumber { get; set; }
        public string OrderYear { get; set; }

        public string PublicName { get; set; }
    }
}
