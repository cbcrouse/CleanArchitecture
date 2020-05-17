using Common.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Common.DataAnnotations
{
	/// <summary>
	/// A custom data annotations validator capable of recursively traversing an object.
	/// </summary>
	public class DataAnnotationsValidator
	{
		/// <summary>
		/// Validate an object using data annotations
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="results"></param>
		/// <param name="validationContextItems"></param>
		public bool TryValidateObject(object obj, ICollection<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
		{
			return Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems), results, true);
		}

		/// <summary>
		/// Recursively validate an object using data annotations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="results">A collection of validation results.</param>
		/// <param name="validationContextItems">A collection of validation context items.</param>
		public bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
		{
			return TryValidateObjectRecursive(obj, results, new HashSet<object>(), validationContextItems);
		}

		private bool TryValidateObjectRecursive<T>(T obj, ICollection<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems = null)
		{
			// Short-circuit to avoid infinite loops on cyclical object graphs
			if (validatedObjects.Contains(obj))
			{
				return true;
			}

			validatedObjects.Add(obj);
			bool result = TryValidateObject(obj, results, validationContextItems);

			var properties = obj.GetType().GetProperties()
				.Where(prop => prop.CanRead
							   && !prop.GetCustomAttributes(typeof(SkipRecursiveValidation), false).Any()
							   && prop.GetIndexParameters().Length == 0).ToList();

			foreach (PropertyInfo property in properties)
			{
				if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

				object value = obj.GetPropertyValue(property.Name);

				List<ValidationResult> nestedResults;
				switch (value)
				{
					case null:
						continue;
					case IEnumerable asEnumerable:
						foreach (object enumObj in asEnumerable)
						{
							if (enumObj == null)
								continue;

							nestedResults = new List<ValidationResult>();
							if (!TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems))
							{
								result = false;
								foreach (ValidationResult validationResult in nestedResults)
								{
									PropertyInfo property1 = property;
									results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
								}
							}
						}

						break;
					default:
						nestedResults = new List<ValidationResult>();
						if (!TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems))
						{
							result = false;
							foreach (ValidationResult validationResult in nestedResults)
							{
								PropertyInfo property1 = property;
								results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
							}
						}
						break;
				}
			}

			return result;
		}
	}
}
