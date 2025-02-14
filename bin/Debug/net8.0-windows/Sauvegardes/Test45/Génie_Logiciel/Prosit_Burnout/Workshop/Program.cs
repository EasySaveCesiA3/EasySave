using System;
using System.ComponentModel;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Test MVC ===");
        new CounterMVC().RunMVC();

        Console.WriteLine("\n=== Test MVVM ===");
        new CounterMVVM().RunMVVM();

        Console.ReadKey();
    }
}

// ============ PATTERN MVC ============
public class CounterMVC
{
    // MODEL : Contient juste la donnée
    public class CounterModel
    {
        public int Value { get; set; }
    }

    // VIEW : Affiche la donnée
    public class CounterView
    {
        public void Display(int value)
        {
            Console.WriteLine($"Compteur: {value}");
        }
    }

    // CONTROLLER : Gère la logique
    public class CounterController
    {
        private CounterModel _model = new();
        private CounterView _view = new();

        public void Increment()
        {
            // 1. Met à jour le model
            _model.Value++;
            // 2. Dit à la vue d'afficher
            _view.Display(_model.Value);
        }
    }

    public void RunMVC()
    {
        var controller = new CounterController();
        // Incrémente 3 fois
        controller.Increment();
        controller.Increment();
        controller.Increment();
    }
}

// ============ PATTERN MVVM ============
public class CounterMVVM
{
    // MODEL : Identique au MVC
    public class CounterModel
    {
        public int Value { get; set; }
    }

    // VIEWMODEL : Gère la logique et notifie des changements
    public class CounterViewModel : INotifyPropertyChanged
    {
        private CounterModel _model = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Increment()
        {
            // 1. Met à jour le model
            _model.Value++;
            // 2. Notifie du changement
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            // Pour l'exemple, on simule l'affichage
            Display();
        }

        // Méthode temporaire pour simuler l'affichage
        private void Display()
        {
            Console.WriteLine($"Compteur: {_model.Value}");
        }
    }

    public void RunMVVM()
    {
        var viewModel = new CounterViewModel();
        // Incrémente 3 fois
        viewModel.Increment();
        viewModel.Increment();
        viewModel.Increment();
    }
}