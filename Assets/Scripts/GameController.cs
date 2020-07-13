using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int whoTurn;
    public int turnCount;
    public GameObject[] turnIcons;
    public Text[] TextScore;
    public Sprite[] playerIcons;
    public Button[] othelloSpaces;
    public int[] markedSpaces;
    public static int depth = 1;
    public Text possMoveListUI ;
    public Text moveListUI;
    public GameObject startButton;
    public GameObject stopButton;
    public Dropdown levels;
    public static int nbMove=0;


    // Start is called before the first frame update
    void Start()
    {

        startButton.SetActive(true);
        stopButton.SetActive(false);

    }
    public void StartGame()
    {
        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);
        levels.interactable = false ;
        gameSetup();
        moveListUI.text = " ";
        nbMove = 0;
        moveListUI.text += "\n" + "<Start Game> " + "\n";

    }

    public void StopGame()
    {
        moveListUI.text = " ";
        moveListUI.text += "\n" + "<Stop Game> " + "\n";
        possMoveListUI.text = " " ;
        ResetGame();
        clearPossMove();

        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);
        levels.interactable = true;



    }
    void ResetGame()
    {

        for (int i = 0; i < othelloSpaces.Length; i++)
        {
            if ((i == 27) || (i == 36))
            {

                othelloSpaces[i].interactable = false;
                othelloSpaces[i].GetComponent<Image>().sprite = playerIcons[1];

            }
            if ((i == 28) || (i == 35))
            {

                othelloSpaces[i].interactable = false;
                othelloSpaces[i].GetComponent<Image>().sprite = playerIcons[0];
            }
            if (i != 27 && i != 28 && i != 35 && i != 36)
            {
                othelloSpaces[i].interactable = true;
                othelloSpaces[i].GetComponent<Image>().sprite = null;
            }
        }

        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if ((i == 27) || (i == 36))
            {

                markedSpaces[i] = 2;
            }
            if ((i == 28) || (i == 35))
            {

                markedSpaces[i] = 1;
            }
            if (i != 27 && i != 28 && i != 35 && i != 36)
            {
                markedSpaces[i] = -1;
            }
        }

        TextScore[0].text = "" + 2;
        TextScore[1].text = "" + 2;
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);


    }

    public void HandleInputData(int val)
    {
        if (val == 0)
          depth = 1;
        else if (val == 1)
           depth = 4;
        else if (val == 2)
            depth = 10;
        else depth = 1;
    }
    void gameSetup()
    {
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);
        for (int i=0; i<othelloSpaces.Length; i++)
        {
            if(i!=27 && i != 28 && i != 35 && i != 36)
            {
                othelloSpaces[i].interactable = true;
                othelloSpaces[i].GetComponent<Image>().sprite = null;
            }
        }
        for(int i=0; i<markedSpaces.Length; i++)
        {
            if (i != 27 && i != 28 && i != 35 && i != 36)
            {
                markedSpaces[i] = -100;
            }
        }
        TextScore[0].text = "" + checkBlackScore(markedSpaces);
        TextScore[1].text = "" + checkWhiteScore(markedSpaces);
        clearPossMove();
        //checkPossibleMove();
        ArrayList possibleMoves = checkPossibleMove(markedSpaces);
        printPossibleMove(possibleMoves);
        showPossibleMove(possibleMoves);


    }

    // Update is called once per frame
    void Update()
    {

    }
    void printPossibleMove(ArrayList possibleMove)
    {
        String s = "";
        int row, col;
        possMoveListUI.text = "\n";
        for (int i = 0; i < possibleMove.Count; i++)
        {
             row = int.Parse(possibleMove[i].ToString()) / 8;
             col = int.Parse(possibleMove[i].ToString()) % 8;
            s += "(" + row +"," + col +") ; ";

        }
        possMoveListUI.text  += " " +s + "\n";
    }
    public void othelloButton(int whichNumber)
    {
        //Debug.Log("depth" + depth);
        if (!startButton.activeInHierarchy)
        {
            int row = whichNumber / 8;
            int col = whichNumber % 8;

            othelloSpaces[whichNumber].image.sprite = playerIcons[whoTurn];
            othelloSpaces[whichNumber].interactable = false;

            markedSpaces[whichNumber] = whoTurn + 1;
            turnCount++;
            nbMove++;
            if (whoTurn == 0)
            {
                moveListUI.text += "" + nbMove + "- Black(" + row + "," + col + ")" + "\n";
                turnIcons[1].SetActive(true);
                turnIcons[0].SetActive(false);
                whoTurn = 1;
            }
            else
            {
                moveListUI.text += "" + nbMove + "- White(" + row + "," + col + ")" + "\n";
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
                whoTurn = 0;
            }
            clearPossMove();
            flipPiece(whichNumber);
            ArrayList possibleMove = checkPossibleMove(markedSpaces);
            showPossibleMove(possibleMove);
            printPossibleMove(possibleMove);

           if (possibleMove.Count == 0)
            {
                if (whoTurn == 1)
                {
                    Debug.Log("White pass");
                    whoTurn = 0;
                    turnIcons[0].SetActive(true);
                    turnIcons[1].SetActive(false);
                    clearPossMove();
                    showPossibleMove(checkPossibleMove(markedSpaces));
                }
                else
                {
                    Debug.Log("Black pass");
                    whoTurn = 1;
                    turnIcons[1].SetActive(true);
                    turnIcons[0].SetActive(false);
                    clearPossMove();
                    possibleMove = checkPossibleMove(markedSpaces);
                    showPossibleMove(possibleMove);
                    printPossibleMove(possibleMove);
                    StartCoroutine(AIPlay(possibleMove));
                }
            }
            else
            {
                if (whoTurn == 1)
                {
                    StartCoroutine(AIPlay(possibleMove));
                }
            }
            TextScore[0].text = "" + checkBlackScore(markedSpaces);
            TextScore[1].text = "" + checkWhiteScore(markedSpaces);
        }
    }

    IEnumerator AIPlay(ArrayList possibleMove)
    {
        yield return new WaitForSeconds(0.1F);
        int[] markedSpacesClone = (int[]) markedSpaces.Clone();    //'cause when i'm giving the markedSpaces to the bestMove function it takes the reference.
        int i = bestMove(markedSpacesClone, depth);
        if (i >= 0 && i < possibleMove.Count)
            othelloSpaces[(int)possibleMove[i]].onClick.Invoke();
        yield return null;
    }

    int bestMove(int[] board, int depth)
    {   ArrayList possibleMove = checkPossibleMove(board);
        int move = 0;

        if (depth == 1) {
            System.Random random = new System.Random();
            move = random.Next(possibleMove.Count);
        }

        else {
            double Infinity = double.PositiveInfinity;
            double bestScore = -Infinity;
            double alpha = -Infinity;
            double beta = Infinity;

            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 2;
                int score = minimax(board, depth, false, alpha, beta);
                board[(int)possibleMove[i]] = -100;
                if (score >= bestScore)
                {
                    bestScore = score;
                    move = i;
                }
                alpha = Math.Max(alpha, bestScore);
                if (beta <= alpha)
                    break;

            }
        }
        return move;
    }

    int minimax(int[] board, int depth, bool isMaximizing, double alpha, double beta)
    {
        double Infinity = double.PositiveInfinity;
        ArrayList possibleMove = checkPossibleMove(board);

        if (depth == 0)
        {
            return evaluationFct(board);
        }

        if (isMaximizing)
        {
            double bestScore = -Infinity;
            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 2;
                int score = minimax(board, depth - 1, false, alpha, beta);
                board[(int)possibleMove[i]] = -100;
                bestScore = Math.Max(score, bestScore);
                alpha = Math.Max(alpha, bestScore);
                if (beta <= alpha)
                    break;
            }
            return (int)bestScore;
        }
        else
        {
            double bestScore = Infinity;
            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 1;
                int score = minimax(board, depth - 1, true, alpha, beta);
                board[(int)possibleMove[i]] = -100;
                bestScore = Math.Min(score, bestScore);
                beta = Math.Min(beta, bestScore);
                if (beta <= alpha)
                    break;
            }
            return (int)bestScore;
        }
    }

    //Evaluation function of the board : (Positive : Black Has more pieces on the board)
    //                                   (Negative : White Has more pieces on the board)
    int evaluationFct(int[] board)
    {
        return checkBlackScore(board) - checkWhiteScore(board);
    }

    int checkBlackScore(int[] board)
    {
        int blackScore = 0;
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if (markedSpaces[i] == 1)
                blackScore++;
        }
        return blackScore;
    }

    int checkWhiteScore(int[] board)
    {
        int whiteScore = 0;
        for(int i=0; i< markedSpaces.Length; i++)
        {
            if (markedSpaces[i] == 2)
                whiteScore++;
        }
        return whiteScore;
    }

    void flipPiece(int pos)
    {
        int row = pos / 8;
        int col = pos % 8;
        flipALL(row, col, markedSpaces[pos]);
    }

    ArrayList checkPossibleMove(int [] board)
    {
        ArrayList possibleMove = new ArrayList();
        //IDictionary<int, string> neighbors;
        if (whoTurn == 0)
        {
            for(int i=0; i< board.Length; i++)
            {
                int row = i / 8;
                int col = i % 8;

                if (board[i] == 2)
                {
                    if(checkTl(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row + 1) * 8 + (col + 1)] == -100)
                            {
                                if(!possibleMove.Contains((row + 1) * 8 + (col + 1)))
                                    possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -100)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col)))
                                    possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col - 1)))
                                    possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -100)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col + 1)))
                                    possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col - 1)))
                                    possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col + 1)))
                                    possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col)))
                                    possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col - 1)))
                                    possibleMove.Add((row - 1) * 8 + (col - 1));
                            }
                    }
                }
            }
        }
        else if (whoTurn == 1)
        {
            for (int i = 0; i < board.Length; i++)
            {
                int row = i / 8;
                int col = i % 8;

                if (board[i] == 1)
                {
                    if (checkTl(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row + 1) * 8 + (col + 1)] == -100)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col + 1)))
                                    possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -100)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col)))
                                    possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col - 1)))
                                    possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -100)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col + 1)))
                                    possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col - 1)))
                                    possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col + 1)))
                                    possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col)))
                                    possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -100)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col - 1)))
                                    possibleMove.Add((row - 1) * 8 + (col - 1));
                            }
                    }
                }
            }
        }
        return possibleMove;
    }

    void showPossibleMove(ArrayList moves)
    {
        for(int i=0; i<moves.Count; i++)
        {
            othelloSpaces[(int)moves[i]].image.sprite = playerIcons[2];
            othelloSpaces[(int)moves[i]].interactable = true;
        }
    }
    void clearPossMove()
    {
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if(markedSpaces[i] == -100)
            {
                othelloSpaces[i].image.sprite = null;
                othelloSpaces[i].interactable = false;
            }
        }
    }

    bool flipTl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col - 1);
        if (row < 1 || col < 1 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipTl(row - 1, col - 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipTm(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col);
        if (row < 1 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipTm(row - 1, col, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipTr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col + 1);
        if (row < 1 || col > 6 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipTr(row - 1, col + 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipMl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row) * 8 + (col - 1);
        if (col < 1 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipMl(row, col - 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipMr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row) * 8 + (col + 1);
        if (col > 6 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipMr(row, col + 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipBl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + (col - 1);
        if (row > 6 || col < 1 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipBl(row + 1, col - 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipBm(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + col;
        if (row > 6 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if(markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipBm(row + 1, col, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool flipBr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + (col + 1);
        if (row > 6 || col > 6 || markedSpaces[posNeighbor] == -100)
        {
            return false;
        }
        else if (markedSpaces[posNeighbor] == color)
        {
            if (color == 1)
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                markedSpaces[(row) * 8 + (col)] = 1;
            }
            else
            {
                othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                markedSpaces[(row) * 8 + (col)] = 2;
            }
            return true;
        }
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -100)
        {
            bool what = flipBr(row + 1, col + 1, color);
            if (what)
            {
                if (color == 1)
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[0];
                    markedSpaces[(row) * 8 + (col)] = 1;
                }
                else
                {
                    othelloSpaces[(row) * 8 + (col)].image.sprite = playerIcons[1];
                    markedSpaces[(row) * 8 + (col)] = 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    void flipALL(int row, int col, int color)
    {

        if (row == 0)
        {
            if (col == 0)
            {
                flipMr(row, col, color);
                flipBm(row, col, color);
                flipBr(row, col, color);
            }
            else if (col == 7)
            {
                flipMl(row, col, color);
                flipBl(row, col, color);
                flipBm(row, col, color);
            }
            else
            {
                flipMl(row, col, color);
                flipMr(row, col, color);
                flipBl(row, col, color);
                flipBm(row, col, color);
                flipBr(row, col, color);
            }
        }
        else if (row == 7)
        {
            if (col == 0)
            {
                flipTm(row, col, color);
                flipTr(row, col, color);
                flipMr(row, col, color);
            }
            else if (col == 7)
            {
                flipTl(row, col, color);
                flipTm(row, col, color);
                flipMl(row, col, color);
            }
            else
            {
                flipTl(row, col, color);
                flipTm(row, col, color);
                flipTr(row, col, color);
                flipMl(row, col, color);
                flipMr(row, col, color);
            }
        }
        else if (col == 0)
        {
            flipTm(row, col, color);
            flipTr(row, col, color);
            flipMr(row, col, color);
            flipBm(row, col, color);
            flipBr(row, col, color);
        }
        else if (col == 7)
        {
            flipTl(row, col, color);
            flipTm(row, col, color);
            flipMl(row, col, color);
            flipBl(row, col, color);
            flipBm(row, col, color);
        }
        else
        {
            flipTl(row, col, color);
            flipTm(row, col, color);
            flipTr(row, col, color);

            flipMl(row, col, color);
            flipMr(row, col, color);

            flipBl(row, col, color);
            flipBm(row, col, color);
            flipBr(row, col, color);
        }
    }
    bool checkTl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row < 0 || col < 0 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkTl(row - 1, col - 1, color);
        }

    }
    bool checkTm(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row < 0 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkTm(row - 1, col, color);
        }

    }
    bool checkTr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row < 0 || col > 7 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkTr(row - 1, col + 1, color);
        }

    }
    bool checkMl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (col < 0 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkMl(row, col - 1, color);
        }

    }
    bool checkMr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (col > 7 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkMr(row, col + 1, color);
        }

    }
    bool checkBl(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row > 7 || col < 0 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkBl(row + 1, col - 1, color);
        }
    }
    bool checkBm(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row > 7 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkBm(row + 1, col, color);
        }
    }
    bool checkBr(int row, int col, int color)
    {
        int pos = row * 8 + col;
        if (row > 7 || col > 7 || markedSpaces[pos] == -100)
        {
            return false;
        }
        else if (markedSpaces[pos] == color)
        {
            return true;
        }
        else
        {
            return checkBr(row + 1, col + 1, color);
        }
    }
}
