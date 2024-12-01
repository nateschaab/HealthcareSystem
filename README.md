# HealthcareSystem
CS3230 Project: A Healthcare System


Username: 1
Password: 1

Username: 2
Password: 2

Username: testusername
Password: testpwd

----------------------------------------------------------------------

# Setup

In Visual Studio IDE, Go to Tools => NuGet Package Manager => Package Manager Console

In Console, Enter the following:

Install-Package MySqlConnector -Version 0.34.0

----------------------------------------------------------------------

# Stored Procedures
// Trinidad's Stored Procedure:


use cs3230f24e;

DELIMITER $$

CREATE PROCEDURE GetLabTestsForVisit(IN p_visit_id INT)
BEGIN
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
        lt.visit_id = p_visit_id;
END$$

DELIMITER ;


----------------------------------------------------------------------
// Nate's Stored Procedure:


CREATE DEFINER=`cs3230f24e`@`%` PROCEDURE `GetRoutineCheckups`()
BEGIN
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
        lt.test_code, 
        lt.test_type_name
    FROM 
        visit v
    LEFT JOIN 
        lab_test lt ON v.visit_id = lt.visit_id;
END


----------------------------------------------------------------------

# Test Queries

Test Queries:

SELECT 
    p.patient_id AS patientId,
    CONCAT(pr.fname, ' ', pr.lname) AS patientName,
    v.datetime AS visitDate,
    v.blood_pressure_reading AS bloodPressure,
    CONCAT(npr.fname, ' ', npr.lname) AS nurseName,
    n.nurse_id AS nurseId,
    CONCAT(dpr.fname, ' ', dpr.lname) AS doctorName,
    d.doctor_id AS doctorId,
    lt.test_type_name AS testName,
    lt.result AS testResult,
    v.initial_diagnosis AS initialDiagnosis,
    v.final_diagnosis AS finalDiagnosis
FROM 
    patient p
JOIN 
    person pr ON p.pid = pr.pid
JOIN 
    appointment a ON p.patient_id = a.patient_id
JOIN 
    visit v ON a.visit_id = v.visit_id
LEFT JOIN 
    nurse n ON n.nurse_id = a.doctor_id
LEFT JOIN 
    person npr ON n.pid = npr.pid
LEFT JOIN 
    doctor d ON a.doctor_id = d.doctor_id
LEFT JOIN 
    person dpr ON d.pid = dpr.pid
LEFT JOIN 
    lab_test lt ON v.visit_id = lt.visit_id
WHERE 
    CONCAT(pr.fname, ' ', pr.lname) = 'Joe Smith'
ORDER BY 
    v.datetime;

----------------------------------------------------------------------

SELECT 
    p.patient_id AS patientId,
    CONCAT(pr.fname, ' ', pr.lname) AS patientName,
    DATE(lt.time_performed) AS testDate,
    COUNT(lt.lab_test_id) AS totalTests
FROM 
    patient p
JOIN 
    person pr ON p.pid = pr.pid
JOIN 
    appointment a ON p.patient_id = a.patient_id
JOIN 
    visit v ON a.visit_id = v.visit_id
JOIN 
    lab_test lt ON v.visit_id = lt.visit_id
GROUP BY 
    p.patient_id, CONCAT(pr.fname, ' ', pr.lname), DATE(lt.time_performed)
HAVING 
    COUNT(lt.lab_test_id) >= 2
ORDER BY 
    p.patient_id, testDate;
----------------------------------------------------------------------
