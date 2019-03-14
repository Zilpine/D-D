using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeDCalculator.Data;
using DeDCalculator.Services;
using System.IO;
using DeDCalculator.Data.DAL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AspNet.Security.OpenIdConnect.Primitives;

namespace DeDCalculator
{
	public class Startup
	{
		private readonly IHostingEnvironment _hostingEnvironment;

		public Startup(IConfiguration configuration, IHostingEnvironment environment)
		{
			Configuration = configuration;
			_hostingEnvironment = environment;

		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<Context>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<Context>()
				.AddDefaultTokenProviders();

			services.AddMvc()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeFolder("/Account/Manage");
					options.Conventions.AuthorizePage("/Account/Logout");
				});
			services.AddDbContext<Context>(options =>
			{
				options.UseSqlite($"Data Source={Path.Combine(_hostingEnvironment.ContentRootPath, Configuration.GetConnectionString("DefaultConnection"))}", b => b.MigrationsAssembly("NaNaNa.Core"));
				options.UseOpenIddict<Guid>();
			});
			services.AddDbContext<Context>(ServiceLifetime.Scoped); // this is the important bit

			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireUppercase = true;
					options.Password.RequiredLength = 8;
					options.User.RequireUniqueEmail = true;
				})
				.AddEntityFrameworkStores<Context>()
				.AddDefaultTokenProviders()
				.AddUserStore<UserStore<ApplicationUser, ApplicationRole, Context, Guid>>()
				.AddRoleStore<RoleStore<ApplicationRole, Context, Guid>>();

			services.AddOpenIddict()
				.AddCore(options =>
				{
					options.UseEntityFrameworkCore()
						.UseDbContext<Context>()
						.ReplaceDefaultEntities<Guid>();
				})
				.AddServer(options =>
				{
					options.UseMvc();

					options
						.EnableTokenEndpoint("/connect/token")
						.EnableLogoutEndpoint("/connect/logout")
						.EnableRevocationEndpoint("/connect/revoke")
						.EnableUserinfoEndpoint("/api/mobile/User/Info");

					//options.AddEventHandler<OpenIddictServerEvents.MatchEndpoint>(notification => { // By default, only requests sent to /connect/token will be treated as valid
					//	// token requests by OpenIddict. This custom logic allows requests pointing
					//	// to /connect/second-token-endpoint to be treated the same way so that
					//	// requests can be sent to one of the two addresses without any distinction.
					//	var request = notification.Context.HttpContext.Request;
					//	if (request.Path == "/api/mobile/Account/ExternalLoginMobileConfirmation")
					//	{
					//		notification.Context.MatchTokenEndpoint();
					//	}
					//	return Task.FromResult(OpenIddictServerEventState.Handled);
					//});

					options
						.AllowPasswordFlow()
						.AllowRefreshTokenFlow()
						.AllowClientCredentialsFlow()
						.AllowCustomFlow("urn:ietf:params:oauth:grant-type:google_identity_token")
						.AllowCustomFlow("urn:ietf:params:oauth:grant-type:facebook_identity_token");

					options.RegisterClaims(
						OpenIdConnectConstants.Claims.Email,
						OpenIdConnectConstants.Claims.FamilyName,
						OpenIdConnectConstants.Claims.GivenName);

					options.RegisterScopes(
						OpenIdConnectConstants.Scopes.Profile,
						OpenIdConnectConstants.Scopes.OfflineAccess,
						OpenIdConnectConstants.Scopes.OpenId,
						OpenIdConnectConstants.Scopes.Email);

					options.EnableRequestCaching();

					options
						.SetAccessTokenLifetime(TimeSpan.FromSeconds(300))
						.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
#if DEBUG
					options.DisableHttpsRequirement();
					options.AcceptAnonymousClients();
#endif
				})
				.AddValidation();

		
			services.Configure<IdentityOptions>(options =>
			{
				options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
				options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
				options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;

			});

			// Register no-op EmailSender used by account confirmation and password reset during development
			// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
			services.AddSingleton<IEmailSender, EmailSender>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc();
		}
	}
}
