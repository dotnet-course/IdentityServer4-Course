// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4Demo.Identity
{
    public class IdentityServerDemoDbContext: IdentityDbContext<IdentityServerDemoIdentityUser>
    {
        public IdentityServerDemoDbContext(DbContextOptions<IdentityServerDemoDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
