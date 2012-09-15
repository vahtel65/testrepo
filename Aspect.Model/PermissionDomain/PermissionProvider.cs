using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

using Aspect.Domain;

namespace Aspect.Model.PermissionDomain
{
    public class PermissionProvider
    {
        public CommonDomain DBDomain { get; set; }
        public PermissionProvider(CommonDomain domain)
        {
            DBDomain = domain;
        }
        /// <summary>
        /// Example: GetDeniedNodesRead(userRoles, PermissionEntity.ClassificationTree)
        /// Example: GetDeniedNodesRead(userRoles, PermissionEntity.CustomClassificationTree)
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="permissionEntityID"></param>
        /// <returns></returns>
        protected List<Guid> GetDeniedNodesRead(List<Guid> roles, Guid permissionEntityID)
        {
            var d = from uv in DBDomain.RoleViewPermissions
                    where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Read
                    && roles.Contains(uv.RoleID)
                    select uv.EntityID;
            return d.ToList();
        }
        protected List<Guid> GetDeniedNodesModify(List<Guid> roles, Guid permissionEntityID)
        {
            var d = from uv in DBDomain.RoleViewPermissions
                    where uv.PermissionEntityID == permissionEntityID && !uv.Modify
                    && roles.Contains(uv.RoleID)
                    select uv.EntityID;
            return d.ToList();
        }
        protected List<Guid> GetDeniedNodesDelete(List<Guid> roles, Guid permissionEntityID)
        {
            var d = from uv in DBDomain.RoleViewPermissions
                    where uv.PermissionEntityID == permissionEntityID && !uv.Delete
                    && roles.Contains(uv.RoleID)
                    select uv.EntityID;
            return d.ToList();
        }
    }
}
