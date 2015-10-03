using System;
using System.Configuration;
using JF.Cloudmarks;
using JF.Cloudmarks.Modules.Identity;
using JF.Cloudmarks.Modules.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using Owin;

[assembly: OwinStartup( typeof (AuthConfig) )]

namespace JF.Cloudmarks {

	public class AuthConfig {

		static AuthConfig() {
			PublicClientId = "web";

			OAuthOptions = new OAuthAuthorizationServerOptions {
				AllowInsecureHttp = true ,
				TokenEndpointPath = new PathString( "/Token" ) ,
				AccessTokenExpireTimeSpan = TimeSpan.FromDays( 14 ) ,
				Provider = new ApplicationOAuthProvider( PublicClientId ) ,
				AuthorizeEndpointPath = new PathString( "/Account/Authorize" )
			};
		}

		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		public static string PublicClientId { get; }

		public string GetAppSetting( string key ) {
			return ConfigurationManager.AppSettings.Get( key );
		}

		public void Configuration( IAppBuilder app ) {
			UserManager.DefaultDataProtectionProvider = app.GetDataProtectionProvider();

			app.CreatePerOwinContext( () => UnityConfig.GetConfiguredContainer().Resolve<IUserManager>() as UserManager );
			app.CreatePerOwinContext( () => UnityConfig.GetConfiguredContainer().Resolve<ISignInManager>() as SignInManager );

			app.UseCookieAuthentication(
				new CookieAuthenticationOptions {
					AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie ,
					LoginPath = new PathString( "/Account/Login" ) ,
					Provider = new CookieAuthenticationProvider {
						OnValidateIdentity =
							SecurityStampValidator.OnValidateIdentity<UserManager , ApplicationUser>(
								TimeSpan.FromMinutes( 20 ) ,
								( manager , user ) => manager.GenerateUserIdentityAsync( user ) )
					}
				} );

			app.UseExternalSignInCookie( DefaultAuthenticationTypes.ExternalCookie );

			app.UseTwoFactorSignInCookie( DefaultAuthenticationTypes.TwoFactorCookie , TimeSpan.FromMinutes( 5 ) );

			app.UseTwoFactorRememberBrowserCookie( DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie );

			app.UseMicrosoftAccountAuthentication( GetAppSetting( "MicrosoftClientId" ) ,
												   GetAppSetting( "MicrosoftClientSecret" ) );

			app.UseTwitterAuthentication( GetAppSetting( "TwitterConsumerKey" ) , GetAppSetting( "TwitterConsumerSecret" ) );

			app.UseFacebookAuthentication( GetAppSetting( "FacebookAppId" ) , GetAppSetting( "FacebookAppSecret" ) );

			app.UseGoogleAuthentication( new GoogleOAuth2AuthenticationOptions {
				ClientId = GetAppSetting( "GoogleClientId" ) ,
				ClientSecret = GetAppSetting( "GoogleClientSecret" )
			} );
		}

	}

}
