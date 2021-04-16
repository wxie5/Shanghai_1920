public class AnimModel : IBaseTextModel
{
    public string animName;
    public string audio;
    public bool isLightning;
    public string background;
    public string bgm;
    public ModelType type;

    public AnimModel(string animName, string audio, bool isLightning, string background, string bgm)
    {
        this.animName = animName;
        this.audio = audio;
        this.isLightning = isLightning;
        this.background = background;
        this.bgm = bgm;
        this.type = ModelType.Anim;
    }
}
