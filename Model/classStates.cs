namespace Model;

public class StateManager
{
    public void generateFinalReport()
    {
        Console.WriteLine("ouiIIIIII");
    }
    public void updateStateFile()
    {
        Console.WriteLine("ouiIIIIII");
    }
    public JsonState loadStateFile()
    {
        //Temp pour pas avoir d'erreur sur l'IDE
        Console.WriteLine("ouiIIIIII");
        return new JsonState();
    }
    
}

public class JsonState()
{
    private string backupName { get;set; }
    private string lastUpdateTime { get;set; }
    private int progress { get;set; }
    private int filesRemaining { get;set; }
    private long totalSizeReamining { get;set; }
    
    public void saveState()
    {
        Console.WriteLine("ouiIIIIII");
    }
    public void loadState()
    {
        Console.WriteLine("ouiIIIIII");
    }
}