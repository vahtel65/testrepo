using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.SettingDomain
{
    public class SettingProvider
    {
        public string GetSetting(Guid userID, SettingEnum setting)
        {
            Guid setID  = Setting.GetSettingID(setting);
            using (CommonDomain domain = new CommonDomain())
            {
                UserSetting us = domain.UserSettings.Where(u => u.UserID == userID && u.SettingID == setID).SingleOrDefault();
                if (us == null) return string.Empty;
                else return us.Value;
            }
        }
        public void SaveSetting(Guid userID, SettingEnum setting, string value)
        {
            Guid setID = Setting.GetSettingID(setting);
            using (CommonDomain domain = new CommonDomain())
            {
                UserSetting us = domain.UserSettings.Where(u => u.UserID == userID && u.SettingID == setID).SingleOrDefault();
                if (us == null)
                {
                    us = new UserSetting();
                    us.ID = Guid.NewGuid();
                    us.UserID = userID;
                    us.SettingID = setID;
                    us.Value = value;
                    domain.UserSettings.InsertOnSubmit(us);
                    domain.SubmitChanges();
                }
                else
                {
                    us.Value = value;
                    domain.SubmitChanges();
                }
            }
        }
    }
}
