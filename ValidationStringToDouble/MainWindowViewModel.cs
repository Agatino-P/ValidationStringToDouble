using FluentValidation.Results;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace ValidationStringToDouble
{
    public class MainWindowViewModel : ViewModelBase
    {
        private double _numero; public double Numero { get => _numero; set { Set(() => Numero, ref _numero, value); }}

        public ObservableCollection<string> Errors { get; private set; } = new ObservableCollection<string>();

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

        private void tryUpdateNumero()
        {
            DoubleStringValidatingConverter validatingConverter = new DoubleStringValidatingConverter();
            ValidationResult validationResults = validatingConverter.Validate(Text);
            if (validationResults.IsValid)
            {
                Numero = validatingConverter.ConvertedValue;
                RaisePropertyChanged(nameof(Numero));
                Errors.Clear();
            }
            else
            {
                Errors.Clear();
                foreach (ValidationFailure error in validationResults.Errors)
                {
                    Errors.Add(error.ErrorMessage);
                }
            }
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
