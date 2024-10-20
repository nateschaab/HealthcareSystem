using System;

namespace DBAccess.Model
{
/// <summary>
/// The Employee Class.
/// </summary>
   public class Employee
    {
        public string FirstName { get; }
        public int?  DepartmentNumber { get;  }
        public DateTime BirthDate { get;  }

        public Employee(string firstname, DateTime? birthdate, int? dno)
        {
            this.FirstName = firstname ?? throw new ArgumentNullException(nameof(firstname), $"{nameof(firstname)} cannot be null");
            this.BirthDate = birthdate ?? throw new ArgumentNullException(nameof(birthdate), $"{nameof(birthdate)} cannot be null");


            if (dno < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dno), $"{nameof(dno)} cannot be negative");
            }

            this.DepartmentNumber = dno;
        }
    }
}
