using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    public Character[] characters;

    private Dictionary<string, Character> charSearchDict;

    private void Start()
    {
        charSearchDict = new Dictionary<string, Character>();

        foreach(Character c in characters)
        {
            charSearchDict[c.charName] = c;
        }
    }

    public Character GetCharacter(string charName)
    {
        if(charSearchDict.ContainsKey(charName))
        {
            return charSearchDict[charName];
        }
        return null;
    }
}
