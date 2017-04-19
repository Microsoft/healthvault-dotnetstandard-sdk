// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace HealthVault.Client.Models
{
    using HealthVault.Client;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// An objective is a high-level concept of what a user wants to accomplish
    /// as part of their enrollment in various action plans
    /// A user may have more than one objective
    /// A plan may contribute to more than one objective
    /// Each objective can have one measurable outcome
    /// </summary>
    public partial class Objective
    {
        /// <summary>
        /// Initializes a new instance of the Objective class.
        /// </summary>
        public Objective()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Objective class.
        /// </summary>
        /// <param name="id">The unique identifier of the instance of the
        /// objective</param>
        /// <param name="name">The name of the objective</param>
        /// <param name="description">The description of the objective</param>
        /// <param name="state">The state of the objective. Possible values
        /// include: 'Unknown', 'Inactive', 'Active'</param>
        /// <param name="outcomeName">Gets or sets the name of the
        /// outcome</param>
        /// <param name="outcomeType">Gets or sets the type of the outcome.
        /// Possible values include: 'Unknown', 'StepsPerDay',
        /// 'CaloriesPerDay', 'ExerciseHoursPerWeek', 'SleepHoursPerNight',
        /// 'MinutesToFallAsleepPerNight', 'Other'</param>
        public Objective(string id = default(string), string name = default(string), string description = default(string), string state = default(string), string outcomeName = default(string), string outcomeType = default(string))
        {
            Id = id;
            Name = name;
            Description = description;
            State = state;
            OutcomeName = outcomeName;
            OutcomeType = outcomeType;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the unique identifier of the instance of the objective
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the objective
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the objective
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the state of the objective. Possible values include:
        /// 'Unknown', 'Inactive', 'Active'
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the name of the outcome
        /// </summary>
        [JsonProperty(PropertyName = "outcomeName")]
        public string OutcomeName { get; set; }

        /// <summary>
        /// Gets or sets the type of the outcome. Possible values include:
        /// 'Unknown', 'StepsPerDay', 'CaloriesPerDay', 'ExerciseHoursPerWeek',
        /// 'SleepHoursPerNight', 'MinutesToFallAsleepPerNight', 'Other'
        /// </summary>
        [JsonProperty(PropertyName = "outcomeType")]
        public string OutcomeType { get; set; }

    }
}
