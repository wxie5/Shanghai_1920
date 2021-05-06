using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TxtFileToModel
{
    public Dictionary<string, Queue<IBaseTextModel>> TextToModelList(string fileName)
    {
        StreamReader reader = new StreamReader("Assets/Resources/" + fileName);
        if(reader == null)
        {
            Debug.LogError("Error: file does not exist!! Please put the file in Resources folder");
            return null;
        }
        Dictionary<string, Queue<IBaseTextModel>> outDict = new Dictionary<string, Queue<IBaseTextModel>>();
        string line = reader.ReadLine();
        string key;
        if(line != null && line[0] == 'S')
        {
            Queue<IBaseTextModel> textModel = new Queue<IBaseTextModel>();
            IBaseTextModel model = StartModelSolver(line);
            textModel.Enqueue(model);
            key = ((StartModel)model).sceneName;
            bool isChoice = false;
            List<string> choiceLines = new List<string>();
            while (true)
            {
                line = reader.ReadLine();
                if(line == "")
                {
                    break;
                }

                if(line[0] == 'S')
                {
                    model = StartModelSolver(line);
                    textModel.Enqueue(model);
                    key = ((StartModel)model).sceneName;
                    continue;
                }
                if(line[0] == 'E')
                {
                    textModel.Enqueue(EndModelSolver(line));
                    outDict.Add(key, new Queue<IBaseTextModel>(textModel));
                    textModel.Clear();
                    key = "";
                    continue;
                }
                if(line[0] == 'C')
                {
                    isChoice = true;
                }
                if(line[0] == 'L' && isChoice)
                {
                    textModel.Enqueue(ChoiceModel(choiceLines.ToArray()));
                    choiceLines.Clear();
                    isChoice = false;
                    continue;
                }

                if(isChoice)
                {
                    choiceLines.Add(line);
                    continue;
                }
                textModel.Enqueue(GeneralSolverExceptChoice(line));
            }
        }

        reader.Close();

        return outDict;
    }

    private IBaseTextModel GeneralSolverExceptChoice(string line)
    {
        switch (line[0])
        {
            case 'M':
                return SelfDiaModelSolver(line);
            case 'A':
                return AnimModelSolver(line);
            case 'P':
                return CharMoveModelSolver(line);
            case 'G':
                return TriggerModelSolver(line);
            case 'T':
                return DiaModelSolver(line);
        };

        return null;
    }

    public IBaseTextModel StartModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        StartModel startModel = new StartModel(splitedStr[0]);

        return startModel;
    }

    public IBaseTextModel SelfDiaModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        SelfDiaModel selfDiaModel = new SelfDiaModel(StrToBool(splitedStr[0]),
                                                      NotNullStr(splitedStr[1]),
                                                      NotNullStr(splitedStr[2]),
                                                      StrToBool(splitedStr[3]),
                                                      NotNullStr(splitedStr[4]),
                                                      NotNullStr(splitedStr[5]));

        return selfDiaModel;
    }

    public IBaseTextModel DiaModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        DiaModel diaModel = new DiaModel(NotNullStr(splitedStr[0]),
                                            NotNullStr(splitedStr[1]),
                                            NotNullStr(splitedStr[2]),
                                            NotNullStr(splitedStr[3]),
                                            StrToBool(splitedStr[4]),
                                            StrToInt(splitedStr[5]),
                                            StrToBool(splitedStr[6]),
                                            NotNullStr(splitedStr[7]),
                                            NotNullStr(splitedStr[8]));

        return diaModel;
    }

    public IBaseTextModel AnimModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        AnimModel animModel = new AnimModel(NotNullStr(splitedStr[0]),
                                                NotNullStr(splitedStr[1]),
                                                StrToBool(splitedStr[2]),
                                                NotNullStr(splitedStr[3]),
                                                NotNullStr(splitedStr[4]));

        return animModel;
    }

    public IBaseTextModel TriggerModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        TriggerModel triggerModel = new TriggerModel(NotNullStr(splitedStr[0]),
                                                        NotNullStr(splitedStr[1]),
                                                        StrToInt(splitedStr[2]));

        return triggerModel;
    }

    public IBaseTextModel CharMoveModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);

        CharMoveModel charMoveModel = new CharMoveModel(StrToBool(splitedStr[0]),
                                                            StrToInt(splitedStr[1]),
                                                            NotNullStr(splitedStr[2]),
                                                            NotNullStr(splitedStr[3]),
                                                            NotNullStr(splitedStr[4]),
                                                            NotNullStr(splitedStr[5]));

        return charMoveModel;
    }

    public IBaseTextModel EndModelSolver(string line)
    {
        string[] splitedStr = CutOutStrSet(line);
        EndModel endModel = new EndModel(NotNullStr(splitedStr[0]), NotNullStr(splitedStr[1]));

        return endModel;
    }

    public IBaseTextModel ChoiceModel(string[] lines)
    {
        string[] splitedFirstLine = CutOutStrSet(lines[0]);
        List<string> choices = new List<string>();
        foreach (string s in splitedFirstLine)
        {
            if (s == "n")
            {
                choices.Add("...");
            }
            else
            {
                choices.Add(s);
            }
        }
        Debug.Log(lines.Length);
        List<Queue<IBaseTextModel>> branches = new List<Queue<IBaseTextModel>>();
        Queue<IBaseTextModel> models = new Queue<IBaseTextModel>();
        for(int i = 1; i < lines.Length; i++)
        {
            if(i == (lines.Length - 1))
            {
                models.Enqueue(GeneralSolverExceptChoice(lines[i]));
                branches.Add(new Queue<IBaseTextModel>(models));
                models.Clear();
            }

            if (lines[i][0] == '1' || lines[i][0] == '2' || lines[i][0] == '3' || lines[i][0] == '4')
            {
                if (models.Count != 0)
                {
                    branches.Add(new Queue<IBaseTextModel>(models));
                    models.Clear();
                }
                continue;
            }
            models.Enqueue(GeneralSolverExceptChoice(lines[i]));
        }

        ChoiceModel choiceModel = new ChoiceModel(choices, branches);

        return choiceModel;
    }

    private string[] CutOutStrSet(string line)
    {
        return line.Substring(2).TrimEnd('\n').Split('|');
    }

    private string NotNullStr(string str)
    {
        if(str == "n")
        {
            return "";
        }else
        {
            return str;
        }
    }

    private bool StrToBool(string str)
    {
        if(str == "t")
        {
            return true;
        }else
        {
            return false;
        }
    }

    private int StrToInt(string str)
    {
        return int.Parse(str);
    }
}
