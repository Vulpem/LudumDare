﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1 turn:

    text appears
    player action
    answer (text)
    result (change in stats)

    */

public enum PLAYER_ACTIONS
{
    ACTION_1,
    ACTION_2,
    ACTION_3,
    ACTION_4,
    ACTION_5
}

public enum STATS
{
    S_MUTINY = 0,
    S_TRUST,

    //Keep this at the end
    S_LAST
}

public enum RELATIONSHIP
{
    GREATER_THAN,
    SMALLER_THAN
}

public enum WHO_AFFECTS
{
    ME,
    EVERYONE,
    OTHERS
}

[System.Serializable]
public class Answer
{
    [TextArea(2, 10)]
    [Tooltip("Text that will appear after the player makes a certain action")]
    public string text;
    [Tooltip("How the values will change because of this")]
    public Result[] result; 
}

[System.Serializable]
public class Result
{
    public WHO_AFFECTS whom;
    [Tooltip("The stat that will change")]
    public STATS stat;
    [Tooltip("The amount it will change")]
    public int amount;
}

[System.Serializable]
public class Condition
{
    public STATS stat;
    public RELATIONSHIP relationship;
    public int value;
}



[System.Serializable]
public class SpeechBubble
{
    [Header(" --- Start of a Speech Bubble --- ")]
    [TextArea(2, 10)]
    [Tooltip("Text that will appear when this speech bubble is chosen")]
    public string text;
    [Header("Answer they will give to each of your actions")]
    public Answer Action1;
    public Answer Action2;
    public Answer Action3;
    public Answer Action4;
    public Answer Action5;
    [Header("Conditions for this speech bubble to appear")]
    public Condition[] conditions;
}




public class Character : MonoBehaviour {

    //The name of the Character is the name of the GameObject
    [TextArea(1, 10)]
    public string descrpition;
    public bool active = false;
    public bool activeLastTurn = false;

    [Header(" --- Speech bubbles --- ")]
    public int activeBubble = 0;
    public SpeechBubble[] bubbles;

    [Header(" --- Stats of the character --- ")]
    public int[] Stats = new int [(int)(STATS.S_LAST)];

    [HideInInspector]
    public TextManager manager;
    [HideInInspector]
    public int characterN = -1;

    Vector3 scale;

    void Start()
    {
        scale = gameObject.transform.localScale;
    }

    void Update()
    {
        if (active != activeLastTurn)
        {
            if (active == false)
            {
                GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
                gameObject.transform.localScale = scale * 0.6f;
            }
            else
            {
                GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
                gameObject.transform.localScale = scale;
            }
            activeLastTurn = active;
        }
    }

    public void ClickUp(int buttonN)
    {
        manager.ClickedOnMe(gameObject, buttonN);
    }

    void ChangeStat(STATS stat, int amount)
    {
        Stats[(int)(stat)] += amount;
    }

    public void ChooseSpeechBubble()
    {
        int n = 0;

        foreach (SpeechBubble bubble in bubbles)
        {

            bool selectable = true;
            foreach(Condition condition in bubble.conditions)
            {
                if(condition.relationship == RELATIONSHIP.SMALLER_THAN)
                {
                    Stats[(int)(condition.stat)] *= -1;
                    condition.value *= -1;
                }

                if(Stats[(int)(condition.stat)] < condition.value)
                {
                    selectable = false;
                    break;
                }

                if (condition.relationship == RELATIONSHIP.SMALLER_THAN)
                {
                    Stats[(int)(condition.stat)] *= -1;
                    condition.value *= -1;
                }

            }

            if (selectable == true)
            {
                activeBubble = n;
                return;
            }

            n++;
        }

        activeBubble = 0;
    }

}
