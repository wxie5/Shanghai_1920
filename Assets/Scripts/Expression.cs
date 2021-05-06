using UnityEngine;

[System.Serializable]
public struct Expression
{
    public ExpressionType expressionType;
    public Sprite expressionSprite;

}

public enum ExpressionType
{
    Normal,
    Happy,
    Angry,
    Shocking,
    Confused
}

