namespace Model;

public class StateManager
{
    public class BackupProgress()
    {
        public required string Name { get; set; }
        public long Progress { get; set; }
        public long Total { get; set; }
        public required string Action { get; set; }
        public required string State { get; set; }
    }

    internal class BackupMetadata
    {
        public bool Crypte { get; set; }
        public string Date { get; set; }
        public string[] ExtensionsCryptees { get; set; }
    }
}