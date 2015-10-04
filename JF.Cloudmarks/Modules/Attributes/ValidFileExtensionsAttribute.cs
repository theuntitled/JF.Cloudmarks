using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace JF.Cloudmarks.Modules.Attributes {

	/// <summary>
	///     Valides file extensions for a HttpPostedFileBase property
	/// </summary>
	[AttributeUsage( AttributeTargets.Property )]
	public class ValidFileExtensionsAttribute : DataTypeAttribute , IClientValidatable {

		private readonly FileExtensionsAttribute _innerAttribute = new FileExtensionsAttribute();

		/// <summary />
		public ValidFileExtensionsAttribute() : base( DataType.Upload ) {
			ErrorMessage = _innerAttribute.ErrorMessage;
		}

		/// <summary>
		///     A list of valid extensions
		/// </summary>
		public string Extensions {
			get { return _innerAttribute.Extensions; }
			set { _innerAttribute.Extensions = value; }
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
			ModelMetadata metadata ,
			ControllerContext context ) {
			var rule = new ModelClientValidationRule {
				ValidationType = "accept" ,
				ErrorMessage = ErrorMessage
			};

			rule.ValidationParameters["exts"] = _innerAttribute.Extensions;

			yield return rule;
		}

		public override string FormatErrorMessage( string name ) {
			return _innerAttribute.FormatErrorMessage( name );
		}

		public override bool IsValid( object value ) {
			var file = value as HttpPostedFileBase;

			return _innerAttribute.IsValid( file != null ? file.FileName : value );
		}

	}

}
