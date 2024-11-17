using HealthcareSystem.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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

        public List<RoutineCheckup> GetRoutineCheckups()
        {
            var routineCheckupList = new List<RoutineCheckup>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

            var query = @"
        SELECT 
            v.visit_id, 
            v.appt_id, 
            v.blood_pressure_reading, 
            v.body_temp, 
            v.weight, 
            v.height, 
            v.pulse, 
            v.symptoms, 
            v.initial_diagnosis, 
            v.final_diagnosis, 
            v.lab_test_id, 
            lt.test_code, 
            lt.test_type_name
        FROM 
            visit v
        LEFT JOIN 
            lab_test lt ON v.lab_test_id = lt.lab_test_id;";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var visitIdOrdinal = reader.GetOrdinal("visit_id");
            var apptIdOrdinal = reader.GetOrdinal("appt_id");
            var bloodPressureOrdinal = reader.GetOrdinal("blood_pressure_reading");
            var bodyTempOrdinal = reader.GetOrdinal("body_temp");
            var weightOrdinal = reader.GetOrdinal("weight");
            var heightOrdinal = reader.GetOrdinal("height");
            var pulseOrdinal = reader.GetOrdinal("pulse");
            var symptomsOrdinal = reader.GetOrdinal("symptoms");
            var initialDiagnosisOrdinal = reader.GetOrdinal("initial_diagnosis");
            var finalDiagnosisOrdinal = reader.GetOrdinal("final_diagnosis");
            var labTestIdOrdinal = reader.GetOrdinal("lab_test_id");
            var testCodeOrdinal = reader.GetOrdinal("test_code");
            var testTypeNameOrdinal = reader.GetOrdinal("test_type_name");

            while (reader.Read())
            {
                routineCheckupList.Add(CreateRoutineCheckup(
                    reader,
                    visitIdOrdinal,
                    apptIdOrdinal,
                    bloodPressureOrdinal,
                    bodyTempOrdinal,
                    weightOrdinal,
                    heightOrdinal,
                    pulseOrdinal,
                    symptomsOrdinal,
                    initialDiagnosisOrdinal,
                    finalDiagnosisOrdinal,
                    labTestIdOrdinal
                ));
            }

            return routineCheckupList;
        }

        public bool CompleteRoutineCheckupWithTests(int appointmentId, string bloodPressureReading, decimal bodyTemp,
    decimal weight, decimal height, int pulse, string symptoms, string initialDiagnosis,
    string finalDiagnosis, List<string> testTypes)
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

                // If no tests are selected, set lab_test_id to NULL in the visit table
                int? labTestId = testTypes.Count > 0 ? GenerateRandomLabTestId() : (int?)null;

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
                    updateVisitCommand.Parameters.AddWithValue("@lab_test_id", (object)labTestId ?? DBNull.Value);
                    updateVisitCommand.Parameters.AddWithValue("@visit_id", visitId);
                    updateVisitCommand.ExecuteNonQuery();
                }

                if (testTypes.Count > 0)
                {
                    string labTestQuery = @"
                INSERT INTO lab_test (lab_test_id, visit_id, time_performed, test_type_name)
                VALUES (@lab_test_id, @visit_id, NOW(), @test_type_name);";

                    foreach (string testType in testTypes)
                    {
                        using var labTestCommand = new MySqlCommand(labTestQuery, connection, transaction);
                        int individualLabTestId = GenerateRandomLabTestId();
                        labTestCommand.Parameters.AddWithValue("@lab_test_id", individualLabTestId);
                        labTestCommand.Parameters.AddWithValue("@visit_id", visitId);
                        labTestCommand.Parameters.AddWithValue("@test_type_name", testType);
                        labTestCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CompleteRoutineCheckupWithTests: {ex.Message}");
                return false;
            }
        }

        private int GenerateRandomLabTestId()
        {
            Random random = new Random();
            return random.Next(100000, 999999); // Generate a 6-digit ID
        }


        private static RoutineCheckup CreateRoutineCheckup(
            MySqlDataReader reader,
            int visitIdOrdinal,
            int apptIdOrdinal,
            int bloodPressureOrdinal,
            int bodyTempOrdinal,
            int weightOrdinal,
            int heightOrdinal,
            int pulseOrdinal,
            int symptomsOrdinal,
            int initialDiagnosisOrdinal,
            int finalDiagnosisOrdinal,
            int labTestIdOrdinal
            )
        {
            return new RoutineCheckup
            {
                VisitId = reader.IsDBNull(visitIdOrdinal) ? (int?)null : reader.GetInt32(visitIdOrdinal),
                AppointmentId = reader.IsDBNull(apptIdOrdinal) ? (int?)null : reader.GetInt32(apptIdOrdinal),
                BloodPressureReading = reader.IsDBNull(bloodPressureOrdinal) ? null : reader.GetString(bloodPressureOrdinal),
                BodyTemp = reader.IsDBNull(bodyTempOrdinal) ? (decimal?)null : reader.GetDecimal(bodyTempOrdinal),
                Weight = reader.IsDBNull(weightOrdinal) ? (decimal?)null : reader.GetDecimal(weightOrdinal),
                Height = reader.IsDBNull(heightOrdinal) ? (decimal?)null : reader.GetDecimal(heightOrdinal),
                Pulse = reader.IsDBNull(pulseOrdinal) ? (int?)null : reader.GetInt32(pulseOrdinal),
                Symptoms = reader.IsDBNull(symptomsOrdinal) ? null : reader.GetString(symptomsOrdinal),
                InitialDiagnosis = reader.IsDBNull(initialDiagnosisOrdinal) ? null : reader.GetString(initialDiagnosisOrdinal),
                FinalDiagnosis = reader.IsDBNull(finalDiagnosisOrdinal) ? null : reader.GetString(finalDiagnosisOrdinal),
                LabTestId = reader.IsDBNull(labTestIdOrdinal) ? (int?)null : reader.GetInt32(labTestIdOrdinal),
            };
        }

        public List<LabTest> GetLabTestsForVisit(int? visitId)
        {
            var labTestList = new List<LabTest>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

            var query = @"
                SELECT 
                    lt.lab_test_id,
                    lt.visit_id,
                    lt.time_performed,
                    lt.test_type_name,
                    lt.test_code,
                    lt.result,
                    lt.abnormality
                FROM 
                    lab_test lt
                WHERE 
                    lt.visit_id = @visit_id;";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@visit_id", visitId);

            using var reader = command.ExecuteReader();

            var labTestIdOrdinal = reader.GetOrdinal("lab_test_id");
            var visitIdOrdinal = reader.GetOrdinal("visit_id");
            var timePerformedOrdinal = reader.GetOrdinal("time_performed");
            var testTypeNameOrdinal = reader.GetOrdinal("test_type_name");
            var testCodeOrdinal = reader.GetOrdinal("test_code");
            var resultOrdinal = reader.GetOrdinal("result");
            var abnormalityOrdinal = reader.GetOrdinal("abnormality");

            while (reader.Read())
            {
                labTestList.Add(CreateLabTest(
                    reader,
                    labTestIdOrdinal,
                    visitIdOrdinal,
                    timePerformedOrdinal,
                    testTypeNameOrdinal,
                    testCodeOrdinal,
                    resultOrdinal,
                    abnormalityOrdinal
                ));
            }

            return labTestList;
        }

        private static LabTest CreateLabTest(
            MySqlDataReader reader,
            int labTestIdOrdinal,
            int visitIdOrdinal,
            int timePerformedOrdinal,
            int testTypeNameOrdinal,
            int testCodeOrdinal,
            int resultOrdinal,
            int abnormalityOrdinal)
        {
            return new LabTest
            {
                LabTestId = reader.GetInt32(labTestIdOrdinal),
                VisitId = reader.GetInt32(visitIdOrdinal),
                TimePerformed = reader.GetDateTime(timePerformedOrdinal),
                TestTypeName = reader.IsDBNull(testTypeNameOrdinal) ? null : reader.GetString(testTypeNameOrdinal),
                TestCode = reader.IsDBNull(testCodeOrdinal) ? null : reader.GetString(testCodeOrdinal),
                Result = reader.IsDBNull(resultOrdinal) ? null : reader.GetString(resultOrdinal),
                Abnormality = reader.IsDBNull(abnormalityOrdinal) ? null : reader.GetString(abnormalityOrdinal)
            };
        }

    }
}