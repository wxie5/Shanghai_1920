[System.Serializable]
public class TriggerModel : IBaseTextModel
{
    public string command;
    public string item;
    public int value;
    public ModelType type;

    public TriggerModel(string command, string item, int value)
    {
        this.command = command;
        this.item = item;
        this.value = value;
        this.type = ModelType.Trigger;
    }
}
