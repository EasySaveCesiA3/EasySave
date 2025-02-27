using System.ComponentModel;

namespace Model;

public class StateManager
{
    public class BackupProgress : INotifyPropertyChanged
    {
        private long _progress;
        private long _total;
        private long _progressPer;

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

        public required string Action { get; set; }
        public required string State { get; set; }

        public long ProgressPer
        {
            get => Total > 0 ? (long)((double)Progress / Total * 100) : 0;
            set
            {
                if (_progressPer != value)
                {
                    _progressPer = value;
                    OnPropertyChanged(nameof(ProgressPer));
                }
            }
        }

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