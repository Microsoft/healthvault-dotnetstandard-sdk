// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace HealthVault.Client.Models
{
    using HealthVault.Client;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The tracking properties of a HealthVault event
    /// </summary>
    public partial class ActionPlanTaskTargetEvent
    {
        /// <summary>
        /// Initializes a new instance of the ActionPlanTaskTargetEvent class.
        /// </summary>
        public ActionPlanTaskTargetEvent()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ActionPlanTaskTargetEvent class.
        /// </summary>
        /// <param name="elementXPath">XML XPath to the property for which task
        /// should be tracked</param>
        /// <param name="elementValues">List of all of the possible values for
        /// a given xPath property. (i.e. "run", "running", "walk"
        /// etc).</param>
        /// <param name="isNegated">Flag to indicate if the condition mentioned
        /// above is negated. e.g. Item shouldn't be a run or a walk.</param>
        public ActionPlanTaskTargetEvent(string elementXPath = default(string), IList<string> elementValues = default(IList<string>), bool? isNegated = default(bool?))
        {
            ElementXPath = elementXPath;
            ElementValues = elementValues;
            IsNegated = isNegated;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets XML XPath to the property for which task should be
        /// tracked
        /// </summary>
        [JsonProperty(PropertyName = "elementXPath")]
        public string ElementXPath { get; set; }

        /// <summary>
        /// Gets or sets list of all of the possible values for a given xPath
        /// property. (i.e. "run", "running", "walk" etc).
        /// </summary>
        [JsonProperty(PropertyName = "elementValues")]
        public IList<string> ElementValues { get; set; }

        /// <summary>
        /// Gets or sets flag to indicate if the condition mentioned above is
        /// negated. e.g. Item shouldn't be a run or a walk.
        /// </summary>
        [JsonProperty(PropertyName = "isNegated")]
        public bool? IsNegated { get; set; }

    }
}
