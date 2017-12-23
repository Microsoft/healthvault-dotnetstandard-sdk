// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.HealthVault.RestApi.Generated.Models
{
    using Microsoft.HealthVault;
    using Microsoft.HealthVault.RestApi;
    using Microsoft.HealthVault.RestApi.Generated;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An action plan for adherence reporting purposes
    /// </summary>
    public partial class ActionPlanAdherenceSummary
    {
        /// <summary>
        /// Initializes a new instance of the ActionPlanAdherenceSummary class.
        /// </summary>
        public ActionPlanAdherenceSummary()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ActionPlanAdherenceSummary class.
        /// </summary>
        /// <param name="id">The ID of the plan</param>
        /// <param name="name">The name of the plan</param>
        /// <param name="startDate">The starting date of the adherence
        /// summary.</param>
        /// <param name="endDate">The ending date of the adherence
        /// summary.</param>
        /// <param name="objectives">The Collection of objectives for the
        /// plan</param>
        public ActionPlanAdherenceSummary(System.Guid? id = default(System.Guid?), string name = default(string), System.DateTime? startDate = default(System.DateTime?), System.DateTime? endDate = default(System.DateTime?), IList<ObjectiveAdherenceSummary> objectives = default(IList<ObjectiveAdherenceSummary>))
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Objectives = objectives;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the ID of the plan
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the plan
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the adherence summary.
        /// </summary>
        [JsonProperty(PropertyName = "startDate")]
        public System.DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the adherence summary.
        /// </summary>
        [JsonProperty(PropertyName = "endDate")]
        public System.DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Collection of objectives for the plan
        /// </summary>
        [JsonProperty(PropertyName = "objectives")]
        public IList<ObjectiveAdherenceSummary> Objectives { get; set; }

    }
}
