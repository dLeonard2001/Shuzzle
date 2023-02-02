using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class puzzleManager : MonoBehaviour
{

    [Header("Puzzle 1")] 
    public List<Detector> detectorList;
    public List<GameObject> list;
    public List<int> blockCombination;
    private List<int> playerInputCombination;
    private int totalObjects;

    private void Awake()
    {
        totalObjects = list.Count;
        playerInputCombination = new List<int>();
        blockCombination = new List<int>();
        SetRandomBlockCombination();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerInput(int blockNum)
    {
        if (playerInputCombination.Count != totalObjects)
        {
            playerInputCombination.Add(blockNum);
        }

        if (playerInputCombination.Count == totalObjects)
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
        Hashtable table = new Hashtable()
        {
            {1, "The new year starts on this day"},
            {2, "... heads are better than one"},
            {3, "33% can also be said as 1/..."},
            {4, "4"},
            {5, "5"},
            {6, "6"},
            {7, "7"},
            {8, "8"},
            {9, "9"},
            {0, "0"}
        };

        string str = (string) table[i];

        return str;
    }
}
