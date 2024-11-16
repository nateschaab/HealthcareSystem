using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;

namespace HealthcareSystem.Model
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int VisitId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public string DisplayName => $"{this.AppointmentId} : {Date}, {Reason}";

        public Appointment(int appId, int visitId, int doctorId, int patientId, DateTime date, string reason)
        {
            AppointmentId = appId;
            VisitId = visitId;
            DoctorId = doctorId;
            PatientId = patientId;
            Date = date;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"{AppointmentId} - {Date}"; // Example fields
        }
    }
}