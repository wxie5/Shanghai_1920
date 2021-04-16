
[System.Serializable]
public class CharMoveModel : IBaseTextModel
{
    public bool isCome;
    public int pos;
    public string charName;
    public string audio;
    public string background;
    public string bgm;
    public ModelType type;

    public CharMoveModel(bool isCome, int pos, string charName, string audio, string background, string bgm)
    {
        this.isCome = isCome;
        this.pos = pos;
        this.charName = charName;
        this.audio = audio;
        this.background = background;
        this.bgm = bgm;
        this.type = ModelType.CharMove;
    }

}
