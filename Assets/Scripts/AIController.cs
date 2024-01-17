using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;

public class AIController : MonoBehaviour
{
    [SerializeField] Slot[] slots;
    private int numOfEmptySlots;

    // Uses MiniMax algorithm to determine best move 
    private void Start()
    {
        UpdateBoardValues();
    }
    private void UpdateBoardValues()
    {
        slots = BoardManager.Instance.GetAllSlots();
        numOfEmptySlots = BoardManager.Instance.GetNumEmptySlots();
    }

    private string[] CreateSimulatedBoard(Slot[] boardSlots)
    {

        // Basically duplicating the board onto the string array 
        string[] simulatedBoard = new string[9];

        for (int i = 0; i < boardSlots.Length; i++)
        {
            Slot.SlotValue slotValue = boardSlots[i].GetSlotValue();

            if (slotValue == Slot.SlotValue.Player)
                simulatedBoard[i] = "X";
            else if (slotValue == Slot.SlotValue.AI)
                simulatedBoard[i] = "O";
            else
                simulatedBoard[i] = "";
        }
        return simulatedBoard;
    }

    public void BestMove()
    {
        string[] simulatedBoard = CreateSimulatedBoard(slots);
        int bestMove = FindBestMove(simulatedBoard);
        slots[bestMove].FillSlot(false); // Actually fill out the slot in the REAL board once its discovered its the best move
        UpdateBoardValues(); 
    }

    private int FindBestMove(string[] boardStatus)
    {
        int bestScore = int.MinValue;
        int bestMove = -1;
        // The start of our simulated game 
        for (int i = 0; i < 9; i++)
        {
            if (boardStatus[i] == "") // Check if empty
            {
                boardStatus[i] = "O"; 
                //This is essentially going to call itself over and over again until it finds the maximizing value and the
                //best move to make vvvvvvvvvvv
                int score = MiniMax(boardStatus, 0, false);
                boardStatus[i] = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        return bestMove;
    }

    //Depth represenets the number of moves made in the simulated game 
    private int MiniMax(string[] boardStatus, int depth, bool isMaximizing)
    {
        string result = CheckForWinner(boardStatus);
        //If the simulated game is over, return the score 
        //The default case of our recursive function
        if (result != null)
        {
            return GetScore(result);
        }

        if (isMaximizing) // If its the AI's turn 
        {
            // When its maximizing, its exploring every possible move by placing an O in the empty slot
            // it calls itself again, simulating the player's move as well, exploring 
            //moves that the player does
            int bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (boardStatus[i] == "")
                {
                    boardStatus[i] = "O";
                    int score = MiniMax(boardStatus, depth + 1, false); // Setting it to player's turn 
                    boardStatus[i] = "";
                    //Resetting that slot to try another slot  
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (boardStatus[i] == "")
                {
                    boardStatus[i] = "X";
                    int score = MiniMax(boardStatus, depth + 1, true); // Setting it back to AI's turn 
                    boardStatus[i] = ""; //Resetting that slot to try another slot 
                    bestScore = Mathf.Min(score, bestScore); 
                }
            }
            return bestScore;
        }
    }

    private string CheckForWinner(string[] boardStatus)
    {
        // Check rows
        for (int i = 0; i < 3; i++)
        {
            int startIndex = i * 3;
            if (AreAllEqual(boardStatus[startIndex], boardStatus[startIndex + 1], boardStatus[startIndex + 2]))
                return boardStatus[startIndex];
        }

        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (AreAllEqual(boardStatus[i], boardStatus[i + 3], boardStatus[i + 6]))
                return boardStatus[i];
        }

        // Check diagonals
        if (AreAllEqual(boardStatus[0], boardStatus[4], boardStatus[8]))
            return boardStatus[0];

        if (AreAllEqual(boardStatus[2], boardStatus[4], boardStatus[6]))
            return boardStatus[2];

        // Check for a tie (board is full)
        if (IsBoardFull(boardStatus))
            return "tie";

        // No winner yet
        return null;
    }

    private bool AreAllEqual(string a, string b, string c)
    {
        return !string.IsNullOrEmpty(a) && a == b && b == c;
    }

    private bool IsBoardFull(string[] boardStatus)
    {
        for (int i = 0; i < boardStatus.Length; i++)
        {
            if (string.IsNullOrEmpty(boardStatus[i]))
                return false;
        }
        return true;
    }


    private int GetScore(string result)
    {
        if (result == "X") return -1;
        else if (result == "O") return 1;
        else return 0;
    }
}