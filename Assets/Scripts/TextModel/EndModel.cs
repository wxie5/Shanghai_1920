[System.Serializable]
public class EndModel : IBaseTextModel
{
    public string command;
    public string targetName;
    public ModelType type;

    public EndModel(string command, string targetName)
    {
        this.command = command;
        this.targetName = targetName;
        this.type = ModelType.End;
    }
    
}
