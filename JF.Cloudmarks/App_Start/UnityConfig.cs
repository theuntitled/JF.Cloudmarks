using System;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using JF.AspNet.Identity.DocumentDB;
using JF.Cloudmarks.Modules.Db;
using JF.Cloudmarks.Modules.Identity;
using JF.Cloudmarks.Modules.Identity.MessageServices;
using JF.Cloudmarks.Modules.Parser;
using Microsoft.Azure.Documents.Client;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;

namespace JF.Cloudmarks {

	/// <summary>
	///     Specifies the Unity configuration for the main container.
	/// </summary>
	public class UnityConfig {

		/// <summary>Registers the type mappings with the Unity container.</summary>
		/// <param name="container">The unity container to configure.</param>
		/// <remarks>
		///     There is no need to register concrete types such as controllers or API controllers (unless you want to
		///     change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
		/// </remarks>
		public static void RegisterTypes( IUnityContainer container ) {
			var documentClient = new DocumentClient( new Uri( ConfigurationManager.AppSettings.Get( "DocumentDB:Uri" ) ) ,
													 ConfigurationManager.AppSettings.Get( "DocumentDB:Key" ) );

			var dataCollectionConstructor = new InjectionConstructor( documentClient ,
																	  ConfigurationManager.AppSettings.Get( "DocumentDB:Database" ) ,
																	  true );

			// DocumentDB
			container.RegisterType<IDocumentManager , DocumentManager>( new PerRequestLifetimeManager() ,
																		dataCollectionConstructor );

			container.RegisterType( typeof (IIdentityCollectionManager<>) ,
									typeof (DocumentManager) ,
									new PerRequestLifetimeManager() ,
									dataCollectionConstructor );

			// Username and message service types
			container.RegisterType<SmtpClient>( new InjectionConstructor() );
			container.RegisterType<IEmailService , EmailService>();

			// Http context types
			container.RegisterType<HttpContextBase>( new InjectionFactory( _ => new HttpContextWrapper( HttpContext.Current ) ) );
			container.RegisterType<IOwinContext>( new InjectionFactory( c => c.Resolve<HttpContextBase>().GetOwinContext() ) );
			container.RegisterType<IAuthenticationManager>( new InjectionFactory( c => c.Resolve<IOwinContext>().Authentication ) );

			// Identity types
			container.RegisterType( typeof (ApplicationUser) );
			container.RegisterType<IUserValidator , UserValidator>();
			container.RegisterType<IIdentityRoleStore , ApplicationRoleStore>();
			container.RegisterType<IUserManager , UserManager>( new PerRequestLifetimeManager() );
			container.RegisterType<ISignInManager , SignInManager>( new PerRequestLifetimeManager() );
			container.RegisterType( typeof (IDocumentDBUserStore<>) , typeof (DocumentDBUserStore<>) );

			// Other types
			container.RegisterType<INetscapeBookmarkParser , NetscapeBookmarkParser>();
		}

		#region Unity Container

		private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>( () => {
			var container = new UnityContainer();
			RegisterTypes( container );
			return container;
		} );

		/// <summary>
		///     Gets the configured Unity container.
		/// </summary>
		public static IUnityContainer GetConfiguredContainer() {
			return Container.Value;
		}

		#endregion
	}

}
