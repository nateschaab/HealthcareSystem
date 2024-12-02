using System;
using System.Collections.Generic;
using System.Data;
using HealthcareSystem.DAL;
using HealthcareSystem.Model;
using MySql.Data.MySqlClient;

namespace DBAccess.DAL
{
    /// <summary>
    ///     Data access layer for managing visit-related operations in the database.
    /// </summary>
    public class VisitDAL
    {
        #region Methods

        /// <summary>
        ///     Completes a routine checkup by updating the visit and lab test information in the database.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment associated with the routine checkup.</param>
        /// <param name="bloodPressureReading">The blood pressure reading of the patient.</param>
        /// <param name="bodyTemp">The body temperature of the patient.</param>
        /// <param name="weight">The weight of the patient.</param>
        /// <param name="height">The height of the patient.</param>
        /// <param name="pulse">The pulse rate of the patient.</param>
        /// <param name="symptoms">The symptoms reported by the patient.</param>
        /// <param name="initialDiagnosis">The initial diagnosis made by the doctor.</param>
        /// <param name="finalDiagnosis">The final diagnosis made by the doctor.</param>
        /// <param name="labTestId">The ID of the lab test performed.</param>
        /// <param name="testCode">The test code of the lab test.</param>
        /// <param name="testTypeName">The name of the test type performed.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
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
                var getVisitIdQuery = "SELECT visit_id FROM visit WHERE appt_id = @appt_id;";
                using (var getVisitIdCommand = new MySqlCommand(getVisitIdQuery, connection, transaction))
                {
                    getVisitIdCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                    visitId = Convert.ToInt32(getVisitIdCommand.ExecuteScalar());
                }

                var labTestQuery = @"
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

                var updateVisitQuery = @"
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

        /// <summary>
        ///     Retrieves a list of all routine checkups from the database.
        /// </summary>
        /// <returns>A list of <see cref="RoutineCheckup" /> objects representing the routine checkups.</returns>
        public bool CheckIfRoutineCheckupExists(int appointmentId)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var query = @"
                    SELECT COUNT(*) 
                    FROM visit 
                    WHERE appt_id = @appt_id 
                        AND (blood_pressure_reading IS NOT NULL OR body_temp IS NOT NULL 
                        OR weight IS NOT NULL OR height IS NOT NULL OR pulse IS NOT NULL 
                        OR symptoms IS NOT NULL OR initial_diagnosis IS NOT NULL 
                        OR final_diagnosis IS NOT NULL OR lab_test_id IS NOT NULL);";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@appt_id", appointmentId);

                var count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckIfRoutineCheckupExists: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Retrieves a list of all routine checkups from the database.
        /// </summary>
        /// <returns>A list of <see cref="RoutineCheckup" /> objects representing the routine checkups.</returns>
        public List<RoutineCheckup> GetRoutineCheckups()
        {
            var routineCheckupList = new List<RoutineCheckup>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

            using var command = new MySqlCommand("GetRoutineCheckups", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

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
                    finalDiagnosisOrdinal
                ));
            }

            return routineCheckupList;
        }

        /// <summary>
        ///     Completes a routine checkup with multiple lab tests by updating the visit and lab test information in the database.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment associated with the routine checkup.</param>
        /// <param name="bloodPressureReading">The blood pressure reading of the patient.</param>
        /// <param name="bodyTemp">The body temperature of the patient.</param>
        /// <param name="weight">The weight of the patient.</param>
        /// <param name="height">The height of the patient.</param>
        /// <param name="pulse">The pulse rate of the patient.</param>
        /// <param name="symptoms">The symptoms reported by the patient.</param>
        /// <param name="initialDiagnosis">The initial diagnosis made by the doctor.</param>
        /// <param name="finalDiagnosis">The final diagnosis made by the doctor.</param>
        /// <param name="testTypes">A list of test types performed during the checkup.</param>
        /// <param name="testResults">A dictionary containing the test results for each test type.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool CompleteRoutineCheckupWithTests(int appointmentId, string bloodPressureReading, decimal bodyTemp,
            decimal weight, decimal height, int pulse, string symptoms, string initialDiagnosis,
            string finalDiagnosis, List<string> testTypes, Dictionary<string, string> testResults)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var transaction = connection.BeginTransaction();

                int visitId;
                var getVisitIdQuery = "SELECT visit_id FROM visit WHERE appt_id = @appt_id;";
                using (var getVisitIdCommand = new MySqlCommand(getVisitIdQuery, connection, transaction))
                {
                    getVisitIdCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                    visitId = Convert.ToInt32(getVisitIdCommand.ExecuteScalar());
                }

                var updateVisitQuery = @"
            UPDATE visit 
            SET 
                blood_pressure_reading = @blood_pressure_reading,
                body_temp = @body_temp,
                weight = @weight,
                height = @height,
                pulse = @pulse,
                symptoms = @symptoms,
                initial_diagnosis = @initial_diagnosis,
                final_diagnosis = @final_diagnosis
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
                    updateVisitCommand.Parameters.AddWithValue("@visit_id", visitId);
                    updateVisitCommand.ExecuteNonQuery();
                }

                if (testTypes.Count > 0)
                {
                    var labTestQuery = @"
                INSERT INTO lab_test (lab_test_id, visit_id, time_performed, test_type_name, result)
                VALUES (@lab_test_id, @visit_id, NOW(), @test_type_name, @result)
                ON DUPLICATE KEY UPDATE 
                    result = @result, 
                    time_performed = NOW();";

                    foreach (var testType in testTypes)
                    {
                        using var labTestCommand = new MySqlCommand(labTestQuery, connection, transaction);

                        int labTestId;
                        var fetchLabTestIdQuery = @"
                    SELECT lab_test_id 
                    FROM lab_test 
                    WHERE visit_id = @visit_id AND test_type_name = @test_type_name;";

                        using (var fetchLabTestIdCommand =
                               new MySqlCommand(fetchLabTestIdQuery, connection, transaction))
                        {
                            fetchLabTestIdCommand.Parameters.AddWithValue("@visit_id", visitId);
                            fetchLabTestIdCommand.Parameters.AddWithValue("@test_type_name", testType);

                            var result = fetchLabTestIdCommand.ExecuteScalar();
                            labTestId = result != null ? Convert.ToInt32(result) : this.GenerateRandomLabTestId();
                        }

                        labTestCommand.Parameters.AddWithValue("@lab_test_id", labTestId);
                        labTestCommand.Parameters.AddWithValue("@visit_id", visitId);
                        labTestCommand.Parameters.AddWithValue("@test_type_name", testType);

                        var testResult = testResults.ContainsKey(testType) ? testResults[testType] : null;
                        labTestCommand.Parameters.AddWithValue("@result", testResult ?? (object)DBNull.Value);

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

        /// <summary>
        ///     Generates a random lab test ID for use in database operations.
        /// </summary>
        /// <returns>A randomly generated lab test ID.</returns>
        private int GenerateRandomLabTestId()
        {
            var random = new Random();
            return random.Next(100000, 999999); // Generate a 6-digit ID
        }

        /// <summary>
        ///     Creates a <see cref="RoutineCheckup" /> object from a database record.
        /// </summary>
        /// <param name="reader">The <see cref="MySqlDataReader" /> containing the data record.</param>
        /// <param name="visitIdOrdinal">The ordinal index of the visit ID column.</param>
        /// <param name="apptIdOrdinal">The ordinal index of the appointment ID column.</param>
        /// <param name="bloodPressureOrdinal">The ordinal index of the blood pressure column.</param>
        /// <param name="bodyTempOrdinal">The ordinal index of the body temperature column.</param>
        /// <param name="weightOrdinal">The ordinal index of the weight column.</param>
        /// <param name="heightOrdinal">The ordinal index of the height column.</param>
        /// <param name="pulseOrdinal">The ordinal index of the pulse column.</param>
        /// <param name="symptomsOrdinal">The ordinal index of the symptoms column.</param>
        /// <param name="initialDiagnosisOrdinal">The ordinal index of the initial diagnosis column.</param>
        /// <param name="finalDiagnosisOrdinal">The ordinal index of the final diagnosis column.</param>
        /// <returns>A <see cref="RoutineCheckup" /> object containing the checkup details.</returns>
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
            int finalDiagnosisOrdinal
        )
        {
            return new RoutineCheckup
            {
                VisitId = reader.IsDBNull(visitIdOrdinal) ? (int?)null : reader.GetInt32(visitIdOrdinal),
                AppointmentId = reader.IsDBNull(apptIdOrdinal) ? (int?)null : reader.GetInt32(apptIdOrdinal),
                BloodPressureReading =
                    reader.IsDBNull(bloodPressureOrdinal) ? null : reader.GetString(bloodPressureOrdinal),
                BodyTemp = reader.IsDBNull(bodyTempOrdinal) ? (decimal?)null : reader.GetDecimal(bodyTempOrdinal),
                Weight = reader.IsDBNull(weightOrdinal) ? (decimal?)null : reader.GetDecimal(weightOrdinal),
                Height = reader.IsDBNull(heightOrdinal) ? (decimal?)null : reader.GetDecimal(heightOrdinal),
                Pulse = reader.IsDBNull(pulseOrdinal) ? (int?)null : reader.GetInt32(pulseOrdinal),
                Symptoms = reader.IsDBNull(symptomsOrdinal) ? null : reader.GetString(symptomsOrdinal),
                InitialDiagnosis = reader.IsDBNull(initialDiagnosisOrdinal)
                    ? null
                    : reader.GetString(initialDiagnosisOrdinal),
                FinalDiagnosis = reader.IsDBNull(finalDiagnosisOrdinal) ? null : reader.GetString(finalDiagnosisOrdinal)
            };
        }

        /// <summary>
        ///     Retrieves all lab tests associated with a specific visit.
        /// </summary>
        /// <param name="visitId">The ID of the visit to retrieve lab tests for.</param>
        /// <returns>A list of <see cref="LabTest" /> objects representing the lab tests for the visit.</returns>
        public List<LabTest> GetLabTestsForVisit(int? visitId)
        {
            var labTestList = new List<LabTest>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

            using var command = new MySqlCommand("GetLabTestsForVisit", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_visit_id", visitId);

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

        /// <summary>
        ///     Creates a <see cref="LabTest" /> object from a database record.
        /// </summary>
        /// <param name="reader">The <see cref="MySqlDataReader" /> containing the data record.</param>
        /// <param name="labTestIdOrdinal">The ordinal index of the lab test ID column.</param>
        /// <param name="visitIdOrdinal">The ordinal index of the visit ID column.</param>
        /// <param name="timePerformedOrdinal">The ordinal index of the time performed column.</param>
        /// <param name="testTypeNameOrdinal">The ordinal index of the test type name column.</param>
        /// <param name="testCodeOrdinal">The ordinal index of the test code column.</param>
        /// <param name="resultOrdinal">The ordinal index of the result column.</param>
        /// <param name="abnormalityOrdinal">The ordinal index of the abnormality column.</param>
        /// <returns>A <see cref="LabTest" /> object containing the lab test details.</returns>
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

        /// <summary>
        ///     Retrieves a routine checkup by the associated appointment ID.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to retrieve the checkup for.</param>
        /// <returns>A <see cref="RoutineCheckup" /> object containing the routine checkup details, or null if not found.</returns>
        public RoutineCheckup GetRoutineCheckupByAppointmentId(int appointmentId)
        {
            RoutineCheckup routineCheckup = null;

            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var query = @"
            SELECT 
                visit_id, appt_id, blood_pressure_reading, body_temp, weight, height, pulse,
                symptoms, initial_diagnosis, final_diagnosis
            FROM visit
            WHERE appt_id = @appt_id;";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@appt_id", appointmentId);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    routineCheckup = new RoutineCheckup
                    {
                        VisitId = reader.GetInt32(reader.GetOrdinal("visit_id")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("appt_id")),
                        BloodPressureReading = reader.IsDBNull(reader.GetOrdinal("blood_pressure_reading"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("blood_pressure_reading")),
                        BodyTemp = reader.IsDBNull(reader.GetOrdinal("body_temp"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("body_temp")),
                        Weight = reader.IsDBNull(reader.GetOrdinal("weight"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("weight")),
                        Height = reader.IsDBNull(reader.GetOrdinal("height"))
                            ? (decimal?)null
                            : reader.GetDecimal(reader.GetOrdinal("height")),
                        Pulse = reader.IsDBNull(reader.GetOrdinal("pulse"))
                            ? (int?)null
                            : reader.GetInt32(reader.GetOrdinal("pulse")),
                        Symptoms = reader.IsDBNull(reader.GetOrdinal("symptoms"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("symptoms")),
                        InitialDiagnosis = reader.IsDBNull(reader.GetOrdinal("initial_diagnosis"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("initial_diagnosis")),
                        FinalDiagnosis = reader.IsDBNull(reader.GetOrdinal("final_diagnosis"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("final_diagnosis"))
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRoutineCheckupByAppointmentId: {ex.Message}");
            }

            return routineCheckup;
        }

        #endregion
    }
}