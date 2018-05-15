// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer4Demo.Identity.DbContextFactory
{
    public class IdentityServerDemoDbContextFactory : IDesignTimeDbContextFactory<IdentityServerDemoDbContext>
    {
        public IdentityServerDemoDbContext CreateDbContext(string[] args)
        {
            var builderOption = new DbContextOptionsBuilder<IdentityServerDemoDbContext>();
            builderOption.UseSqlServer(@"data source=WANGPENG;User Id=sa;Pwd=sa123;initial catalog=IdentityServer4Demo;integrated security=True;MultipleActiveResultSets=True;");
            return new IdentityServerDemoDbContext(builderOption.Options);
        }
    }
}
