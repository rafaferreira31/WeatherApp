﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeatherApp.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMsg = "Please, provide a name.";
        private const string InvalidNameErrorMsg = "Please, provide a valid name.";
        private const string EmptyEmailErrorMsg = "Please, provide a email.";
        private const string InvalidEmailErrorMsg = "Please, provide a valid email.";
        private const string EmptyPhoneErrorMsg = "Please, provide a phone number.";
        private const string InvalidPhoneErrorMsg = "Please, provide a valid phone number.";
        private const string EmptyPasswordErrorMsg = "Please, provide a password.";
        private const string InvalidPasswordErrorMsg = "The password must contain at least 8 characters, including letters and numbers";

        public Task<bool> Validate(string name, string email, string phone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email); 
            var isPhoneValid = ValidatePhone(phone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3) 
            { 
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }


        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                PhoneError = EmptyPhoneErrorMsg;
                return false;
            }

            if (phone.Length < 9)
            {
                PhoneError = InvalidPhoneErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }


        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
