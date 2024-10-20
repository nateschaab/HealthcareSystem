using DBAccess.DAL;
using HealthcareSystem.Model;
using System.IO.Ports;
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
        public PatientManagementPage()
        {
            this.InitializeComponent();
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
            }
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
