[System.Serializable]
public class DiaModel : IBaseTextModel
{
    public string name;
    public string text;
    public string audio;
    public string expression;
    public bool isLightning;
    public bool isShaking;
    public int pos;
    public string background;
    public string bgm;
    public ModelType type;

    public DiaModel(string name, string text, string expression, string audio, bool isShaking, int pos, bool isLightning, string background, string bgm)
    {
        this.name = name;
        this.text = text;
        this.expression = expression;
        this.audio = audio;
        this.isLightning = isLightning;
        this.isShaking = isShaking;
        this.pos = pos;
        this.background = background;
        this.bgm = bgm;
        this.type = ModelType.Dia;
    }
}
