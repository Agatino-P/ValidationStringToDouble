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
    public class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private double _numero; public double Numero { get => _numero; set { Set(() => Numero, ref _numero, value); } }

        private List<string> _allErrors = new List<string>();
        public List<string>  AllErrors { get => _allErrors; set { Set(() => AllErrors, ref _allErrors, value); }}
        

        public Dictionary<string, List<string>> Errors { get; private set; } = new Dictionary<string, List<string>>();

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                Set(() => Text, ref _text, value);
                tryUpdateNumero();
            }
        }

        public bool HasErrors => Errors.Count > 0;

        private void tryUpdateNumero()
        {
            DoubleStringValidatingConverter validatingConverter = new DoubleStringValidatingConverter();
            ValidationResult validationResults = validatingConverter.Validate(Text);
            if (validationResults.IsValid)
            {
                Numero = validatingConverter.ConvertedValue;
                RaisePropertyChanged(nameof(Numero));
                Errors.Clear();
                RaiseErrorChangedForProperty(nameof(Text));
            }
            else
            {
                if (Errors.ContainsKey(nameof(Text)))
                {
                    Errors.Remove(nameof(Text));
                }
                Errors.Add(nameof(Text), validationResults.Errors.Select(e=>e.ErrorMessage).ToList());
                RaiseErrorChangedForProperty(nameof(Text));
            }
        }


        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return AllErrors;

            if (Errors.ContainsKey(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void RaiseErrorChangedForProperty(string propertyName)
        {
            AllErrors=new List<string>(Errors.SelectMany(kvp => kvp.Value.Select(err => $"{kvp.Key}:{err}")));
            RaisePropertyChanged(nameof(AllErrors));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            Text = "Text";
        }

        public MainWindowViewModel(MainWindowModel mainWindowModel)
        {
            Numero = mainWindowModel.Numero;
        }
    }
}
