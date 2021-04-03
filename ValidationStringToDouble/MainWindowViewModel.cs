using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ValidationStringToDouble
{
    public class MainWindowViewModel : ValidatingViewModelBase
    {
        private double _numero; public double Numero { get => _numero; set { Set(() => Numero, ref _numero, value); } }
        private string _text="Insert a number";
        public string Text
        {
            get => _text;
            set
            {
                Set(() => Text, ref _text, value);
                tryUpdateNumero();
            }
        }

        private void tryUpdateNumero()
        {
            DoubleStringValidatingConverter validatingConverter = new DoubleStringValidatingConverter();
            ValidationResult validationResults = validatingConverter.Validate(Text);
            if (validationResults.IsValid)
            {
                Numero = validatingConverter.ConvertedValue;
                RaisePropertyChanged(nameof(Numero));
            }
            SetErrorsForProperty(nameof(Text), validationResults);
        }

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(MainWindowModel mainWindowModel)
        {
            Numero = mainWindowModel.Numero;
        }
    }
}
