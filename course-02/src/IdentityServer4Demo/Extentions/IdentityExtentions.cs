// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityServer4Demo.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Demo.Extentions
{
    public static class IdentityExtentions
    {
        public static void AddIdentityServerDemoIdentity(this IServiceCollection services)
        {
            //可设置密码规则
            services.AddIdentity<IdentityServerDemoIdentityUser, IdentityRole>(options =>
            {
                //密码不需要数字
                options.Password.RequireDigit = false;

                //密码不区分小写
                options.Password.RequireLowercase = false;
                //密码不区分大写
                options.Password.RequireUppercase = false;

                //密码不需要字母数字
                options.Password.RequireNonAlphanumeric = false;
                
                //密码长度至少6位
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<IdentityServerDemoDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
