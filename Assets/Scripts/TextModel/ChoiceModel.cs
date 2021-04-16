using System.Collections.Generic;

[System.Serializable]
public class ChoiceModel : IBaseTextModel
{
    public List<string> choicesText;
    public List<Queue<IBaseTextModel>> choicesBranch;
    public ModelType type;

    public ChoiceModel(List<string> choicesText, List<Queue<IBaseTextModel>> choicesBranch)
    {
        this.choicesText = choicesText;
        this.choicesBranch = choicesBranch;
        this.type = ModelType.Choice;
    }
}
