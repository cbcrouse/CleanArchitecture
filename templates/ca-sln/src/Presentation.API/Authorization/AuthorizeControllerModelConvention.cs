using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Presentation.API.Authorization
{
	/// <summary>
	/// Applies authorization to controllers within the API.
	/// </summary>
	public class AuthorizeControllerModelConvention : IControllerModelConvention
	{
		/// <summary>
		/// Called to apply the convention to the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.
		/// </summary>
		/// <param name="controller">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.</param>
		public void Apply(ControllerModel controller)
		{
			var defaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			controller.Filters.Add(new AuthorizeFilter(defaultPolicy));
		}
	}
}
