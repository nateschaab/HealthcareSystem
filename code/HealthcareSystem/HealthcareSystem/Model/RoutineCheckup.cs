using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareSystem.Model
{
    public class RoutineCheckup
    {
        public int VisitId { get; set; }
        public int AppointmentId { get; set; }
        public string BloodPressureReading { get; set; }
        public decimal BodyTemp { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public int Pulse { get; set; }
        public string Symptoms { get; set; }
        public string InitialDiagnosis { get; set; }
        public string FinalDiagnosis { get; set; }
        public int LabTestId { get; set; }
        public string TestCode { get; set; }
        public string TestTypeName { get; set; }
        public int Systolic
        {
            get
            {
                return int.Parse(BloodPressureReading.Split('/')[0]);
            }
        }
        public int Dystolic
        {
            get
            {
                return int.Parse(BloodPressureReading.Split('/')[1]);
            }
        }
    }
}
