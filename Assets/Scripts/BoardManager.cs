using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// This will manage all the positions in the board, if they are empty, occupied, etc 
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    [SerializeField] GameObject[] boardSlots;
    [SerializeField] AIController aiController; 
    public bool playerTurn;

    #region On Start and Awake
    public void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        foreach (GameObject slotObject in boardSlots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            slot.OnSlotValueChanged += HandleSlotValueChanged;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject slotObject in boardSlots)
        {
            Slot slot = slotObject.GetComponent<Slot>();

            if (slot != null)
            {
                slot.OnSlotValueChanged -= HandleSlotValueChanged;
            }
        }
    }

    private void Start()
    {
        playerTurn = true;
        //aiController.BestMove(); 
    }

    #endregion

    #region Getting All Slots, Empty Slots, and Empty slot values
    public Slot[] GetAllSlots()
    {
        Slot[] allSlots = new Slot[boardSlots.Length];

        for (int i = 0; i < boardSlots.Length; i++)
        {
            Slot slot = boardSlots[i].GetComponent<Slot>();

            if (slot != null)
            {
                allSlots[i] = slot;
            }
        }

        return allSlots;
    }

    public Slot[] GetEmptySlots()
    {
        List<Slot> emptySlotsList = new List<Slot>();

        for (int i = 0; i < 9; i++)
        {
            if (boardSlots[i].GetComponent<Slot>().GetSlotValue() == Slot.SlotValue.Empty)
            {
                emptySlotsList.Add(boardSlots[i].GetComponent<Slot>());
            }
        }
        return emptySlotsList.ToArray();
    }

    public int GetNumEmptySlots()
    {
        Debug.Log("Empty Count: " + GetEmptySlots().Length); 
        return GetEmptySlots().Length; 
    }
    #endregion


    #region Turn Getters and Setters 
    public void SwitchTurn()
    {
        playerTurn = !playerTurn;
        Debug.Log("Player turn switched to: " + playerTurn);
        if (!playerTurn && GameManager.Instance.GetGameState() == GameManager.GameState.GAME) aiController.BestMove(); 
    }

    #endregion 

    #region Handling Slot value changing and updating board accordingly 
    private void HandleSlotValueChanged()
    {
        Debug.Log("Slot value on the board has changed!");
        GetEmptySlots(); 
        CheckBoardForWin(); //Check for a win / loss condition
    }

    private void CheckBoardForWin()
    {
        // checking for each row, 0, 1, 2, then 3,4,5, then 6,7,8 
        //Checking for a win in rows
        Debug.Log("Check Board For Win"); 
        Slot[] slots = new Slot[9];

        for (int i = 0; i < boardSlots.Length; i++)
        {
            slots[i] = boardSlots[i].GetComponent<Slot>();
        }

        CheckRows(slots);
        CheckColumns(slots);
        CheckDiagonals(slots);
        if (IsBoardFull(slots)) GameManager.Instance.SwitchState(GameManager.GameState.DRAW);  
    }
    private bool CheckAllSlots(Slot slot1, Slot slot2, Slot slot3) // Checks if all three slots being checked are equal to one another 
    {
        Slot.SlotValue value1 = slot1.GetSlotValue();
        Slot.SlotValue value2 = slot2.GetSlotValue();
        Slot.SlotValue value3 = slot3.GetSlotValue();

        // If the first value is not Empty, return true if the value of the first slot is equivalent to the two other slots
        return value1 != Slot.SlotValue.Empty && value1 == value2 && value1 == value3;
    }

    private void CheckRows(Slot[] slots)
    {
        //Checking win conditions in all 3 rows, 0-2, 3-5, row 6-8
        for (int row = 0; row < 3; row++) // 0, 1, 2 
        {
            if (CheckAllSlots(slots[row * 3], slots[row * 3 + 1], slots[row * 3 + 2]))
            {
                Debug.Log("Three in a row in position " + slots[row * 3] + ", " + slots[row * 3 + 1] +  ", " + slots[row * 3 + 2]); 
                if (slots[row*3].GetSlotValue() == Slot.SlotValue.AI)
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.DEFEAT); // AI Wins, player loses
                    return;
                }
                else if (slots[row*3].GetSlotValue() == Slot.SlotValue.Player)
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.WIN); // Player wins, AI loses. SHOULD NEVER HAPPEN!!!
                    return;
                }

            }
        }
    }
    private void CheckColumns(Slot[] slots)
    {
        //Checking win conditions im all 3 columns, (0,3,6), (1,4,7), (2,5,8) 
        for (int col = 0; col < 3; col++)
        {
            if (CheckAllSlots(slots[col], slots[col + 3], slots[col + 6]))
            {
                Debug.Log("Three in a row in position " + slots[col].name + ", " + slots[col+3].name + ", " + slots[col+6].name);
                if (slots[col].GetSlotValue() == Slot.SlotValue.AI)
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.DEFEAT); // AI Wins, player loses
                    return;
                }
                else if (slots[col].GetSlotValue() == Slot.SlotValue.Player)
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.WIN); // Player wins, AI loses. SHOULD NEVER HAPPEN!!!
                    return;
                }
            }
        }
    }

    private void CheckDiagonals(Slot[] slots)
    {
        //Checking win conditions diagonal

        if (CheckAllSlots(slots[0], slots[4], slots[8]) || CheckAllSlots(slots[2], slots[4], slots[6]))
        {
            if (slots[0].GetSlotValue() == Slot.SlotValue.AI || slots[2].GetSlotValue() == Slot.SlotValue.AI)
            {
                Debug.Log("AI Diagonal Win!");
                GameManager.Instance.SwitchState(GameManager.GameState.DEFEAT); // AI Wins, player loses
                return;
            }
            else
            {
                Debug.Log("Player Diagonal Win!");
                GameManager.Instance.SwitchState(GameManager.GameState.WIN); // Player wins, AI loses. SHOULD NEVER HAPPEN!!!
                return;
            }
        }
        
    }

    private bool IsBoardFull(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<Slot>().GetSlotValue() == Slot.SlotValue.Empty)
            {
                return false; 
            }
        }
        return true; 
    }
    #endregion 
}