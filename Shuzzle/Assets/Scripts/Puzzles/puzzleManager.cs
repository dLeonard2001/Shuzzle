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
    private List<int> playerInputCombination;
    private List<int> blockCombination;
    private int totalObjects;

    private void Start()
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
            
            int num = Random.Range(0, 4);
            
            if (blockCombination.Contains(num))
            {
                continue;
            }

            if (!blockCombination.Contains(num))
            {
                blockCombination.Add(num);
            }
        }
    }

    private void CheckPlayerCombination()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            if (blockCombination[i] == playerInputCombination[i])
            {
                // do nothing
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
}
