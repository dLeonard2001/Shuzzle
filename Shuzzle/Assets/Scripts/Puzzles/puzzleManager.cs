using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class puzzleManager : MonoBehaviour
{

    [Header("Combination Puzzle")] 
    public List<Detector> detectorList;
    public List<GameObject> list;
    public List<int> blockCombination;
    private List<int> playerInputCombination;
    private int totalObjects;

    private Hashtable combinationNums;

    private void Awake()
    {
        totalObjects = list.Count;
        playerInputCombination = new List<int>();
        blockCombination = new List<int>();
        combinationNums = new Hashtable{
            {1, "What day does the new year start on?"},
            {2, "How many heads are better than one?"},
            {3, "What is 33% in fraction form?"},
            {4, "Is Independence day a national celebration in the USA?"},
            {5, "What rhymes with Hive?"},
            {6, "Nine, One, Five, Two, Three, Seven, Four, Eight"},
            {7, "Sunday is my favorite Day"},
            {8, "Who the fuck ate 9?"},
            {9, "What is the German word for \"no\"?"},
            {0, "Neither positive nor a negative number"}
        };
        
        SetRandomBlockCombination();
    }

    public void PlayerInput(int blockNum)
    {
        if (playerInputCombination.Count != totalObjects)
        {
            playerInputCombination.Add(blockNum);
        }else if (playerInputCombination.Count == totalObjects)
        {
            CheckPlayerCombination();
        }
    }
    
    private void SetRandomBlockCombination()
    {
        while (true)
        {
            if(blockCombination.Count == totalObjects) 
                break;
            
            int num = Random.Range(0, 10);
            
            if (blockCombination.Contains(num))
            {
                continue;
            }

            if (!blockCombination.Contains(num))
            {
                blockCombination.Add(num);
                // Debug.Log(num);
            }
        }
    }

    private void CheckPlayerCombination()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            // Debug.Log("blockCombo" + blockCombination[i] + ": playerInput: " + playerInputCombination[i] );
            if (blockCombination[i] == playerInputCombination[i])
            {
                
            }
            else
            {
                Failed();
                return;
            }
        }
        Success();
    }

    private void Success()
    {
        Debug.Log("Puzzle solved!");
    }

    private void Failed()
    {
        Debug.Log("Wrong combination, please try again");
        for (int i = 0; i < detectorList.Count; i++)
        {
            detectorList[i].ResetBlock();
        }
        playerInputCombination.Clear();
    }

    public String PuzzleHintCombinations(int i)
    {
        

        string str = (string) combinationNums[i];

        return str;
    }
}
