using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Set in Inspector")]
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI diaText;
    public Image char0Image;
    public Image char1Image;
    public Image char2Image;
    public Image backgroundImage;
    public GameObject choicePanel;
    public GameObject choice1;
    public GameObject choice2;
    public GameObject choice3;
    public GameObject choice4;
    public GameObject nextSentenceImage;
    public Picture[] backgroundImages;

    private Button choice1Button;
    private Button choice2Button;
    private Button choice3Button;
    private Button choice4Button;
    private TextMeshProUGUI choice1Text;
    private TextMeshProUGUI choice2Text;
    private TextMeshProUGUI choice3Text;
    private TextMeshProUGUI choice4Text;
    private Dictionary<string, Picture> backgroundImageDict;

    private void Start()
    {
        InitializeUI();

        backgroundImageDict = new Dictionary<string, Picture>();

        foreach(Picture s in backgroundImages)
        {
            backgroundImageDict.Add(s.pictureName, s);
        }
    }

    private void InitializeUI()
    {
        choice1Button = choice1.GetComponent<Button>();
        choice2Button = choice2.GetComponent<Button>();
        choice3Button = choice3.GetComponent<Button>();
        choice4Button = choice4.GetComponent<Button>();
        choice1Text = choice1.GetComponentInChildren<TextMeshProUGUI>();
        choice2Text = choice2.GetComponentInChildren<TextMeshProUGUI>();
        choice3Text = choice3.GetComponentInChildren<TextMeshProUGUI>();
        choice4Text = choice4.GetComponentInChildren<TextMeshProUGUI>();

        choicePanel.SetActive(false);
    }

    public void UpdateSpeakerName(string name)
    {
        charNameText.text = name;
    }

    public IEnumerator UpdateDiaText(string text)
    {
        diaText.text = "";
        DisplayMain.Waiting = text.Length * 0.01f + 0.2f;
        foreach (char c in text.ToCharArray())
        {
            diaText.text += c;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SetChoiceText(string text, int choiceIndex)
    {
        if(!choicePanel.activeSelf)
        {
            choicePanel.SetActive(true);
        }

        if(choiceIndex < 1 || choiceIndex > 4)
        {
            Debug.LogError("Choice index can only be the integer from 1 to 4");
            return;
        }

        switch(choiceIndex)
        {
            case 1:
                choice1Text.text = text;
                return;
            case 2:
                choice2Text.text = text;
                return;
            case 3:
                choice3Text.text = text;
                return;
            case 4:
                choice4Text.text = text;
                return;
        }
    }

    public void DisableChoicePanel()
    {
        choice1Text.text = "...";
        choice2Text.text = "...";
        choice3Text.text = "...";
        choice4Text.text = "...";

        choicePanel.SetActive(false);
    }

    public void SetBackGroundImage(string backgroundImageName)
    {
        if(!backgroundImageDict.ContainsKey(backgroundImageName))
        {
            return;
        }

        backgroundImage.sprite = backgroundImageDict[backgroundImageName].picture;
    }

    public void SetExpression(int pos, Sprite expression)
    {
        switch(pos)
        {
            case 0:
                char0Image.sprite = expression;
                break;
            case 1:
                char1Image.sprite = expression;
                break;
            case 2:
                char2Image.sprite = expression;
                break;
        }
    }

    public void ShowNextSentenceImage()
    {

        if (!nextSentenceImage.activeSelf) { nextSentenceImage.SetActive(true); }
    }

    public void HideNextSentenceImage()
    {
        if (nextSentenceImage.activeSelf) { nextSentenceImage.SetActive(false); }
    }
}
