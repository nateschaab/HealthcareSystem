using System.Collections.Generic;

namespace HealthcareSystem.Model
{
    /// <summary>
    ///     Represents a routine checkup performed during a medical visit.
    ///     Contains details about the patient's vitals, symptoms, diagnoses, and lab tests.
    /// </summary>
    public class RoutineCheckup
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the unique identifier of the visit associated with the routine checkup.
        /// </summary>

        public int? VisitId { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier of the appointment associated with the routine checkup.
        /// </summary>

        public int? AppointmentId { get; set; }

        /// <summary>
        ///     Gets or sets the patient's blood pressure reading in the format "systolic/diastolic".
        /// </summary>

        public string BloodPressureReading { get; set; }

        /// <summary>
        ///     Gets or sets the patient's body temperature, typically in degrees Fahrenheit.
        /// </summary>

        public decimal? BodyTemp { get; set; }

        /// <summary>
        ///     Gets or sets the patient's weight, typically in pounds.
        /// </summary>

        public decimal? Weight { get; set; }

        /// <summary>
        ///     Gets or sets the patient's height, typically in inches.
        /// </summary>

        public decimal? Height { get; set; }

        /// <summary>
        ///     Gets or sets the patient's pulse rate in beats per minute.
        /// </summary>

        public int? Pulse { get; set; }

        /// <summary>
        ///     Gets or sets a description of the patient's symptoms as reported during the visit.
        /// </summary>

        public string Symptoms { get; set; }

        /// <summary>
        ///     Gets or sets the initial diagnosis made during the checkup.
        /// </summary>

        public string InitialDiagnosis { get; set; }

        /// <summary>
        ///     Gets or sets the final diagnosis confirmed during the checkup.
        /// </summary>

        public string FinalDiagnosis { get; set; }

        /// <summary>
        ///     Gets or sets the list of lab tests conducted during the visit.
        /// </summary>

        public IList<LabTest> LabTests { get; set; }

        /// <summary>
        ///     Gets the systolic value from the blood pressure reading.
        /// </summary>
        /// <exception cref="System.FormatException">
        ///     Thrown if the <see cref="BloodPressureReading" /> is not in the expected "systolic/diastolic" format.
        /// </exception>

        public int Systolic => int.Parse(this.BloodPressureReading.Split('/')[0]);

        /// <summary>
        ///     Gets the diastolic value from the blood pressure reading.
        /// </summary>
        /// <exception cref="System.FormatException">
        ///     Thrown if the <see cref="BloodPressureReading" /> is not in the expected "systolic/diastolic" format.
        /// </exception>

        public int Diastolic => int.Parse(this.BloodPressureReading.Split('/')[1]);

        #endregion
    }
}