using DBAccess.DAL;
using HealthcareSystem.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Windows.Networking;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientManagementPage : Page
    {
        private List<Patient> patients;

        public PatientManagementPage()
        {
            this.InitializeComponent();
            LoadPatients();
        }

        // Load all patients from the database and populate the ListView
        private void LoadPatients()
        {
            Debug.WriteLine("Loading Patients From Database");
            patients = GetPatientsFromDatabase();  // Retrieve patients from the DB

            // Bind the patient list to the ListView
            PatientListView.ItemsSource = patients;
        }

        // Simulate retrieving patients from the DB (replace this with actual DB call)
        private List<Patient> GetPatientsFromDatabase()
        {
            var dal = new PatientDal();
            // Example code - this should be replaced with actual DB retrieval logic
            return dal.GetPatientsFromReader();
        }

        // Event when a patient is selected in the ListView
        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientListView.SelectedItem is Patient selectedPatient)
            {
                // Load the selected patient's data into the input fields
                PatientSSNTextBox.Text = selectedPatient.SSN;
                PatientFirstNameTextBox.Text = selectedPatient.FirstName;
                PatientLastNameTextBox.Text = selectedPatient.LastName;
                DOBDatePicker.Date = selectedPatient.DateOfBirth;
                //GenderComboBox.SelectedValue = selectedPatient.Gender;
                StreetAddressTextBox.Text = selectedPatient.MailAddress.StreetAddress;
                ZipCodeTextBox.Text = selectedPatient.MailAddress.Zip;
                CityTextBox.Text = selectedPatient.MailAddress.City;
                //StateComboBox.SelectedValue = selectedPatient.MailAddress.State;
                //CountryComboBox.SelectedValue = selectedPatient.MailAddress.Country;
                PhoneNumberTextBox.Text = selectedPatient.PhoneNumber;

                // Set ComboBox value by directly matching the content of ComboBoxItem
                GenderComboBox.SelectedItem = GenderComboBox.Items
                    .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.Gender);

                StateComboBox.SelectedItem = StateComboBox.Items
                    .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.State);

                CountryComboBox.SelectedItem = CountryComboBox.Items
                    .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.Country);
            }
        }

        private void RegisterPatient_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous error messages
            SSNErrorTextBlock.Visibility = Visibility.Collapsed;
            GenderErrorTextBlock.Visibility = Visibility.Collapsed;
            ZipCodeErrorTextBlock.Visibility = Visibility.Collapsed;
            PhoneErrorTextBlock.Visibility = Visibility.Collapsed;
            StateErrorTextBlock.Visibility = Visibility.Collapsed;

            // Validate each field
            bool isValid = true;

            // Validate SSN (example: 9 digits)
            if (!Regex.IsMatch(PatientSSNTextBox.Text, @"^\d{9}$"))
            {
                SSNErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Gender selection
            if (GenderComboBox.SelectedItem == null)
            {
                GenderErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Zip Code (example: US 5-digit code)
            if (!Regex.IsMatch(ZipCodeTextBox.Text, @"^\d{5}$"))
            {
                ZipCodeErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Phone Number (example: US 10-digit number)
            if (!Regex.IsMatch(PhoneNumberTextBox.Text, @"^\d{10}$"))
            {
                PhoneErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate State selection
            if (StateComboBox.SelectedItem == null)
            {
                StateErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate State selection
            if (CountryComboBox.SelectedItem == null)
            {
                CountryErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // If all fields are valid, proceed to register the patient
            if (isValid)
            {
                // Code to register the patient
                // Example: Saving to database or calling an API
                // Use PatientSSNTextBox.Text, GenderComboBox.SelectedItem, etc.
                var ssn = PatientSSNTextBox.Text;
                var gender = (GenderComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                var firstName = PatientFirstNameTextBox.Text;
                var lastName = PatientLastNameTextBox.Text;
                var dob = DOBDatePicker.Date.DateTime;
                var address = StreetAddressTextBox.Text;
                var zipcode = ZipCodeTextBox.Text;
                var city = CityTextBox.Text;
                var country = (CountryComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                var state = (StateComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                var phoneNumber = PhoneNumberTextBox.Text;

                var patientInfo = new Patient
                (
                    ssn, gender, firstName, lastName, dob, address, zipcode, city, country, state, phoneNumber
                );

                var dal = new PatientDal();
                dal.RegisterPatient( patientInfo );
                // Example output (logging or further action)
                System.Diagnostics.Debug.WriteLine("Patient Registered: " + patientInfo);
                this.LoadPatients();

                foreach (var patient in patients)
                {
                    if (patient.PersonId == patientInfo.PersonId)
                    {
                        PatientListView.SelectedItem = patient;
                        break;
                    }
                }
            }
        }

        // Edit patient in the database
        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            if (PatientListView.SelectedItem is Patient selectedPatient)
            {
                // Clear previous error messages
                SSNErrorTextBlock.Visibility = Visibility.Collapsed;
                GenderErrorTextBlock.Visibility = Visibility.Collapsed;
                ZipCodeErrorTextBlock.Visibility = Visibility.Collapsed;
                PhoneErrorTextBlock.Visibility = Visibility.Collapsed;
                StateErrorTextBlock.Visibility = Visibility.Collapsed;

                // Validate each field
                bool isValid = true;

                // Validate SSN (example: 9 digits)
                if (!Regex.IsMatch(PatientSSNTextBox.Text, @"^\d{9}$"))
                {
                    SSNErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Gender selection
                if (GenderComboBox.SelectedItem == null)
                {
                    GenderErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Zip Code (example: US 5-digit code)
                if (!Regex.IsMatch(ZipCodeTextBox.Text, @"^\d{5}$"))
                {
                    ZipCodeErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Phone Number (example: US 10-digit number)
                if (!Regex.IsMatch(PhoneNumberTextBox.Text, @"^\d{10}$"))
                {
                    PhoneErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate State selection
                if (StateComboBox.SelectedItem == null)
                {
                    StateErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate State selection
                if (CountryComboBox.SelectedItem == null)
                {
                    CountryErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // If all fields are valid, proceed to register the patient
                if (isValid)
                {
                    // Code to register the patient
                    // Example: Saving to database or calling an API
                    // Use PatientSSNTextBox.Text, GenderComboBox.SelectedItem, etc.
                    var patientId = selectedPatient.PatientId;
                    var personId = selectedPatient.PersonId;
                    var phoneNumber = PhoneNumberTextBox.Text;
                    var ssn = PatientSSNTextBox.Text;
                    var gender = (GenderComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    var firstName = PatientFirstNameTextBox.Text;
                    var lastName = PatientLastNameTextBox.Text;
                    var dob = DOBDatePicker.Date.DateTime;
                    var address = StreetAddressTextBox.Text;
                    var zipcode = ZipCodeTextBox.Text;
                    var city = CityTextBox.Text;
                    var country = (CountryComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    var state = (StateComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                    var patientInfo = new Patient
                    (
                        patientId, personId, phoneNumber, ssn, gender, firstName, lastName, dob, address, zipcode, city, country, state
                    );

                    var dal = new PatientDal();
                    dal.UpdatePatientInDatabase(patientInfo);
                    // Example output (logging or further action)
                    Debug.WriteLine("Patient Registered: " + patientInfo);
                }
            }
        }

        // Simulate updating the patient in the database (replace with actual DB update logic)
        private void UpdatePatientInDatabase(Patient patient)
        {
            // Implement the actual DB update logic here
            // Example code (to be replaced with actual SQL update)
            Debug.WriteLine($"Patient {patient.FirstName} {patient.LastName} updated in the database.");
        }
    }
}
