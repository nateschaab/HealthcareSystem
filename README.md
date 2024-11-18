# HealthcareSystem
CS3230 Project: A Healthcare System


Username: 1
Password: 1

Username: 2
Password: 2

Username: testusername
Password: testpwd


----------------------------------------------------------------------
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
