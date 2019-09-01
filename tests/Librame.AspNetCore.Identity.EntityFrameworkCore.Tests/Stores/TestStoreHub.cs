﻿using System.Collections.Generic;
using System.Linq;

namespace Librame.AspNetCore.Identity.Tests
{
    using Extensions.Data;

    public class TestStoreHub : StoreHubBase<IdentityDbContextAccessor>
    {
        public TestStoreHub(IAccessor accessor, IStoreInitializer<IdentityDbContextAccessor> initializer)
            : base(accessor, initializer)
        {
        }


        public IList<DefaultIdentityRole> GetRoles()
        {
            return Accessor.Roles.ToList();
        }

        public IPageable<DefaultIdentityUser> GetUsers()
        {
            return Accessor.Users.AsPagingByIndex(ordered => ordered.OrderBy(k => k.Id), 1, 10);
        }


        public TestStoreHub UseDefaultDbConnection()
        {
            Accessor.ToggleTenant(t => t.DefaultConnectionString);
            return this;
        }

        public TestStoreHub UseWriteDbConnection()
        {
            Accessor.ToggleTenant(t => t.WritingConnectionString);
            return this;
        }
    }
}
