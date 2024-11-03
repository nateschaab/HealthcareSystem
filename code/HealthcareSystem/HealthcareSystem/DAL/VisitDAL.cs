using MySql.Data.MySqlClient;
using System;

namespace DBAccess.DAL
{
    public class VisitDAL
    {
        public bool CompleteRoutineCheckup(int appointmentId, string bloodPressureReading, decimal bodyTemp,
            decimal weight, decimal height, int pulse, string symptoms, string initialDiagnosis,
            string finalDiagnosis, int labTestId, string testCode, string testTypeName)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var transaction = connection.BeginTransaction();

                int visitId;
                string getVisitIdQuery = "SELECT visit_id FROM visit WHERE appt_id = @appt_id;";
                using (var getVisitIdCommand = new MySqlCommand(getVisitIdQuery, connection, transaction))
                {
                    getVisitIdCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                    visitId = Convert.ToInt32(getVisitIdCommand.ExecuteScalar());
                }

                string labTestQuery = @"
            INSERT INTO lab_test (lab_test_id, visit_id, time_performed, test_code, test_type_name)
            VALUES (@lab_test_id, @visit_id, NOW(), @test_code, @test_type_name);";

                using (var labTestCommand = new MySqlCommand(labTestQuery, connection, transaction))
                {
                    labTestCommand.Parameters.AddWithValue("@lab_test_id", labTestId);
                    labTestCommand.Parameters.AddWithValue("@visit_id", visitId);
                    labTestCommand.Parameters.AddWithValue("@test_code", testCode);
                    labTestCommand.Parameters.AddWithValue("@test_type_name", testTypeName);
                    labTestCommand.ExecuteNonQuery();
                }

                string updateVisitQuery = @"
            UPDATE visit 
            SET 
                blood_pressure_reading = @blood_pressure_reading,
                body_temp = @body_temp,
                weight = @weight,
                height = @height,
                pulse = @pulse,
                symptoms = @symptoms,
                initial_diagnosis = @initial_diagnosis,
                final_diagnosis = @final_diagnosis,
                lab_test_id = @lab_test_id
            WHERE visit_id = @visit_id;";

                using (var updateVisitCommand = new MySqlCommand(updateVisitQuery, connection, transaction))
                {
                    updateVisitCommand.Parameters.AddWithValue("@blood_pressure_reading", bloodPressureReading);
                    updateVisitCommand.Parameters.AddWithValue("@body_temp", bodyTemp);
                    updateVisitCommand.Parameters.AddWithValue("@weight", weight);
                    updateVisitCommand.Parameters.AddWithValue("@height", height);
                    updateVisitCommand.Parameters.AddWithValue("@pulse", pulse);
                    updateVisitCommand.Parameters.AddWithValue("@symptoms", symptoms);
                    updateVisitCommand.Parameters.AddWithValue("@initial_diagnosis", initialDiagnosis);
                    updateVisitCommand.Parameters.AddWithValue("@final_diagnosis", finalDiagnosis);
                    updateVisitCommand.Parameters.AddWithValue("@lab_test_id", labTestId);
                    updateVisitCommand.Parameters.AddWithValue("@visit_id", visitId);
                    updateVisitCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CompleteRoutineCheckup: {ex.Message}");
                return false;
            }
        }

        public bool CheckIfRoutineCheckupExists(int appointmentId)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                string query = @"
                    SELECT COUNT(*) 
                    FROM visit 
                    WHERE appt_id = @appt_id 
                        AND (blood_pressure_reading IS NOT NULL OR body_temp IS NOT NULL 
                        OR weight IS NOT NULL OR height IS NOT NULL OR pulse IS NOT NULL 
                        OR symptoms IS NOT NULL OR initial_diagnosis IS NOT NULL 
                        OR final_diagnosis IS NOT NULL OR lab_test_id IS NOT NULL);";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@appt_id", appointmentId);

                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckIfRoutineCheckupExists: {ex.Message}");
                return false;
            }
        }


    }
}