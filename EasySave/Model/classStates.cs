using System.ComponentModel;
using System.Windows;

namespace Model;

public class StateManager
{
    public class BackupProgress : INotifyPropertyChanged
{
    private long _progress;
    private long _total;

    public required string Name { get; set; }
    public long Progress
    {
        get => _progress;
        set
        {
            if (_progress != value)
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressPer)); // Recalcul automatique
            }
        }
    }

    public long Total
    {
        get => _total;
        set
        {
            if (_total != value)
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(ProgressPer)); // Recalcul automatique
            }
        }
    }

    public long ProgressPer
    {
        get
        {
            if (Total == 0) return 0; // Évite la division par zéro
            return Progress * 100 / Total; // Calcul du pourcentage
        }
    }

    public required string Action { get; set; }
    public required string State { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}





    internal class BackupMetadata
    {
        public bool Crypte { get; set; }
        public string Date { get; set; }
        public string[] ExtensionsCryptees { get; set; }
    }
}