using TMPro;
using UnityEngine;

public class Detector : MonoBehaviour
{

    [Header("Block Config")] 
    public int blockNumber;
    public TextMeshPro numberDisplay;
    public TextMeshPro hintDisplay;
    private bool alreadyHit;

    [Header("References")] 
    public puzzleManager puzzleManager;

    private void Start()
    {
        alreadyHit = false;
        numberDisplay.text = puzzleManager.blockCombination[blockNumber - 1].ToString();
        hintDisplay.text = blockNumber + ". " + puzzleManager.PuzzleHintCombinations(puzzleManager.blockCombination[blockNumber - 1]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alreadyHit)
        {
            Debug.Log("Already hit");
        }
        else
        {
            Debug.Log("Block: " + blockNumber + " hit");
            puzzleManager.PlayerInput(puzzleManager.blockCombination[blockNumber - 1]);
        }
    }

    public void ResetBlock()
    {
        alreadyHit = false;
    }
}
