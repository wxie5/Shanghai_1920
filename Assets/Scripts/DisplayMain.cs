using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMain : Singleton<DisplayMain>
{
    private UIManager uiManager;
    private AnimationManager animManager;
    private AudioManager audioManager;
    private StatsManager statsManager;

    private Queue<IBaseTextModel> currentTextModels;
    private Dictionary<string, Queue<IBaseTextModel>> currentDict;
    private TxtFileToModel tftm;
    private bool isDiaStarted = false;
    private bool followAnim = false;
    private bool followCharMove = false;

    private bool isChoosing = false;
    private List<Queue<IBaseTextModel>> currentChoiceBranches;

    private float timer = 0f;
    private static float waiting = 0f;

    private void Start()
    {
        uiManager = UIManager.Instance;
        animManager = AnimationManager.Instance;
        audioManager = AudioManager.Instance;
        statsManager = StatsManager.Instance;

        currentTextModels = new Queue<IBaseTextModel>();
        currentDict = new Dictionary<string, Queue<IBaseTextModel>>();
        currentChoiceBranches = new List<Queue<IBaseTextModel>>();
        tftm = new TxtFileToModel();

        ReadAFile("testStory.txt");
        StartDia("Scene1");
    }

    private void Update()
    {
        if (isDiaStarted)
        {
            if(isChoosing)
            {
                return;
            }

            if(followAnim == true)
            {
                NextCommand();
                followAnim = false;
            }

            timer += Time.deltaTime;

            if (timer > waiting)
            {
                if(followCharMove)
                {
                    NextCommand();
                    followCharMove = false;
                    return;
                }
                uiManager.ShowNextSentenceImage();
                if (Input.GetMouseButtonDown(0))
                {
                    timer = 0;
                    NextCommand();
                    uiManager.HideNextSentenceImage();
                }
            }
        }
    }

    public static float Waiting
    {
        set { waiting = value; }
    }

    //Please ensure that the file is in the correct format, there is no exception check for now
    public void ReadAFile(string fileName)
    {
        currentDict = tftm.TextToModelList(fileName);
    }

    public void StartDia(string diaSceneName)
    {
        if(!currentDict.ContainsKey(diaSceneName))
        {
            Debug.Log("No Such Scene!");
            return;
        }

        currentTextModels = currentDict[diaSceneName];
        isDiaStarted = true;
        NextCommand();
    }

    private void NextCommand()
    {
        if(currentTextModels.Count <= 0)
        {
            return;
        }
        IBaseTextModel ibtm = currentTextModels.Dequeue();
        string nextType = "";
        if (currentTextModels.Count >= 1)
        {
            nextType = currentTextModels.Peek().GetType().ToString();
        }
        string typeName = ibtm.GetType().ToString();
        Debug.Log(typeName);

        switch(typeName)
        {
            case "StartModel":
                NextCommand();
                break;
            case "SelfDiaModel":
                DisplaySelfDiaModel(ibtm);
                break;
            case "DiaModel":
                DisplayDiaModel(ibtm);
                break;
            case "AnimModel":
                DisplayAnimModel(ibtm);
                break;
            case "CharMoveModel":
                DisplayCharMoveModel(ibtm);
                break;
            case "TriggerModel":
                DisplayTriggerModel(ibtm);
                break;
            case "ChoiceModel":
                DisplayChoiceModel(ibtm);
                break;
            case "EndModel":
                break;
        }

        CheckNextTypeAndFollowAnim(typeName, nextType);
        CheckNextTypeAndFollowCharMove(typeName, nextType);
    }

    private void DisplaySelfDiaModel(IBaseTextModel ibtm)
    {
        SelfDiaModel selfDiaModel = (SelfDiaModel)ibtm;

        if (selfDiaModel.isSelf) { uiManager.UpdateSpeakerName("Me"); }
        else                     { uiManager.UpdateSpeakerName(""); }

        uiManager.StartCoroutine("UpdateDiaText", selfDiaModel.text);

        if (selfDiaModel.audio != "") { audioManager.PlayMusic(selfDiaModel.audio, MusicType.HumanSound); }

        if (selfDiaModel.isLightning) { animManager.LightningShock(); }

        if (selfDiaModel.background != "") { uiManager.SetBackGroundImage(selfDiaModel.background); }

        if (selfDiaModel.bgm != "") { audioManager.PlayMusic(selfDiaModel.bgm, MusicType.BGM); }
    }

    private void DisplayAnimModel(IBaseTextModel ibtm)
    {
        AnimModel animModel = (AnimModel)ibtm;

        switch(animModel.animName)
        {
            case "bf_oi":
                animManager.StartCoroutine("BlindfoldFadeOutFadeIn", 2f);
                break;
            case "bf_i":
                animManager.BlindfoldFadeIn();
                break;
            case "bf_o":
                animManager.BlindfoldFadeOut();
                break;
            case "src_shake":
                animManager.BackgroundShake();
                break;
        }

        if (animModel.audio != "") { audioManager.PlayMusic(animModel.audio, MusicType.HumanSound); }

        if (animModel.isLightning) { animManager.LightningShock(); }

        if (animModel.background != "") { uiManager.SetBackGroundImage(animModel.background); }

        if (animModel.bgm != "") { audioManager.PlayMusic(animModel.bgm, MusicType.BGM); }
    }

    private void DisplayCharMoveModel(IBaseTextModel ibtm)
    {
        CharMoveModel charMoveModel = (CharMoveModel)ibtm;

        if(charMoveModel.isCome) { animManager.Come(charMoveModel.pos); }
        else                     { animManager.Leave(charMoveModel.pos); }

        uiManager.SetExpression(charMoveModel.pos, statsManager.GetCharacter(charMoveModel.charName).FindExpression(ExpressionType.Normal));

        if (charMoveModel.audio != "") { audioManager.PlayMusic(charMoveModel.audio, MusicType.HumanSound); }

        if (charMoveModel.background != "") { uiManager.SetBackGroundImage(charMoveModel.background); }

        if (charMoveModel.bgm != "") { audioManager.PlayMusic(charMoveModel.bgm, MusicType.BGM); }
    }

    private void DisplayDiaModel(IBaseTextModel ibtm)
    {
        DiaModel diaModel = (DiaModel)ibtm;

        uiManager.UpdateSpeakerName(diaModel.name);

        uiManager.StartCoroutine("UpdateDiaText", diaModel.text);

        Character character = statsManager.GetCharacter(diaModel.name);
        Sprite expression = null;
        switch(diaModel.expression)
        {
            case "normal":
                expression = character.FindExpression(ExpressionType.Normal);
                break;
            case "happy":
                expression = character.FindExpression(ExpressionType.Happy);
                break;
            case "angry":
                expression = character.FindExpression(ExpressionType.Angry);
                break;
            case "confused":
                expression = character.FindExpression(ExpressionType.Confused);
                break;
            case "shocking":
                expression = character.FindExpression(ExpressionType.Shocking);
                break;
        }

        if(expression != null)
        {
            uiManager.SetExpression(diaModel.pos, expression);
        }

        if (diaModel.audio != "") { audioManager.PlayMusic(diaModel.audio, MusicType.HumanSound); }

        if (diaModel.isShaking) { animManager.Shake(diaModel.pos); }

        if (diaModel.isLightning) { animManager.LightningShock(); }

        if (diaModel.background != "") { uiManager.SetBackGroundImage(diaModel.background); }

        if(diaModel.bgm != "") { audioManager.PlayMusic(diaModel.bgm, MusicType.BGM); }
    }

    private void DisplayTriggerModel(IBaseTextModel ibtm)
    {
        return;
    }

    private void DisplayChoiceModel(IBaseTextModel ibtm)
    {
        ChoiceModel choiceModel = (ChoiceModel)ibtm;

        List<string> choiceTexts = choiceModel.choicesText;
        for(int i = 0; i < choiceTexts.Count; i++)
        {
            uiManager.SetChoiceText(choiceTexts[i], i + 1);
        }

        currentChoiceBranches = choiceModel.choicesBranch;

        isChoosing = true;
    }

    private void CheckNextTypeAndFollowAnim(string currentType, string nextType)
    {
        if(currentType != "AnimModel" && nextType == "AnimModel")
        {
            followAnim = true;
        }
    }

    private void CheckNextTypeAndFollowCharMove(string currentType, string nextType)
    {
        if(currentType == "AnimModel" && nextType == "CharMoveModel")
        {
            followCharMove = true;
        }
    }

    public void ChooseBranch0()
    {
        if(isChoosing)
        {
            Queue<IBaseTextModel> newTextModels = new Queue<IBaseTextModel>();
            Queue<IBaseTextModel> addinTextModels = currentChoiceBranches[0];

            Debug.Log(addinTextModels.Count);

            while(addinTextModels.Count > 0)
            {
                newTextModels.Enqueue(addinTextModels.Dequeue());
            }

            while(currentTextModels.Count > 0)
            {
                newTextModels.Enqueue(currentTextModels.Dequeue());
            }

            currentTextModels = newTextModels;

            uiManager.DisableChoicePanel();
            isChoosing = false;
        }
    }

    public void ChooseBranch1()
    {
        if(isChoosing)
        {
            Queue<IBaseTextModel> newTextModels = new Queue<IBaseTextModel>();
            Queue<IBaseTextModel> addinTextModels = currentChoiceBranches[1];

            while (addinTextModels.Count > 0)
            {
                newTextModels.Enqueue(addinTextModels.Dequeue());
            }

            while (currentTextModels.Count > 0)
            {
                newTextModels.Enqueue(currentTextModels.Dequeue());
            }

            currentTextModels = newTextModels;

            uiManager.DisableChoicePanel();
            isChoosing = false;
        }
    }

    public void ChooseBranch2()
    {
        if(isChoosing)
        {
            Queue<IBaseTextModel> newTextModels = new Queue<IBaseTextModel>();
            Queue<IBaseTextModel> addinTextModels = currentChoiceBranches[2];

            while (addinTextModels.Count > 0)
            {
                newTextModels.Enqueue(addinTextModels.Dequeue());
            }

            while (currentTextModels.Count > 0)
            {
                newTextModels.Enqueue(currentTextModels.Dequeue());
            }

            currentTextModels = newTextModels;

            uiManager.DisableChoicePanel();
            isChoosing = false;
        }
    }

    public void ChooseBranch3()
    {
        if(isChoosing)
        {
            Queue<IBaseTextModel> newTextModels = new Queue<IBaseTextModel>();
            Queue<IBaseTextModel> addinTextModels = currentChoiceBranches[3];

            while (addinTextModels.Count > 0)
            {
                newTextModels.Enqueue(addinTextModels.Dequeue());
            }

            while (currentTextModels.Count > 0)
            {
                newTextModels.Enqueue(currentTextModels.Dequeue());
            }

            currentTextModels = newTextModels;

            uiManager.DisableChoicePanel();
            isChoosing = false;
        }
    }

}
