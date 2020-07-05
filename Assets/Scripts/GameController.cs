using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int whoTurn;
    public int turnCount;
    public GameObject[] turnIcons;
    public Sprite[] playerIcons;
    public Button[] othelloSpaces;
    public int[] markedSpaces;

    // Start is called before the first frame update
    void Start()
    {
        gameSetup();
    }

    void gameSetup()
    {
        whoTurn = 0;
        turnCount = 0;
        for(int i=0; i<othelloSpaces.Length; i++)
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
        for (int i = 0; i < possibleMove.Count; i++)
        {
            s += possibleMove[i] + " ";
        }
        Debug.Log("Possible Moves : " + s);
    }
    public void othelloButton(int whichNumber)
    {
        othelloSpaces[whichNumber].image.sprite = playerIcons[whoTurn];
        othelloSpaces[whichNumber].interactable = false;

        markedSpaces[whichNumber] = whoTurn+1;
        turnCount++;
        /*
        int[] Neighbors = getNeighbors(whichNumber);
        String names = "" ;
        for (int i = 0; i <Neighbors.Length; i++)
        {
            names += Neighbors[i] + " ";
            //othelloSpaces[Neighbors[i]].image.sprite = playerIcons[2];
        }
        Debug.Log(names);

        //int[] oldNeighbors = Neighbors;
        //int oldWhichNumber = whichNumber;
        */
        if (whoTurn == 0)
        {
            whoTurn = 1;
        }
        else
        {
            whoTurn = 0;
        }
        clearPossMove();
        flipPiece(whichNumber);
        //checkPossibleMove();
        ArrayList possibleMove = checkPossibleMove(markedSpaces);
        showPossibleMove(possibleMove);
        printPossibleMove(possibleMove);

        Debug.Log("Black Score : " + checkBlackScore(markedSpaces) + " / White Score : " + checkWhiteScore(markedSpaces));

        if (possibleMove.Count == 0)
        {
            if (whoTurn == 1)
            {
                whoTurn = 0;
                showPossibleMove(checkPossibleMove(markedSpaces)); 
            }
            else
            {
                whoTurn = 1;
                showPossibleMove(checkPossibleMove(markedSpaces));
            }
        }
        else
        {
            if (whoTurn == 1)
            {
                StartCoroutine(AIPlay(possibleMove, 1));
            }
        }
    }
    IEnumerator AIPlay(ArrayList possibleMove, int level)
    {
        if(level == 1)
        {
            yield return new WaitForSeconds(0.1F);
            othelloSpaces[(int)possibleMove[0]].onClick.Invoke();
            yield return null;
        }
        else if(level == 2)
        {
            yield return new WaitForSeconds(0.1F);
            int i = bestMove(markedSpaces, 1000);
            othelloSpaces[(int)possibleMove[i]].onClick.Invoke();
            yield return null;
        }
    }
    int bestMove(int[] board, int depth)
    {
        double Infinity = double.PositiveInfinity;
        double bestScore = -Infinity;
        int move = 0;
        ArrayList possibleMove = checkPossibleMove(board);

        for (int i = 0; i < possibleMove.Count; i++)
        {
            board[i] = 2;
            int score = minimax(board, depth, false);
            board[i] = -100;
            if(score > bestScore)
            {
                bestScore = score;
                move = i;
            }
        }
        return move;
    }
    int minimax(int[] board, int depth, bool isMaximizing)
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
                board[i] = 2;
                int score = minimax(board, depth - 1, false);
                board[i] = -100;
                bestScore = Math.Max(score, bestScore);
            }
            return (int)bestScore;
        }
        else
        {
            double bestScore = Infinity;
            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[i] = 1;
                int score = minimax(board, depth - 1, true);
                board[i] = -100;
                bestScore = Math.Min(score, bestScore);
            }
            return (int)bestScore;
        }
    }
    public void quit()
    {
        Application.Quit();
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
                                possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -100)
                            {
                                possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -100)
                            {
                                possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -100)
                            {
                                possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -100)
                            {
                                possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -100)
                            {
                                possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -100)
                            {
                                possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -100)
                            {
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
                                possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -100)
                            {
                                possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -100)
                            {
                                possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -100)
                            {
                                possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -100)
                            {
                                possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -100)
                            {
                                possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -100)
                            {
                                possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -100)
                            {
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
    int[] getNeighbors(int buttonPos)
    {
        int row = buttonPos / 8;
        int col = buttonPos % 8;

        int tl, tm, tr;
        int ml, mr;
        int bl, bm, br;
        int[] neighbors;

        tl = (row - 1) * 8 + (col - 1);
        tm = (row - 1) * 8 + col;
        tr = (row - 1) * 8 + (col + 1);
        ml = row * 8 + (col - 1);
        mr = row * 8 + (col + 1);
        bl = (row + 1) * 8 + (col - 1);
        bm = (row + 1) * 8 + col;
        br = (row + 1) * 8 + (col + 1);

        if(row == 0)
        {
            if(col == 0)
            {
                neighbors = new int[] {mr, bm, br};
            }
            else if(col == 7)
            {
                neighbors = new int[] { ml, bl, bm };
            }
            else
            {
                neighbors = new int[] { ml, mr, bl, bm, br};
            }
        }
        else if (row == 7)
        {
            if (col == 0)
            {
                neighbors = new int[] { tm, tr, mr };
            }
            else if (col == 7)
            {
                neighbors = new int[] { tl, tm, ml };
            }
            else
            {
                neighbors = new int[] { tl, tm, tr, ml, mr };
            }
        }
        else if (col == 0)
        {
            neighbors = new int[] { tm, tr, mr, bm, br };
        }
        else if (col == 7)
        {
            neighbors = new int[] { tl, tm, ml, bl, bm };
        }
        else
        {
            neighbors = new int[] { tl, tm, tr, ml, mr, bl, bm, br };
        }

        return neighbors;
    }
    IDictionary<int, string> getNeighborsLocation(int buttonPos)
    {
        int row = buttonPos / 8;
        int col = buttonPos % 8;

        int tl, tm, tr;
        int ml, mr;
        int bl, bm, br;

        IDictionary<int, string> neighbors = new Dictionary<int, string>();

        tl = (row - 1) * 8 + (col - 1);
        tm = (row - 1) * 8 + col;
        tr = (row - 1) * 8 + (col + 1);
        ml = row * 8 + (col - 1);
        mr = row * 8 + (col + 1);
        bl = (row + 1) * 8 + (col - 1);
        bm = (row + 1) * 8 + col;
        br = (row + 1) * 8 + (col + 1);

        if (row == 0)
        {
            if (col == 0)
            {
                neighbors.Add(mr, "mr");
                neighbors.Add(bm, "bm");
                neighbors.Add(br, "br");
            }
            else if (col == 7)
            {
                neighbors.Add(ml, "ml");
                neighbors.Add(bl, "bl");
                neighbors.Add(bm, "bm");
            }
            else
            {
                neighbors.Add(ml, "ml");
                neighbors.Add(mr, "mr");
                neighbors.Add(bl, "bl");
                neighbors.Add(bm, "bm");
                neighbors.Add(br, "br");
            }
        }
        else if (row == 7)
        {
            if (col == 0)
            {
                neighbors.Add(tl, "tl");
                neighbors.Add(tr, "tr");
                neighbors.Add(mr, "mr");
            }
            else if (col == 7)
            {
                neighbors.Add(tl, "tl");
                neighbors.Add(tm, "tm");
                neighbors.Add(ml, "ml");
            }
            else
            {
                neighbors.Add(tl, "tl");
                neighbors.Add(tm, "tm");
                neighbors.Add(tr, "tr");
                neighbors.Add(ml, "ml");
                neighbors.Add(mr, "mr");
            }
        }
        else if (col == 0)
        {
            neighbors.Add(tm, "tm");
            neighbors.Add(tr, "tr");
            neighbors.Add(mr, "mr");
            neighbors.Add(bm, "bm");
            neighbors.Add(br, "br");
        }
        else if (col == 7)
        {
            neighbors.Add(tl, "tl");
            neighbors.Add(tm, "tm");
            neighbors.Add(ml, "ml");
            neighbors.Add(bl, "bl");
            neighbors.Add(bm, "bm");
        }
        else
        {
            neighbors.Add(tl, "tl");
            neighbors.Add(tm, "tm");
            neighbors.Add(tr, "tr");
            neighbors.Add(ml, "ml");
            neighbors.Add(mr, "mr");
            neighbors.Add(bl, "bl");
            neighbors.Add(bm, "bm");
            neighbors.Add(br, "br");
        }

        return neighbors;
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
