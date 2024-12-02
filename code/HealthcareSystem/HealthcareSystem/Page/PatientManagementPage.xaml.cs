using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DBAccess.DAL;
using DBAccess.Model;
using HealthcareSystem.DAL;
using HealthcareSystem.Model;
using HealthcareSystem.Page;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents a page for managing patient records, including adding, editing, and displaying patient details.
    /// </summary>
    public sealed partial class PatientManagementPage : BasePage
    {
        #region Data members

        private Patient patient;

        private List<Patient> patients;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatientManagementPage" /> class and loads patient records.
        /// </summary>
        public PatientManagementPage()
        {
            this.InitializeComponent();
            this.LoadPatients();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatientManagementPage" /> class with a selected patient.
        /// </summary>
        /// <param name="selectedPatient">The patient whose details are preloaded into the form.</param>
        public PatientManagementPage(Patient selectedPatient)
        {
            this.InitializeComponent();
            this.patient = selectedPatient;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles navigation to this page, preselecting a patient and populating their details if a parameter is passed.
        /// </summary>
        /// <param name="e">Navigation event arguments containing the selected patient parameter.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Patient selectedPatient)
            {
                this.patient = selectedPatient;
                var matchingPatient = this.PatientListView.Items?.Cast<Patient>()
                    .FirstOrDefault(p => p.PatientId == selectedPatient.PatientId);
                this.PatientListView.SelectedItem = matchingPatient;

                this.PopulatePatientFields(this.patient);
            }
        }

        /// <summary>
        ///     Loads all patient records from the database and populates the patient list view.
        /// </summary>
        private void LoadPatients()
        {
            Debug.WriteLine("Loading Patients From Database");
            this.patients = this.GetPatientsFromDatabase();

            this.PatientListView.ItemsSource = this.patients;
        }

        /// <summary>
        ///     Retrieves the list of patients from the database.
        /// </summary>
        /// <returns>A list of <see cref="Patient" /> objects retrieved from the database.</returns>
        private List<Patient> GetPatientsFromDatabase()
        {
            var dal = new PatientDal();
            return dal.GetPatientsFromReader();
        }

        /// <summary>
        ///     Handles selection changes in the patient list view and populates the form with the selected patient's details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing information about the selection change.</param>
        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PatientListView.SelectedItem is Patient selectedPatient)
            {
                this.PopulatePatientFields(selectedPatient);
            }
        }

        /// <summary>
        ///     Populates the form fields with the details of the specified patient.
        /// </summary>
        /// <param name="selectedPatient">The patient whose details are displayed in the form.</param>
        private void PopulatePatientFields(Patient selectedPatient)
        {
            this.PatientSSNTextBox.Text = selectedPatient.SSN;
            this.PatientFirstNameTextBox.Text = selectedPatient.FirstName;
            this.PatientLastNameTextBox.Text = selectedPatient.LastName;
            this.DOBDatePicker.Date = selectedPatient.DateOfBirth;
            this.StreetAddressTextBox.Text = selectedPatient.MailAddress?.StreetAddress ?? string.Empty;
            this.ZipCodeTextBox.Text = selectedPatient.MailAddress?.Zip ?? string.Empty;
            this.CityTextBox.Text = selectedPatient.MailAddress?.City ?? string.Empty;
            this.PhoneNumberTextBox.Text = selectedPatient.PhoneNumber;

            this.GenderComboBox.SelectedItem = this.GenderComboBox.Items?
                .FirstOrDefault(item => (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.Gender);

            if (selectedPatient.MailAddress != null)
            {
                this.StateComboBox.SelectedItem = this.StateComboBox.Items?
                    .FirstOrDefault(item =>
                        (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.State);

                this.CountryComboBox.SelectedItem = this.CountryComboBox.Items?
                    .FirstOrDefault(item =>
                        (item as ComboBoxItem)?.Content?.ToString() == selectedPatient.MailAddress.Country);
            }
        }

        /// <summary>
        ///     Handles the "Register Patient" button click event to register a new patient in the database.
        ///     Validates inputs and reloads the patient list upon successful registration.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RegisterPatient_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateInputs())
            {
                var ssn = this.PatientSSNTextBox.Text;
                var gender = (this.GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var firstName = this.PatientFirstNameTextBox.Text;
                var lastName = this.PatientLastNameTextBox.Text;
                var dob = this.DOBDatePicker.Date.DateTime;
                var address = this.StreetAddressTextBox.Text;
                var zipcode = this.ZipCodeTextBox.Text;
                var city = this.CityTextBox.Text;
                var country = (this.CountryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var state = (this.StateComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
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

        /// <summary>
        ///     Handles the "Edit Patient" button click event to update the details of an existing patient.
        ///     Validates inputs and reloads the patient list upon successful update.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
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
                    var gender = (this.GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var firstName = this.PatientFirstNameTextBox.Text;
                    var lastName = this.PatientLastNameTextBox.Text;
                    var dob = this.DOBDatePicker.Date.DateTime;
                    var address = this.StreetAddressTextBox.Text;
                    var zipcode = this.ZipCodeTextBox.Text;
                    var city = this.CityTextBox.Text;
                    var country = (this.CountryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var state = (this.StateComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

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

        /// <summary>
        ///     Validates the form inputs for registering or editing a patient.
        ///     Highlights errors if validation fails.
        /// </summary>
        /// <returns><c>true</c> if all inputs are valid; otherwise, <c>false</c>.</returns>
        private bool ValidateInputs()
        {
            var isValid = true;

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
                this.CountryErrorTextBlock.Text =
                    "Country selection is required. Please choose a country from the list.";
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

        /// <summary>
        ///     Sets the visibility of an error message for a specific field.
        /// </summary>
        /// <param name="errorTextBlock">The <see cref="TextBlock" /> containing the error message.</param>
        /// <param name="isVisible">Indicates whether the error message should be visible.</param>
        private void SetErrorVisibility(TextBlock errorTextBlock, bool isVisible)
        {
            errorTextBlock.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}