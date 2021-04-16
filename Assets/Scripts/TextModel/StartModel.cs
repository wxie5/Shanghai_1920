[System.Serializable]
public class StartModel : IBaseTextModel
{
    public string sceneName;
    public ModelType type;

    public StartModel(string sceneName)
    {
        this.sceneName = sceneName;
        this.type = ModelType.Start;
    }
}
