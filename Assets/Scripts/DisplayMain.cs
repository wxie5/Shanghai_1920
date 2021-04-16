using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMain : MonoBehaviour
{
    private UIManager uiManager;
    private AnimationManager animManager;
    private AudioManager audioManager;

    private Queue<IBaseTextModel> currentTextModels;
    private Dictionary<string, Queue<IBaseTextModel>> currentDict;
    private TxtFileToModel tftm;
    private bool isDiaStarted = false;
    private bool followAnim = false;
    private float timer = 0f;
    private static float waiting = 0f;

    private void Start()
    {
        uiManager = UIManager.Instance;
        animManager = AnimationManager.Instance;
        audioManager = AudioManager.Instance;

        currentTextModels = new Queue<IBaseTextModel>();
        currentDict = new Dictionary<string, Queue<IBaseTextModel>>();
        tftm = new TxtFileToModel();

        ReadAFile("testStory.txt");
        StartDia("Scene1");
    }

    private void Update()
    {
        if (isDiaStarted)
        {
            if(followAnim == true)
            {
                NextCommand();
                followAnim = false;
            }
            
            if (timer > waiting)
            {
                uiManager.ShowNextSentenceImage();
                if (Input.GetMouseButtonDown(0))
                {
                    timer = 0;
                    NextCommand();
                    uiManager.HideNextSentenceImage();
                }
            }

            timer += Time.deltaTime;
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
            Debug.LogError("No more command/sentence in this scene!!");
        }
        IBaseTextModel ibtm = currentTextModels.Dequeue();
        string nextType = "";
        if (currentTextModels.Count >= 1)
        {
            nextType = currentTextModels.Peek().GetType().ToString();
        }
        string typeName = ibtm.GetType().ToString();

        switch(typeName)
        {
            case "StartModel":
                NextCommand();
                break;
            case "SelfDiaModel":
                DisplaySelfDiaModel(ibtm);
                break;
            case "DiaModel":
                break;
            case "AnimModel":
                DisplayAnimModel(ibtm);
                break;
            case "CharMoveModel":
                break;
            case "TriggerModel":
                break;
            case "ChoiceModel":
                break;
            case "EndModel":
                break;
        }

        CheckNextTypeAndFollowAnim(typeName, nextType);
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

    private void CheckNextTypeAndFollowAnim(string currentType, string nextType)
    {
        if(currentType != "AnimModel" && nextType == "AnimModel")
        {
            followAnim = true;
        }
    }

}
