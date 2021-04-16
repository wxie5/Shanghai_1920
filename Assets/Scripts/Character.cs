using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string charName;
    public Expression[] expressions;

    private int expectation = 0;

    public int Expectation
    {
        get { return expectation; }
        set { expectation = value; }
    }

    public Sprite FindExpression(ExpressionType expressionType)
    {
        foreach(Expression e in expressions)
        {
            if(e.expressionType == expressionType)
            {
                return e.expressionSprite;
            }
        }
        return null;
    }


}
