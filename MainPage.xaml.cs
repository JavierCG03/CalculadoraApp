using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace CalculadoraApp
{
    public partial class MainPage : ContentPage
    {
        private string _currentInput = string.Empty;
        private double _firstNumber = 0;
        private double _secondNumber = 0;
        private double _memory = 0;
        private string _currentOperator = string.Empty;
        private bool _isNewOperation = true;
        private bool _isOperatorClicked = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void UpdateDisplay()
        {
            ResultLabel.Text = _currentInput;
        }

        private void OnNumberButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string digit = button.Text;

            // Si estamos empezando un nuevo número o después de un operador
            if (_isNewOperation || _isOperatorClicked)
            {
                // Si el primer dígito es cero, no permitirlo a menos que sea el único dígito
                if (digit == "0")
                {
                    _currentInput = "0";
                }
                else
                {
                    // Cualquier otro dígito empieza normalmente
                    _currentInput = digit;
                }
                _isNewOperation = false;
                _isOperatorClicked = false;
            }
            else
            {
                // Estamos continuando con la entrada de un número

                // Si el input actual es solamente "0"
                if (_currentInput == "0")
                {
                    // Si el nuevo dígito no es cero, reemplazar el cero
                    if (digit != "0")
                    {
                        _currentInput = digit;
                    }
                    // Si el dígito es cero de nuevo, mantener solo un cero
                    // (esto no hace nada porque _currentInput ya es "0")
                }
                else
                {
                    // En cualquier otro caso, agregar el dígito normalmente
                    _currentInput += digit;
                }
            }

            UpdateDisplay();
        }

        private void OnDecimalPointButtonClicked(object sender, EventArgs e)
        {
            if (_isNewOperation || _isOperatorClicked)
            {
                _currentInput = "0.";
                _isNewOperation = false;
                _isOperatorClicked = false;
            }
            else if (!_currentInput.Contains('.'))
            {
                _currentInput += ".";
            }

            UpdateDisplay();
        }

        private void OnOperatorButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (!string.IsNullOrEmpty(_currentInput))
            {
                _firstNumber = double.Parse(_currentInput);
            }

            switch (button.Text)
            {
                case "+":
                    _currentOperator = "+";
                    break;
                case "-":
                    _currentOperator = "-";
                    break;
                case "×":
                    _currentOperator = "*";
                    break;
                case "÷":
                    _currentOperator = "/";
                    break;
            }

            HistoryLabel.Text = $"{_firstNumber} {button.Text}";
            _isOperatorClicked = true;
        }

        private void OnEqualsButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput) && !string.IsNullOrEmpty(_currentOperator))
            {
                _secondNumber = double.Parse(_currentInput);
                double result = 0;

                switch (_currentOperator)
                {
                    case "+":
                        result = _firstNumber + _secondNumber;
                        break;
                    case "-":
                        result = _firstNumber - _secondNumber;
                        break;
                    case "*":
                        result = _firstNumber * _secondNumber;
                        break;
                    case "/":
                        if (_secondNumber != 0)
                        {
                            result = _firstNumber / _secondNumber;
                        }
                        else
                        {
                            DisplayAlert("Error", "No se puede dividir por cero", "OK");
                            OnClearButtonClicked(sender, e);
                            return;
                        }
                        break;
                    case "^":
                        result = Math.Pow(_firstNumber, _secondNumber);
                        break;
                }

                HistoryLabel.Text = $"{_firstNumber} {GetOperatorSymbol(_currentOperator)} {_secondNumber} =";
                _currentInput = result.ToString();
                UpdateDisplay();
                _isNewOperation = true;
            }
        }

        private string GetOperatorSymbol(string op)
        {
            return op switch
            {
                "+" => "+",
                "-" => "-",
                "*" => "×",
                "/" => "÷",
                "^" => "^",
                _ => op
            };
        }

        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            _currentInput = "0";
            _firstNumber = 0;
            _secondNumber = 0;
            _currentOperator = string.Empty;
            _isNewOperation = true;
            HistoryLabel.Text = string.Empty;
            UpdateDisplay();
        }

        private void OnPercentButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double currentValue = double.Parse(_currentInput);

                if (string.IsNullOrEmpty(_currentOperator))
                {
                    // Si no hay operador previo, simplemente divide por 100
                    _currentInput = (currentValue / 100).ToString();
                }
                else
                {
                    // Si hay un operador, calcula el porcentaje del primer número
                    _currentInput = (_firstNumber * currentValue / 100).ToString();
                }

                UpdateDisplay();
            }
        }

        private void OnPowerButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                _firstNumber = double.Parse(_currentInput);
                _currentOperator = "^";
                HistoryLabel.Text = $"{_firstNumber} ^";
                _isOperatorClicked = true;
            }
        }

        private void OnInverseButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double currentValue = double.Parse(_currentInput);

                if (currentValue != 0)
                {
                    _currentInput = (1 / currentValue).ToString();
                    UpdateDisplay();
                }
                else
                {
                    DisplayAlert("Error", "No se puede dividir por cero", "OK");
                }
            }
        }

        private void OnSquareRootButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double currentValue = double.Parse(_currentInput);

                if (currentValue >= 0)
                {
                    _currentInput = Math.Sqrt(currentValue).ToString();
                    UpdateDisplay();
                }
                else
                {
                    DisplayAlert("Error", "No se puede calcular la raíz cuadrada de un número negativo", "OK");
                }
            }
        }

        private void OnSignChangeButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput) && _currentInput != "0")
            {
                if (_currentInput.StartsWith("-"))
                {
                    _currentInput = _currentInput.Substring(1);
                }
                else
                {
                    _currentInput = "-" + _currentInput;
                }

                UpdateDisplay();
            }
        }

        private void OnMemoryButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                // Si ya hay un valor en memoria, preguntamos qué acción realizar
                if (_memory != 0)
                {
                    DisplayActionSheet("Opciones de Memoria", "Cancelar", null,
                        "Guardar Nuevo Valor (M+)",
                        "Recuperar Valor (MR)",
                        "Borrar Memoria (MC)")
                    .ContinueWith(t =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            string result = t.Result;
                            switch (result)
                            {
                                case "Guardar Nuevo Valor (M+)":
                                    _memory = double.Parse(_currentInput);
                                    DisplayAlert("Memoria", $"Valor {_memory} guardado en memoria", "OK");
                                    break;
                                case "Recuperar Valor (MR)":
                                    _currentInput = _memory.ToString();
                                    UpdateDisplay();
                                    break;
                                case "Borrar Memoria (MC)":
                                    _memory = 0;
                                    DisplayAlert("Memoria", "Memoria borrada", "OK");
                                    break;
                            }
                        });
                    });
                }
                else
                {
                    // Si no hay valor en memoria, guardamos el actual
                    _memory = double.Parse(_currentInput);
                    DisplayAlert("Memoria", $"Valor {_memory} guardado en memoria", "OK");
                }
            }
            else if (_memory != 0)
            {
                // Si no hay input actual pero hay memoria, recuperamos el valor
                _currentInput = _memory.ToString();
                UpdateDisplay();
            }
        }
    }
}