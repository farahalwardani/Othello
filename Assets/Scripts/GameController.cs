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
    void Start(){
        startButton.SetActive(true);
        stopButton.SetActive(false);
    }

    public void StartGame(){
        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);
        levels.interactable = false ;
        ResetGame();
        moveListUI.text = " ";
        nbMove = 0;
        moveListUI.text += "\n" + "<Start Game> " + "\n";
    }

    public void StopGame(){
        moveListUI.text = " ";
        moveListUI.text += "\n" + "<Stop Game> " + "\n";
        ResetGame();
        clearPossMove();
        possMoveListUI.text = " " ;
        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);
        levels.interactable = true;
    }

    void ResetGame(){
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
        clearPossMove();
        ArrayList possibleMoves = checkPossibleMove(markedSpaces, whoTurn);
        printPossibleMove(possibleMoves);
        showPossibleMove(possibleMoves);
    }

    public void HandleInputData(int val){
        if (val == 0)
          depth = 1;
        else if (val == 1)
           depth = 4;
        else if (val == 2)
            depth = 7;
        else depth = 1;
    }

/*    void gameSetup(){
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
                markedSpaces[i] = -1;
            }
        }
        TextScore[0].text = "" + checkBlackScore(markedSpaces);
        TextScore[1].text = "" + checkWhiteScore(markedSpaces);
        clearPossMove();
        ArrayList possibleMoves = checkPossibleMove(markedSpaces, whoTurn);
        printPossibleMove(possibleMoves);
        showPossibleMove(possibleMoves);
    }*/

    // Update is called once per frame
    void Update(){
    }

    void printPossibleMove(ArrayList possibleMove){
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

    public void othelloButton(int whichNumber){
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
            ArrayList possibleMove = checkPossibleMove(markedSpaces, whoTurn);
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
                    possibleMove = checkPossibleMove(markedSpaces, whoTurn);
                    if (possibleMove.Count > 0){
                        showPossibleMove(possibleMove);
                        printPossibleMove(possibleMove);
                    }
                    else //if the 2 players have no possible moves
                        Debug.Log("game ended .. no more possible moves");
                }
                else
                {
                    Debug.Log("Black pass");
                    whoTurn = 1;
                    turnIcons[1].SetActive(true);
                    turnIcons[0].SetActive(false);
                    clearPossMove();
                    possibleMove = checkPossibleMove(markedSpaces, whoTurn);
                    if (possibleMove.Count > 0){
                        showPossibleMove(possibleMove);
                        printPossibleMove(possibleMove);
                        StartCoroutine(AIPlay(possibleMove));
                    }
                    else     //if the 2 players have no possible moves
                        Debug.Log("game ended .. no more possible moves");
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

    IEnumerator AIPlay(ArrayList possibleMove){ //always possibleMoves is ensured not to be empty when this coroutine is called
        yield return new WaitForSeconds(0.1F);
        int[] markedSpacesClone = (int[]) markedSpaces.Clone();    //'cause when i'm giving the markedSpaces to the bestMove function it takes the reference.
        int i = bestMove(markedSpacesClone, depth);
        if (i >= 0 && i < possibleMove.Count)
            othelloSpaces[(int)possibleMove[i]].onClick.Invoke();
        yield return null;
    }

    int bestMove(int[] board, int depth){
        ArrayList possibleMove = checkPossibleMove(board, whoTurn);
        int move = 0;

        if (depth == 1) {
            System.Random random = new System.Random();
            move = random.Next(possibleMove.Count);
        }

        if (possibleMove.Count == 1)
            return move;

        else { //bestMove is used by white which is minimizing
            double Infinity = double.PositiveInfinity;
            double bestScore = -Infinity;
            double alpha = -Infinity;
            double beta = Infinity;

            for(int i = 0; i < possibleMove.Count; i++){
                //if possible move is a corner, directly play it
                int index = (int)possibleMove[i];
                if (index == 0 || index == 7 || index == 56 || index == 63)
                    return i;
            }

            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 2;
                int score = minimax(board, depth, false, alpha, beta);
                board[(int)possibleMove[i]] = -1;
                if (score <= bestScore)
                {
                    bestScore = score;
                    move = i;
                }
                beta = Math.Min(beta, bestScore);
                if (beta <= alpha)
                    break;
            }
        }
        return move;
    }

    //black is maximizing and white is minimizing
    int minimax(int[] board, int depth, bool isMaximizing, double alpha, double beta){
        double Infinity = double.PositiveInfinity;
        int turn = 1;
        if(isMaximizing)
            turn = 0;
        ArrayList possibleMove = checkPossibleMove(board, turn);           //turn 0 corresponds to black, 1 to white

        if (depth == 0)
            return evaluationFct(board);

        //check if there's no possible moves
        if (possibleMove.Count == 0) {
            if (checkPossibleMove(board, 1 - turn).Count == 0) //game finished
                return evaluationFct(board);
            return minimax(board, depth, !isMaximizing, alpha, beta);
        }

        if (isMaximizing)
        {
            double bestScore = -Infinity;
            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 2;
                int score = minimax(board, depth - 1, false, alpha, beta);
                board[(int)possibleMove[i]] = -1;
                bestScore = Math.Max(score, bestScore);
                alpha = Math.Max(alpha, bestScore);
                if (beta <= alpha)
                    break;
            }
            return (int)bestScore;
        }
        else   //if minimizing player
        {
            double bestScore = Infinity;
            for (int i = 0; i < possibleMove.Count; i++)
            {
                board[(int)possibleMove[i]] = 1;
                int score = minimax(board, depth - 1, true, alpha, beta);
                board[(int)possibleMove[i]] = -1;
                bestScore = Math.Min(score, bestScore);
                beta = Math.Min(beta, bestScore);
                if (beta <= alpha)
                    break;
            }
            return (int)bestScore;
        }
    }

    //Evaluation function of the board : black is the maximizing player and white is the minimizing
    int evaluationFct(int[] board){
        //piece difference (p), frontier disks (f), and static factor (s)
        int maxPlayerCoins = 0; int maxPlayerFrontCoins = 0;
        int minPlayerCoins = 0; int minPlayerFrontCoins = 0;
        double p = 0; double f = 0; double s = 0; double c = 0; double l = 0; double m = 0;
        int[] neighborIndices = {-1, -1, -1, -1, -1, -1, -1, -1};
        int[] V = {20, -3, 11, 8, 8, 11, -3, 20,
                    3, -7, -4, 1, 1, -4, -7, -3,
                    11, -4, 2, 2, 2, 2, -4, 11,
                    8, 1, 2, -3, -3, 2, 1, 8,
                    8, 1, 2, -3, -3, 2, 1, 8,
                    11, -4, 2, 2, 2, 2, -4, 11,
                    -3, -7, -4, 1, 1, -4, -7, -3,
                    20, -3, 11, 8, 8, 11, -3, 20};

        for (int i = 0; i < board.Length; i++){
            for (int x = 0; x < neighborIndices.Length; x++)
                neighborIndices[x] = -1;
            if (board[i] == 1){ //max,black
                s += V[i];
                maxPlayerCoins++;
            }
            else if (board[i] == 2){
                s -= V[i];
                minPlayerCoins++;
            }

            if (board[i] != -1){
                //corners
                if (i == 0){
                    neighborIndices[0] = 1;
                    neighborIndices[1] = 8;
                    neighborIndices[2] = 9;
                }
                else if (i == 7){
                    neighborIndices[0] = 6;
                    neighborIndices[1] = 14;
                    neighborIndices[2] = 15;
                }
                else if (i == 56){
                    neighborIndices[0] = 48;
                    neighborIndices[1] = 49;
                    neighborIndices[2] = 57;
                }
                else if (i == 63){
                    neighborIndices[0] = 54;
                    neighborIndices[1] = 55;
                    neighborIndices[2] = 62;
                }
                //top
                else if (i >= 1 && i <= 6){
                    neighborIndices[0] = i - 1;
                    neighborIndices[1] = i + 1;
                    neighborIndices[2] = i + 9;
                    neighborIndices[3] = i + 8;
                    neighborIndices[4] = i + 7;
                }
                //left
                else if (i % 8 == 0 && i != 0 && i != 56){
                    neighborIndices[0] = i - 8;
                    neighborIndices[1] = i + 1;
                    neighborIndices[2] = i + 9;
                    neighborIndices[3] = i + 8;
                    neighborIndices[4] = i - 7;
                }
                //bottom
                else if (i >= 57 && i <= 62){
                    neighborIndices[0] = i - 1;
                    neighborIndices[1] = i + 1;
                    neighborIndices[2] = i - 9;
                    neighborIndices[3] = i - 8;
                    neighborIndices[4] = i - 7;
                }
                //right
                else if ((i + 1) % 8 == 0 && i != 7 && i != 63){
                    neighborIndices[0] = i - 1;
                    neighborIndices[1] = i + 7;
                    neighborIndices[2] = i - 9;
                    neighborIndices[3] = i - 8;
                    neighborIndices[4] = i + 8;
                }

                for (int k = 0; k < neighborIndices.Length; k++){
                    if (neighborIndices[k] != -1){
                        if (board[neighborIndices[k]] == -1){
                            if (board[i] == 1)
                                maxPlayerFrontCoins++;
                            else
                                minPlayerFrontCoins++;
                            break;
                        }
                    }
                }
            }
        }

        if(maxPlayerCoins > minPlayerCoins)
            p = (100.0 * maxPlayerCoins)/(maxPlayerCoins + minPlayerCoins);
	    else if(maxPlayerCoins < minPlayerCoins)
		    p = -(100.0 * minPlayerCoins)/(maxPlayerCoins + minPlayerCoins);
	    else p = 0;

	    if(maxPlayerFrontCoins > minPlayerFrontCoins)
            f = -(100.0 * maxPlayerFrontCoins)/(maxPlayerFrontCoins + minPlayerFrontCoins);
	    else if(maxPlayerFrontCoins < minPlayerFrontCoins)
		    f = (100.0 * minPlayerFrontCoins)/(maxPlayerFrontCoins + minPlayerFrontCoins);
	    else f = 0;



        //mobility (m)
        int maxPlayerMoves = checkPossibleMove(board, 0).Count;
        int minPlayerMoves = checkPossibleMove(board, 1).Count;
        if(maxPlayerMoves > minPlayerMoves)
		      m = (100.0 * maxPlayerMoves)/(maxPlayerMoves + minPlayerMoves);
	    else if(maxPlayerMoves < minPlayerMoves)
		      m = -(100.0 * minPlayerMoves)/(maxPlayerMoves + minPlayerMoves);
	    else m = 0;

        //corners occupancy (c)
        int maxPlayerCorners = 0;
        int minPlayerCorners = 0;
        int[] cornerIndices = {0, 7, 56, 63};
        for (int i = 0; i < 4; i++){
            if (board[cornerIndices[i]] == 1)
                maxPlayerCorners++;
            else if (board[cornerIndices[i]] == 2)
                minPlayerCorners++;
        }
        c = 25 * (maxPlayerCorners - minPlayerCorners);


        //corner closeness (l)
        maxPlayerCoins = 0;
        minPlayerCoins = 0;
        if (board[0] == -1){
            if (board[1] == 1)
                maxPlayerCoins++;
            else if (board[1] == 2)
                minPlayerCoins++;
            if (board[8] == 1)
                maxPlayerCoins++;
            else if (board[8] == 2)
                minPlayerCoins++;
            if (board[9] == 1)
                maxPlayerCoins++;
            else if (board[9] == 2)
                minPlayerCoins++;
        }

        if (board[7] == -1){
            if (board[6] == 1)
                maxPlayerCoins++;
            else if (board[6] == 2)
                minPlayerCoins++;
            if (board[14] == 1)
                maxPlayerCoins++;
            else if (board[14] == 2)
                minPlayerCoins++;
            if (board[15] == 1)
                maxPlayerCoins++;
            else if (board[15] == 2)
                minPlayerCoins++;
        }
        if (board[56] == -1){
            if (board[48] == 1)
                maxPlayerCoins++;
            else if (board[48] == 2)
                minPlayerCoins++;
            if (board[49] == 1)
                maxPlayerCoins++;
            else if (board[49] == 2)
                minPlayerCoins++;
            if (board[57] == 1)
                maxPlayerCoins++;
            else if (board[57] == 2)
                minPlayerCoins++;
        }
        if (board[63] == -1){
            if (board[62] == 1)
                maxPlayerCoins++;
            else if (board[62] == 2)
                minPlayerCoins++;
            if (board[54] == 1)
                maxPlayerCoins++;
            else if (board[54] == 2)
                minPlayerCoins++;
            if (board[55] == 1)
                maxPlayerCoins++;
            else if (board[55] == 2)
                minPlayerCoins++;
        }
        l = -12.5 * (maxPlayerCoins - minPlayerCoins);


        double finalEval = (10 * p) + (800 * c) + (400 * l) + (80 * m) + (70 * f) + (10 * s);
        return (int)finalEval;
    }


/*    int evaluationFct(int[] board){
        return (checkBlackScore(board) - checkWhiteScore(board));
    }*/

    int checkBlackScore(int[] board){
        int blackScore = 0;
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 1)
                blackScore++;
        }
        return blackScore;
    }

    int checkWhiteScore(int[] board){
        int whiteScore = 0;
        for(int i=0; i< board.Length; i++)
        {
            if (board[i] == 2)
                whiteScore++;
        }
        return whiteScore;
    }

    void flipPiece(int pos){
        int row = pos / 8;
        int col = pos % 8;
        flipALL(row, col, markedSpaces[pos]);
    }

    ArrayList checkPossibleMove(int [] board, int turn){
        ArrayList possibleMove = new ArrayList();

        if (turn == 0)
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
                            if (board[(row + 1) * 8 + (col + 1)] == -1)
                            {
                                if(!possibleMove.Contains((row + 1) * 8 + (col + 1)))
                                    possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -1)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col)))
                                    possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 1))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -1)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col - 1)))
                                    possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -1)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col + 1)))
                                    possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 1))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -1)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col - 1)))
                                    possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -1)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col + 1)))
                                    possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -1)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col)))
                                    possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 1))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -1)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col - 1)))
                                    possibleMove.Add((row - 1) * 8 + (col - 1));
                            }
                    }
                }
            }
        }
        else if (turn == 1)
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
                            if (board[(row + 1) * 8 + (col + 1)] == -1)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col + 1)))
                                    possibleMove.Add((row + 1) * 8 + (col + 1));
                            }
                    }
                    if (checkTm(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row + 1) * 8 + (col)] == -1)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col)))
                                    possibleMove.Add((row + 1) * 8 + (col));
                            }
                    }
                    if (checkTr(row, col, 2))
                    {
                        if ((row + 1) >= 0 && (row + 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row + 1) * 8 + (col - 1)] == -1)
                            {
                                if (!possibleMove.Contains((row + 1) * 8 + (col - 1)))
                                    possibleMove.Add((row + 1) * 8 + (col - 1));
                            }
                    }
                    if (checkMl(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row) * 8 + (col + 1)] == -1)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col + 1)))
                                    possibleMove.Add((row) * 8 + (col + 1));
                            }
                    }
                    if (checkMr(row, col, 2))
                    {
                        if ((row) >= 0 && (row) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row) * 8 + (col - 1)] == -1)
                            {
                                if (!possibleMove.Contains((row) * 8 + (col - 1)))
                                    possibleMove.Add((row) * 8 + (col - 1));
                            }
                    }
                    if (checkBl(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col + 1) >= 0 && (col + 1) <= 7)
                            if (board[(row - 1) * 8 + (col + 1)] == -1)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col + 1)))
                                    possibleMove.Add((row - 1) * 8 + (col + 1));
                            }
                    }
                    if (checkBm(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col) >= 0 && (col) <= 7)
                            if (board[(row - 1) * 8 + (col)] == -1)
                            {
                                if (!possibleMove.Contains((row - 1) * 8 + (col)))
                                    possibleMove.Add((row - 1) * 8 + (col));
                            }
                    }
                    if (checkBr(row, col, 2))
                    {
                        if ((row - 1) >= 0 && (row - 1) <= 7 && (col - 1) >= 0 && (col - 1) <= 7)
                            if (board[(row - 1) * 8 + (col - 1)] == -1)
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

    void showPossibleMove(ArrayList moves){
        for(int i=0; i<moves.Count; i++)
        {
            othelloSpaces[(int)moves[i]].image.sprite = playerIcons[2];
            othelloSpaces[(int)moves[i]].interactable = true;
        }
    }

    void clearPossMove(){
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if(markedSpaces[i] == -1)
            {
                othelloSpaces[i].image.sprite = null;
                othelloSpaces[i].interactable = false;
            }
        }
    }

    bool flipTl(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col - 1);
        if (row < 1 || col < 1 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipTm(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col);
        if (row < 1 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipTr(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row - 1) * 8 + (col + 1);
        if (row < 1 || col > 6 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipMl(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row) * 8 + (col - 1);
        if (col < 1 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipMr(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row) * 8 + (col + 1);
        if (col > 6 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipBl(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + (col - 1);
        if (row > 6 || col < 1 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipBm(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + col;
        if (row > 6 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    bool flipBr(int row, int col, int color){
        int pos = row * 8 + col;
        int posNeighbor = (row + 1) * 8 + (col + 1);
        if (row > 6 || col > 6 || markedSpaces[posNeighbor] == -1)
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
        else //if (markedSpaces[posNeighbor] != color && markedSpaces[posNeighbor] != -1)
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

    void flipALL(int row, int col, int color){
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

    bool checkTl(int row, int col, int color){
        int pos = row * 8 + col;
        if (row < 0 || col < 0 || markedSpaces[pos] == -1)
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

    bool checkTm(int row, int col, int color){
        int pos = row * 8 + col;
        if (row < 0 || markedSpaces[pos] == -1)
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

    bool checkTr(int row, int col, int color){
        int pos = row * 8 + col;
        if (row < 0 || col > 7 || markedSpaces[pos] == -1)
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

    bool checkMl(int row, int col, int color){
        int pos = row * 8 + col;
        if (col < 0 || markedSpaces[pos] == -1)
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

    bool checkMr(int row, int col, int color){
        int pos = row * 8 + col;
        if (col > 7 || markedSpaces[pos] == -1)
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

    bool checkBl(int row, int col, int color){
        int pos = row * 8 + col;
        if (row > 7 || col < 0 || markedSpaces[pos] == -1)
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

    bool checkBm(int row, int col, int color){
        int pos = row * 8 + col;
        if (row > 7 || markedSpaces[pos] == -1)
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

    bool checkBr(int row, int col, int color){
        int pos = row * 8 + col;
        if (row > 7 || col > 7 || markedSpaces[pos] == -1)
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
