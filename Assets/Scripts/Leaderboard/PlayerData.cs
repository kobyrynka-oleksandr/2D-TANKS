[System.Serializable]
public class PlayerData
{
    public string Name;
    public int Stage;

    public PlayerData(string name, int stage)
    {
        Name = name;
        Stage = stage;
    }
}