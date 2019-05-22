using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewTechParentPortalV3.Models;

[assembly: HostingStartup(typeof(NewTechParentPortalV3.Areas.Identity.IdentityHostingStartup))]
namespace NewTechParentPortalV3.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<NewTechParentPortalV31Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("NewTechParentPortalV3ContextConnection")));

                services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<NewTechParentPortalV31Context>();

                services.AddMvc(config =>
                {
                    // using Microsoft.AspNetCore.Mvc.Authorization;
                    // using Microsoft.AspNetCore.Authorization;
                    var policy = new AuthorizationPolicyBuilder()
                                     .RequireAuthenticatedUser()
                                     .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            });
        }
    }
}