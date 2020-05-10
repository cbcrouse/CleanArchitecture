namespace Presentation.API.Configuration
{
	/// <summary>
	/// Provides configuration settings for Swagger.
	/// </summary>
	public class SwaggerOptions
	{
		/// <summary>
		/// Gets or sets whether Swagger is enabled.
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the description that appears in the document selector drop-down.
		/// </summary>
		public string DropdownDescription { get; set; } = "Default Description";

		/// <summary>
		/// Gets or sets the Swagger page title.
		/// </summary>
		public string Title { get; set; } = "API Title";

		/// <summary>
		/// Gets or sets whether to include XML comments from the controllers.
		/// </summary>
		public bool IncludeControllerXmlComments { get; set; }
	}
}
