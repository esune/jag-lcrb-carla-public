// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collection of adoxio_importedoutstandinginvoices
    /// </summary>
    /// <remarks>
    /// Microsoft.Dynamics.CRM.adoxio_importedoutstandinginvoicesCollection
    /// </remarks>
    public partial class MicrosoftDynamicsCRMadoxioImportedoutstandinginvoicesCollection
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioImportedoutstandinginvoicesCollection
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioImportedoutstandinginvoicesCollection()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioImportedoutstandinginvoicesCollection
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioImportedoutstandinginvoicesCollection(IList<MicrosoftDynamicsCRMadoxioImportedoutstandinginvoices> value = default(IList<MicrosoftDynamicsCRMadoxioImportedoutstandinginvoices>))
        {
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<MicrosoftDynamicsCRMadoxioImportedoutstandinginvoices> Value { get; set; }

    }
}
