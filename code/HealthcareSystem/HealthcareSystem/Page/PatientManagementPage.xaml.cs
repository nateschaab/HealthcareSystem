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

        public PatientManagementPage()
        {
            this.InitializeComponent();
            this.LoadPatients();
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
                var matchingPatient = this.PatientListView.Items.Cast<Patient>()
                    .FirstOrDefault(p => p.PatientId == selectedPatient.PatientId); // assuming Patient has an Id property
                this.PatientListView.SelectedItem = matchingPatient;

                this.PopulatePatientFields(this.patient);
            }
        }

        // Load all patients from the database and populate the ListView
        private void LoadPatients()
        {
            Debug.WriteLine("Loading Patients From Database");
            this.patients = this.GetPatientsFromDatabase();  // Retrieve patients from the DB

            // Bind the patient list to the ListView
            this.PatientListView.ItemsSource = this.patients;
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
            if (this.PatientListView.SelectedItem is Patient selectedPatient)
            {
                this.PopulatePatientFields(selectedPatient);
            }
        }

        private void PopulatePatientFields(Patient selectedPatient)
        {
            // Load the selected patient's data into the input fields
            this.PatientSSNTextBox.Text = selectedPatient.SSN;
            this.PatientFirstNameTextBox.Text = selectedPatient.FirstName;
            this.PatientLastNameTextBox.Text = selectedPatient.LastName;
            this.DOBDatePicker.Date = selectedPatient.DateOfBirth;
            this.StreetAddressTextBox.Text = selectedPatient.MailAddress?.StreetAddress ?? string.Empty;
            this.ZipCodeTextBox.Text = selectedPatient.MailAddress?.Zip ?? string.Empty;
            this.CityTextBox.Text = selectedPatient.MailAddress?.City ?? string.Empty;
            this.PhoneNumberTextBox.Text = selectedPatient.PhoneNumber;

            // Set ComboBox value for Gender
            this.GenderComboBox.SelectedItem = this.GenderComboBox.Items
                .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.Gender);

            // Check if MailAddress is not null before setting State and Country
            if (selectedPatient.MailAddress != null)
            {
                this.StateComboBox.SelectedItem = this.StateComboBox.Items
                    .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.State);

                this.CountryComboBox.SelectedItem = this.CountryComboBox.Items
                    .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.Country);
            }
        }

        private void RegisterPatient_Click(object sender, RoutedEventArgs e)
        {

            // If all fields are valid, proceed to register the patient
            if (this.ValidateInputs())
            {
                // Code to register the patient
                // Example: Saving to database or calling an API
                // Use PatientSSNTextBox.Text, GenderComboBox.SelectedItem, etc.
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
                var phoneNumber = this.PhoneNumberTextBox.Text;

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

                Debug.WriteLine("Patient Registered: " + patientInfo);
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
                if (this.ValidateInputs())
                {
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

                    var mailingAddress = new MailingAddress(address, zipcode, city, state, country);

                    var patientInfo = new Patient
                    (
                        patientId, personId, phoneNumber, ssn, gender, firstName, lastName, dob, mailingAddress
                    );

                    var dal = new PatientDal();
                    dal.UpdatePatientInDatabase(patientInfo);

                    Debug.WriteLine("Patient Registered: " + patientInfo);
                    this.LoadPatients();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private bool ValidateInputs()
        {
            bool isValid = true;

            if (!Regex.IsMatch(this.PatientSSNTextBox.Text, @"^\d{9}$"))
            {
                this.SSNErrorTextBlock.Text = "SSN must be exactly 9 digits (e.g., 123456789).";
                this.SetErrorVisibility(this.SSNErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.SSNErrorTextBlock, false);
            }

            if (string.IsNullOrWhiteSpace(this.PatientFirstNameTextBox.Text))
            {
                this.FirstNameErrorTextBlock.Text = "First name is required. Please enter a valid first name.";
                this.SetErrorVisibility(this.FirstNameErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.FirstNameErrorTextBlock, false);
            }

            if (string.IsNullOrWhiteSpace(this.PatientLastNameTextBox.Text))
            {
                this.LastNameErrorTextBlock.Text = "Last name is required. Please enter a valid last name.";
                this.SetErrorVisibility(this.LastNameErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.LastNameErrorTextBlock, false);
            }

            if (this.DOBDatePicker.SelectedDate == null)
            {
                this.DOBErrorTextBlock.Text = "Date of Birth is required. Please select a valid date.";
                this.SetErrorVisibility(this.DOBErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.DOBErrorTextBlock, false);
            }

            if (this.GenderComboBox.SelectedItem == null)
            {
                this.GenderErrorTextBlock.Text = "Gender is required. Please choose a gender.";
                this.SetErrorVisibility(this.GenderErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.GenderErrorTextBlock, false);
            }

            if (string.IsNullOrWhiteSpace(this.StreetAddressTextBox.Text))
            {
                this.StreetAddressErrorTextBlock.Text = "Street address is required. Please enter a valid address.";
                this.SetErrorVisibility(this.StreetAddressErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.StreetAddressErrorTextBlock, false);
            }

            if (!Regex.IsMatch(this.ZipCodeTextBox.Text, @"^\d{5}$"))
            {
                this.ZipCodeErrorTextBlock.Text = "Zip code must be exactly 5 digits (e.g., 12345).";
                this.SetErrorVisibility(this.ZipCodeErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.ZipCodeErrorTextBlock, false);
            }

            if (string.IsNullOrWhiteSpace(this.CityTextBox.Text))
            {
                this.CityErrorTextBlock.Text = "City is required. Please enter a valid city name.";
                this.SetErrorVisibility(this.CityErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.CityErrorTextBlock, false);
            }

            if (this.StateComboBox.SelectedItem == null)
            {
                this.StateErrorTextBlock.Text = "State selection is required. Please choose a state from the list.";
                this.SetErrorVisibility(this.StateErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.StateErrorTextBlock, false);
            }

            if (this.CountryComboBox.SelectedItem == null)
            {
                this.CountryErrorTextBlock.Text = "Country selection is required. Please choose a country from the list.";
                this.SetErrorVisibility(this.CountryErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.CountryErrorTextBlock, false);
            }

            if (!Regex.IsMatch(this.PhoneNumberTextBox.Text, @"^\d{10}$"))
            {
                this.PhoneErrorTextBlock.Text = "Phone number must be exactly 10 digits (e.g., 1234567890).";
                this.SetErrorVisibility(this.PhoneErrorTextBlock, true);
                isValid = false;
            }
            else
            {
                this.SetErrorVisibility(this.PhoneErrorTextBlock, false);
            }

            return isValid;
        }


        private void SetErrorVisibility(TextBlock errorTextBlock, bool isVisible)
        {
            errorTextBlock.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
