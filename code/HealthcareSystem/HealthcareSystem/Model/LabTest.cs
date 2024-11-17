using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareSystem.Model
{
    public class LabTest
    {
        public int LabTestId { get; set; }
        public int VisitId { get; set; }
        public DateTime TimePerformed { get; set; }
        public string TestTypeName { get; set; }
        public string TestCode { get; set; }
        public string Result { get; set; }
        public string Abnormality { get; set; }
    }

}
