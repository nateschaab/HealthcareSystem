using DBAccess.DAL;
using DBAccess.Model;
using HealthcareSystem.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Windows.Networking;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientManagementPage : Page
    {
        private Patient patient;

        private List<Patient> patients;

        List<String> genderItems;

        List<String> stateItems;

        List<String> countryItems;

        public PatientManagementPage()
        {
            this.InitializeComponent();
            LoadPatients();
        }

        public PatientManagementPage(Patient selectedPatient)
        {
            this.InitializeComponent();
            this.patient = selectedPatient;
            // You can now use this.patient within this page
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Patient selectedPatient)
            {
                this.patient = selectedPatient;
                var matchingPatient = PatientListView.Items.Cast<Patient>()
                    .FirstOrDefault(p => p.PatientId == selectedPatient.PatientId); // assuming Patient has an Id property
                PatientListView.SelectedItem = matchingPatient;

                this.PopulatePatientFields(patient);
            }
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
                PopulatePatientFields(selectedPatient);
            }
        }

        private void PopulatePatientFields(Patient selectedPatient)
        {
            // Load the selected patient's data into the input fields
            PatientSSNTextBox.Text = selectedPatient.SSN;
            PatientFirstNameTextBox.Text = selectedPatient.FirstName;
            PatientLastNameTextBox.Text = selectedPatient.LastName;
            DOBDatePicker.Date = selectedPatient.DateOfBirth;
            StreetAddressTextBox.Text = selectedPatient.MailAddress?.StreetAddress ?? string.Empty;
            ZipCodeTextBox.Text = selectedPatient.MailAddress?.Zip ?? string.Empty;
            CityTextBox.Text = selectedPatient.MailAddress?.City ?? string.Empty;
            PhoneNumberTextBox.Text = selectedPatient.PhoneNumber;

            // Set ComboBox value for Gender
            GenderComboBox.SelectedItem = GenderComboBox.Items
            .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.Gender);

            // Check if MailAddress is not null before setting State and Country
            if (selectedPatient.MailAddress != null)
            {
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
            CountryErrorTextBlock.Visibility = Visibility.Collapsed;
            StreetAddressErrorTextBlock.Visibility = Visibility.Collapsed;
            CityErrorTextBlock.Visibility = Visibility.Collapsed;
            DOBErrorTextBlock.Visibility = Visibility.Collapsed;
            FirstNameErrorTextBlock.Visibility = Visibility.Collapsed;
            LastNameErrorTextBlock.Visibility = Visibility.Collapsed;

            // Validate each field
            bool isValid = true;

            // Validate SSN (example: 9 digits)
            if (!Regex.IsMatch(PatientSSNTextBox.Text, @"^\d{9}$"))
            {
                SSNErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate First Name
            if (string.IsNullOrWhiteSpace(PatientFirstNameTextBox.Text))
            {
                FirstNameErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(PatientLastNameTextBox.Text))
            {
                LastNameErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Date of Birth selection
            if (DOBDatePicker.SelectedDate == null)
            {
                DOBErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Gender selection
            if (GenderComboBox.SelectedItem == null)
            {
                GenderErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Street Address
            if (string.IsNullOrWhiteSpace(StreetAddressTextBox.Text))
            {
                StreetAddressErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate City
            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                CityErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Zip Code (example: US 5-digit code)
            if (!Regex.IsMatch(ZipCodeTextBox.Text, @"^\d{5}$"))
            {
                ZipCodeErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate State selection
            if (StateComboBox.SelectedItem == null)
            {
                StateErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Country selection
            if (CountryComboBox.SelectedItem == null)
            {
                CountryErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Phone Number (example: US 10-digit number)
            if (!Regex.IsMatch(PhoneNumberTextBox.Text, @"^\d{10}$"))
            {
                PhoneErrorTextBlock.Visibility = Visibility.Visible;
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

                var mailingAddress = new MailingAddress
                    (
                        address, zipcode, city, state, country
                    );

                var patientInfo = new Patient
                (
                    ssn, gender, firstName, lastName, dob, mailingAddress, phoneNumber
                );

                var dal = new PatientDal();
                dal.RegisterPatient(patientInfo);
                // Example output (logging or further action)
                System.Diagnostics.Debug.WriteLine("Patient Registered: " + patientInfo);
                this.LoadPatients();

                foreach (var patient in this.patients)
                {
                    if (patient.PersonId == patientInfo.PersonId)
                    {
                        this.PatientListView.SelectedItem = patient;
                        break;
                    }
                }
            }
        }


        // Edit patient in the database
        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            if (this.PatientListView.SelectedItem is Patient selectedPatient)
            {
                // Clear previous error messages
                this.SSNErrorTextBlock.Visibility = Visibility.Collapsed;
                this.GenderErrorTextBlock.Visibility = Visibility.Collapsed;
                this.ZipCodeErrorTextBlock.Visibility = Visibility.Collapsed;
                this.PhoneErrorTextBlock.Visibility = Visibility.Collapsed;
                this.StateErrorTextBlock.Visibility = Visibility.Collapsed;

                // Validate each field
                bool isValid = true;

                // Validate SSN (example: 9 digits)
                if (!Regex.IsMatch(this.PatientSSNTextBox.Text, @"^\d{9}$"))
                {
                    this.SSNErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Gender selection
                if (this.GenderComboBox.SelectedItem == null)
                {
                    this.GenderErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Zip Code (example: US 5-digit code)
                if (!Regex.IsMatch(ZipCodeTextBox.Text, @"^\d{5}$"))
                {
                    this.ZipCodeErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate Phone Number (example: US 10-digit number)
                if (!Regex.IsMatch(this.PhoneNumberTextBox.Text, @"^\d{10}$"))
                {
                    this.PhoneErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate State selection
                if (this.StateComboBox.SelectedItem == null)
                {
                    this.StateErrorTextBlock.Visibility = Visibility.Visible;
                    isValid = false;
                }

                // Validate State selection
                if (this.CountryComboBox.SelectedItem == null)
                {
                    this.CountryErrorTextBlock.Visibility = Visibility.Visible;
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
                    var phoneNumber = this.PhoneNumberTextBox.Text;
                    var ssn = this.PatientSSNTextBox.Text;
                    var gender = (this.GenderComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    var firstName = this.PatientFirstNameTextBox.Text;
                    var lastName = this.PatientLastNameTextBox.Text;
                    var dob = this.DOBDatePicker.Date.DateTime;
                    var address = this.StreetAddressTextBox.Text;
                    var zipcode = this.ZipCodeTextBox.Text;
                    var city = this.CityTextBox.Text;
                    var country = (this.CountryComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    var state = (this.StateComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                    var mailingAddress = new MailingAddress
                        (
                            address, zipcode, city, state, country
                        );

                    var patientInfo = new Patient
                    (
                        patientId, personId, phoneNumber, ssn, gender, firstName, lastName, dob, mailingAddress
                    );

                    var dal = new PatientDal();
                    dal.UpdatePatientInDatabase(patientInfo);
                    // Example output (logging or further action)
                    Debug.WriteLine("Patient Registered: " + patientInfo);
                    this.LoadPatients();
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
