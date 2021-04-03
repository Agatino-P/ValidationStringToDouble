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
        private ObservableCollection<string> _allErrors = new ObservableCollection<string>();
        public ObservableCollection<string> AllErrors { get => _allErrors; set { Set(() => AllErrors, ref _allErrors, value); } }

        private Dictionary<string, List<string>> _errorsDict { get; set; } = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsDict.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) { return AllErrors; }
            if (_errorsDict.ContainsKey(propertyName)) { return _errorsDict[propertyName]; }
            return Enumerable.Empty<string>();
        }

        protected void SetErrorsForProperty(string propertyName, ValidationResult validationResult)
        {
            SetErrorsForProperty(propertyName,
                validationResult.IsValid ? new List<string>() : validationResult.Errors.Select(vr => vr.ErrorMessage).ToList());
        }

        protected void SetErrorsForProperty(string propertyName, List<string> newErrors)
        {
            if (!updateErrorsIfNeeded(propertyName, newErrors)) { return; }
            populateAllErrors();
            raiseErrorChangedForProperty(propertyName);
        }

        private bool updateErrorsIfNeeded(string propertyName, List<string> newErrors)
        {
            if (!_errorsDict.ContainsKey(propertyName))
            {
                if (newErrors == null || newErrors.Count < 1) {return false;}
            
                _errorsDict.Add(propertyName, newErrors);
                return true;
            }
            
            if (_errorsDict[propertyName].SequenceEqual(newErrors)){return false;}

            _errorsDict[propertyName] = newErrors;
            return true; ;
        }

        private void raiseErrorChangedForProperty(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        
        private void populateAllErrors() => AllErrors = new ObservableCollection<string>(_errorsDict.SelectMany(kvp => kvp.Value.Select(err => $"{kvp.Key}: {err}")));
        
    }
}
