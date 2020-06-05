using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.AutoFixture
{
	/// <summary>
	/// A factory for generating instances of <see cref="IFixture"/>.
	/// </summary>
	public static class AutoFixtureFactory
	{
		/// <summary>
		/// Retrieves the factory that creates the customized <see cref="IFixture"/>.
		/// </summary>
		/// <param name="targetAssemblyTypes">The types from assemblies to load.</param>
		/// <param name="assemblyStartsWith">
		/// Only loads assemblies that starts with this string.
		/// Assemblies that end with "Tests" are always loaded.
		/// </param>
		public static Func<IFixture> Get(HashSet<Type> targetAssemblyTypes = null, string assemblyStartsWith = "AgSurfer")
		{
			IFixture Factory()
			{
				var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies())
				{
					Assembly.GetEntryAssembly()
				};

				if (targetAssemblyTypes != null)
				{
					foreach (Assembly assembly in targetAssemblyTypes.Select(x => x.Assembly))
					{
						assemblies.Add(assembly);
					}
				}

				var uniqueAssemblies = new HashSet<Assembly>(assemblies);
				return GetInstance(uniqueAssemblies, assemblyStartsWith);
			}

			return Factory;
		}

		/// <summary>
		/// Retrieves an instance of <see cref="IFixture"/>.
		/// </summary>
		/// <param name="targetAssemblies">The assemblies to load types from.</param>
		/// <param name="assemblyStartsWith">
		/// Only loads assemblies that starts with this string.
		/// Assemblies that contain "Tests" are always loaded.
		/// </param>
		public static IFixture GetInstance(HashSet<Assembly> targetAssemblies, string assemblyStartsWith = "")
		{
			IFixture fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

			var filteredAssemblies = new HashSet<Assembly>(targetAssemblies);

			foreach (Assembly targetAssembly in targetAssemblies)
			{
				var refAssemblies = targetAssembly
					.GetReferencedAssemblies()
					.Where(x =>
						x.FullName.StartsWith(assemblyStartsWith) ||
						x.FullName.Contains("Tests")).ToList()
					.Select(Assembly.Load).ToList();

				foreach (Assembly assemblyName in refAssemblies)
				{
					filteredAssemblies.Add(assemblyName);
				}
			}

			IEnumerable<Type> types = filteredAssemblies.Where(x =>
			{
				try
				{
					// Attempting to assign ExportedTypes will validate whether the assembly will load.
					var exported = x.ExportedTypes;
					return true;
				}
				catch
				{
					return false;
				}
			}).SelectMany(x => x.ExportedTypes).ToList();

			IEnumerable<Type> customizations = FilterTypesByInterfaceType(types, typeof(ICustomization));

			foreach (Type customization in customizations)
			{
				fixture.Customize((ICustomization)Activator.CreateInstance(customization));
			}

			IEnumerable<Type> specimenBuilders = FilterTypesByInterfaceType(types, typeof(ISpecimenBuilder));

			foreach (Type specimenBuilder in specimenBuilders)
			{
				fixture.Customizations.Add((ISpecimenBuilder)Activator.CreateInstance(specimenBuilder));
			}

			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			fixture.Customize(new AutoMoqCustomization());

			return fixture;
		}

		private static IEnumerable<Type> FilterTypesByInterfaceType(IEnumerable<Type> types, Type interfaceType)
		{
			return types.Where(x => interfaceType.IsAssignableFrom(x) && x.IsValid());
		}

		private static bool IsValid(this Type type)
		{
			return !type.ContainsGenericParameters && type.HasEmptyConstructor() && type.IsClass && !type.IsAbstract;
		}
	}
}
