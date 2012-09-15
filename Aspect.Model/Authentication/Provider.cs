using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.Authentication
{
    public class Provider : CommonDomain
    {
        public User GeneralAuthentication(string login, string password)
        {
            return this.Users.FirstOrDefault(u => u.Name == login && u.Password == password);
        }
        public User SelectFromListAuthentication(Guid id, string password)
        {
            return this.Users.FirstOrDefault(u => u.ID == id && u.Password == password);
        }

        public User GetUser(Guid id)
        {
            return this.Users.Single(u => u.ID == id);
        }

        public List<Guid> GetUserRoles(Guid userID)
        {
            return UserRoles.Where(ur => ur.UserID == userID).Select(u => u.Role.ID).ToList();
        }
    }
}
