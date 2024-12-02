using System;

namespace HealthcareSystem.Model
{
    /// <summary>
    ///     Represents a lab test performed during a medical visit, including details about the test type, result, and
    ///     abnormalities.
    /// </summary>
    public class LabTest
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the unique identifier for the lab test.
        /// </summary>

        public int LabTestId { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the visit associated with this lab test.
        /// </summary>

        public int VisitId { get; set; }

        /// <summary>
        ///     Gets or sets the date and time when the lab test was performed.
        /// </summary>

        public DateTime TimePerformed { get; set; }

        /// <summary>
        ///     Gets or sets the name of the lab test type (e.g., "Blood Glucose", "White Blood Cell Count").
        /// </summary>

        public string TestTypeName { get; set; }

        /// <summary>
        ///     Gets or sets the code associated with the lab test (e.g., CPT or ICD codes).
        /// </summary>

        public string TestCode { get; set; }

        /// <summary>
        ///     Gets or sets the result of the lab test.
        /// </summary>

        public string Result { get; set; }

        /// <summary>
        ///     Gets or sets any abnormalities detected during the lab test, if applicable.
        /// </summary>

        public string Abnormality { get; set; }

        #endregion
    }
}