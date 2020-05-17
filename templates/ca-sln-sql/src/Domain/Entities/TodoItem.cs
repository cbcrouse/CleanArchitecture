using System;

namespace Domain.Entities
{
	/// <summary>
	/// Represents a simple todo item.
	/// </summary>
	public class TodoItem
	{
		/// <summary>
		/// Gets or sets the unique identifier for the task.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the description of the task.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the point in time when the task was created.
		/// </summary>
		public DateTimeOffset CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the point in time when the task should be completed by.
		/// </summary>
		public DateTimeOffset? DueOn { get; set; }

		/// <summary>
		/// Gets or sets the point in time when the task was completed.
		/// </summary>
		public DateTimeOffset? CompletedOn { get; set; }
	}
}
