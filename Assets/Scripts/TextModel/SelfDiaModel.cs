[System.Serializable]
public class SelfDiaModel : IBaseTextModel
{
    public bool isSelf;
    public string text;
    public string audio;
    public bool isLightning;
    public string background;
    public string bgm;
    public ModelType type;

    public SelfDiaModel(bool isSelf, string text, string audio, bool isLightning, string background, string bgm)
    {
        this.isSelf = isSelf;
        this.text = text;
        this.audio = audio;
        this.isLightning = isLightning;
        this.background = background;
        this.bgm = bgm;
        this.type = ModelType.SelfDia;
    }
}
