using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace Aspect.Domain
{
    public enum SettingEnum
    {
        MainGridAreaSize,
        Nothing
    }

    public partial class Setting
    {
        public static Guid MainGridAreaSize = new Guid("75138d4d-6e99-482f-92ec-a1a0bc05fc94");

        partial void OnLoaded()
        {
            if (this.ID == MainGridAreaSize) Value = SettingEnum.MainGridAreaSize;
            else Value = SettingEnum.Nothing;
        }

        public static Guid GetSettingID(SettingEnum value)
        {
            switch (value)
            {
                case SettingEnum.MainGridAreaSize:
                    return MainGridAreaSize;
                default:
                    return Guid.Empty;
            }
        }

        public SettingEnum Value { get; set; }
    }
}
