use cs3230f24e;

SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE account (
    username VARCHAR(50) PRIMARY KEY,
    password VARCHAR(255),
    fname VARCHAR(50),
    lname VARCHAR(50)
);

CREATE TABLE mailing_address (
    street_address VARCHAR(255),
    zip CHAR(10),
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),
    pid INT,
    PRIMARY KEY (street_address, zip),
    FOREIGN KEY (pid) REFERENCES person(pid)
);

CREATE TABLE person (
    pid INT PRIMARY KEY,
    ssn CHAR(11) UNIQUE,
    gender CHAR(1),
    fname VARCHAR(50),
    lname VARCHAR(50),
    dob DATE,
    street_address VARCHAR(255),
    zip CHAR(10),
    FOREIGN KEY (street_address, zip) REFERENCES mailing_address(street_address, zip)
);

CREATE TABLE administrator (
    admin_id INT PRIMARY KEY,
    pid INT,
    username VARCHAR(50),
    FOREIGN KEY (pid) REFERENCES person(pid),
    FOREIGN KEY (username) REFERENCES account(username)
);

CREATE TABLE nurse (
    nurse_id INT PRIMARY KEY,
    pid INT,
    username VARCHAR(50),
    FOREIGN KEY (pid) REFERENCES person(pid),
    FOREIGN KEY (username) REFERENCES account(username)
);

CREATE TABLE patient (
    patient_id INT PRIMARY KEY,
    pid INT,
    phone_number VARCHAR(15),
    FOREIGN KEY (pid) REFERENCES person(pid)
);

CREATE TABLE specialty (
    specialty_id INT PRIMARY KEY,
    name VARCHAR(100)
);

CREATE TABLE doctor (
    doctor_id INT PRIMARY KEY,
    pid INT,
    FOREIGN KEY (pid) REFERENCES person(pid)
);

CREATE TABLE specialized_in (
    specialty_id INT,
    doctor_id INT,
    PRIMARY KEY (specialty_id, doctor_id),
    FOREIGN KEY (specialty_id) REFERENCES specialty(specialty_id),
    FOREIGN KEY (doctor_id) REFERENCES doctor(doctor_id)
);

CREATE TABLE appointment (
    appt_id INT PRIMARY KEY,
    doctor_id INT,
    patient_id INT,
    datetime DATETIME,
    appt_reason VARCHAR(255),
    visit_id INT,
    UNIQUE (patient_id, datetime),
    FOREIGN KEY (doctor_id) REFERENCES doctor(doctor_id),
    FOREIGN KEY (patient_id) REFERENCES patient(patient_id)
);

CREATE TABLE visit (
    visit_id INT PRIMARY KEY,
    appt_id INT,
    datetime DATETIME,
    blood_pressure_reading VARCHAR(10),
    body_temp DECIMAL(5,2),
    weight DECIMAL(5,2),
    height DECIMAL(5,2),
    pulse INT,
    symptoms TEXT,
    initial_diagnosis TEXT,
    final_diagnosis TEXT,
    lab_test_id INT,
    UNIQUE (appt_id, datetime),
    FOREIGN KEY (appt_id) REFERENCES appointment(appt_id),
    FOREIGN KEY (lab_test_id) REFERENCES lab_test(lab_test_id)
);

CREATE TABLE lab_test (
    lab_test_id INT PRIMARY KEY,
    visit_id INT,
    time_performed DATETIME,
    test_type_name VARCHAR(100),
    test_code VARCHAR(50),
    result VARCHAR(255),
    abnormality VARCHAR(255),
    UNIQUE (visit_id, time_performed, test_type_name, test_code),
    FOREIGN KEY (visit_id) REFERENCES visit(visit_id),
    FOREIGN KEY (test_code) REFERENCES test_type(test_code)
);

CREATE TABLE test_type (
    test_code VARCHAR(50) PRIMARY KEY,
    test_type_name VARCHAR(100),
    low_value DECIMAL(10,2),
    high_value DECIMAL(10,2),
    unit_measurement VARCHAR(20)
);
