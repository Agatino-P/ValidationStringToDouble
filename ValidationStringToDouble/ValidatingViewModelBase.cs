using FluentValidation.Results;
using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ValidationStringToDouble
{
    public class ValidatingViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private ObservableCollection<string> _allErrors;
        public ObservableCollection<string> AllErrors
        {
            get => _allErrors; set { Set(() => AllErrors, ref _allErrors, value); }
        }

        public Dictionary<string, List<string>> Errors { get; private set; } = new Dictionary<string, List<string>>();


        public bool HasErrors => Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) { return AllErrors; }

            if (Errors.ContainsKey(propertyName)) { return Errors[propertyName]; }

            return Enumerable.Empty<string>();
        }

        protected void SetErrorsForProperty(string propertyName, ValidationResult validationResult)
        {
            SetErrorsForProperty(
                propertyName,
                validationResult.IsValid ? new List<string>() : validationResult.Errors.Select(vr => vr.ErrorMessage).ToList()
                );
        }

        protected void SetErrorsForProperty(string propertyName, List<string> newErrors)
        {
            if (!updateErrorsIfNeeded(propertyName, newErrors)) { return; }
            populateAllErrors();
            raiseErrorChangedForProperty(propertyName);
        }

        private bool updateErrorsIfNeeded(string propertyName, List<string> newErrors)
        {
            if (!Errors.ContainsKey(propertyName))
            {
                if (newErrors == null || newErrors.Count < 1)
                {
                    return false;
                }
                Errors.Add(propertyName, newErrors);
                return true;
            }
            if (Errors[propertyName].SequenceEqual(newErrors))
            {
                return false;
            }

            Errors[propertyName] = newErrors;
            return true; ;
        }

        private void raiseErrorChangedForProperty(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private void populateAllErrors()
        {
            
            AllErrors = new ObservableCollection<string>(Errors.SelectMany(kvp => kvp.Value.Select(err => $"{kvp.Key}: {err}")));
            //RaisePropertyChanged(nameof(AllErrors));
        }
    }
}
