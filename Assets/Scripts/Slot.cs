using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    public enum SlotValue
    {
        Empty,
        Player,
        AI
    }
    [SerializeField] private SlotValue currentValue;
    [SerializeField] private GameObject xPrefab;
    [SerializeField] private GameObject oPrefab;

    public event Action OnSlotValueChanged;

    private void Start()
    {
        OnSlotValueChanged += UpdateSlotValue;
    }

    public void SetSlotValue(SlotValue newValue)
    {
        Debug.Log("New value on position " + gameObject.name + " has been set to " + newValue);
        currentValue = newValue;
        OnSlotValueChanged?.Invoke();
        // Call this in player/enemy AI. 
        // The methods subscribed are: UpdateSlotValue, UpdateBoardValues, CheckBoardForWin, and HandleSlotValueChanged
    }

    public SlotValue GetSlotValue()
    {
        return currentValue;
    }

    private void UpdateSlotValue()
    {
        if (transform.childCount > 0)
        {
            if (transform.GetChild(0).tag == "X") currentValue = SlotValue.Player;
            else currentValue = SlotValue.AI;
        }
        else currentValue = SlotValue.Empty;
    }

    public void FillSlot(bool turn)
    {
        GameObject playerObject; 

        if (currentValue == SlotValue.Empty)
        {
            if (turn == false) //AI's turn
            {
                playerObject = Instantiate(oPrefab, transform.position, transform.rotation);
                playerObject.transform.SetParent(transform);
                SetSlotValue(SlotValue.AI);
                BoardManager.Instance.SwitchTurn(); 
            }
            else //Player's Turn
            {
                playerObject = Instantiate(xPrefab, transform.position, transform.rotation);
                playerObject.transform.SetParent(transform);
                SetSlotValue(SlotValue.Player);
                BoardManager.Instance.SwitchTurn();
            }
        }
    }

    
    private void OnMouseDown()
    {
        if (BoardManager.Instance.playerTurn == true && GameManager.Instance.GetGameState() == GameManager.GameState.GAME)
        {
            FillSlot(true); 
        }
    }
    

    private void OnMouseEnter()
    {
        Debug.Log(GetSlotValue().ToString());
    }

    private void OnDestroy()
    {
        OnSlotValueChanged -= UpdateSlotValue;
    }
}
