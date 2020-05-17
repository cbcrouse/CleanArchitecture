using System;

namespace Common.DataAnnotations
{
	/// <summary>
	/// An attribute that indicates the property should be skipped during recursive data annotation validation.
	/// </summary>
	public class SkipRecursiveValidation : Attribute { }
}
