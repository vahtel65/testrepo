using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.UserDomain
{
    public class UserProvider : CommonDomain
    {
        public List<User> GetUsers()
        {
            return this.Users.ToList();
        }

        public List<UserMenuItem> GetFavorites(Guid userID)
        {
            var q = from m in UserMenuItems
                    where m.UserID == userID && m.UserMenuTypeID == UserMenuType.Favorites
                    orderby m.CreatedDate descending
                    select m;
            return q.ToList();
        }

        public void AddToFavorites(Guid viewID, Guid userID, string name, string url)
        {
            List<UserMenuItem> list = this.GetFavorites(userID).OrderBy(v => v.CreatedDate).ToList();
            if (list.Where(v => v.ViewID == viewID).Count() == 0)
            {
                UserMenuItem item = new UserMenuItem()
                {
                    ID = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Name = name,
                    UserID = userID,
                    UserMenuTypeID = UserMenuType.Favorites,
                    ViewID = viewID,
                    Url = url
                };
                UserMenuItems.InsertOnSubmit(item);
                this.SubmitChanges();
            }
        }

        public List<UserMenuItem> GetLastViewed(Guid userID)
        {
            var q = from m in UserMenuItems
                    where m.UserID == userID && m.UserMenuTypeID == UserMenuType.LastViewed
                    orderby m.CreatedDate descending
                    select m;
            return q.ToList();
        }
        private void DeleteFromLastViewed(Guid id)
        {
            this.UserMenuItems.DeleteOnSubmit(UserMenuItems.Single(s => s.ID == id));
            this.SubmitChanges();
        }
        public void AddToLastViewed(Guid viewID, Guid userID, string name, string url)
        {
            List<UserMenuItem> viewed = this.GetLastViewed(userID).OrderBy(v => v.CreatedDate).ToList();
            IList<UserMenuItem> exists = viewed.Where(v => v.ViewID == viewID).ToList();
            if (exists.Count == 0)
            {
                if (viewed.Count >= 10)
                {
                    DeleteFromLastViewed(viewed[0].ID);
                }
                UserMenuItem item = new UserMenuItem()
                {
                    ID = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Name = name,
                    UserID = userID,
                    UserMenuTypeID = UserMenuType.LastViewed,
                    ViewID = viewID,
                    Url = url
                };
                UserMenuItems.InsertOnSubmit(item);
                this.SubmitChanges();
            }
            else
            {
                foreach (UserMenuItem item in exists)
                {
                    item.CreatedDate = DateTime.Now;
                }
                this.SubmitChanges();
            }
        }

        public List<UserMenuItem> GetPreDefined(Guid userID)
        {
            var q = from m in UserMenuItems
                    where m.UserID == userID && m.UserMenuTypeID == UserMenuType.PreDefined
                    select m;
            return q.ToList();
        }

        public bool IsAdministrator(List<Guid> roles)
        {            
            // Разработчики
            return roles.Exists(role => role == new Guid("6D7B72E0-E110-4DAB-9A6F-CC09EEB30120"));
        }
    }
}
