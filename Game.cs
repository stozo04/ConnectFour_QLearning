using ConnectFour;
using System;

public class Game
{
    private int[,] board; // 2D array to represent the game board
    private int currentPlayer; // Current player's turn (1 or 2)
    private bool isGameOver; // Flag to indicate if the game is over
    private MonteCarloTreeSearch mcts; // MCTS object

    public int CurrentPlayer
    {
        get { return currentPlayer; }
    }

    public Game()
    {
        // 6 rows and 7 column=s for the game board
        board = new int[6, 7];
        // Initialize the MCTS algorithm
        mcts = new MonteCarloTreeSearch();
        // Player '1' starts the game
        currentPlayer = 1;
        // Game starts as not over
        isGameOver = false;
    }

    //public double PlayGame()
    //{
    //    board = new int[6, 7];
    //    bool isGameOver = false;
    //    double totalReward = 0.0;

    //    while (!isGameOver)
    //    {
    //        int currentState = GetCurrentGameState();  // Default state
    //        int action = ChooseAction(currentState);  // Default action
    //        bool p1MoveSuccessful = false;
    //        while (!p1MoveSuccessful)
    //        {
    //            currentState = GetCurrentGameState();
    //            action = ChooseAction(currentState);
    //            p1MoveSuccessful = MakeMove(action);
    //        }


    //        PrintBoard();
    //        totalReward += GetReward();
    //        int nextState = GetCurrentGameState();
    //        qLearning.UpdateQValue(totalReward, currentState, nextState, action);

    //        Console.Write("Player 1 Moved.");

    //        // Player 2's move (you could use a random move, another AI, etc.)

    //        bool p2MoveSuccessful = false;
    //        while (!p2MoveSuccessful)
    //        {
    //            Random random = new Random();
    //            int randomNumber = random.Next(0, 7); // Generates a random number between 0 and 6 (exclusive of 7)
    //            p2MoveSuccessful = MakeMove(randomNumber);
    //        }
    //        PrintBoard();

    //        Console.Write("Player 2 Moved.");
    //    }

    //    return totalReward;
    //}

    public void PlayGame()
    {
        while (!isGameOver)
        {
            State state = new State() { Board = this.board, PlayerNo = this.currentPlayer };
            Node node = new Node() { State = state };
            Tree tree = new Tree() { Root = node };

            int action = mcts.FindNextMove(tree, -1);

            MakeMove(action);
            PrintBoard();
           // Console.Write("Player 1 Moved.");
            if (CheckWinner())
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
                isGameOver = true;
            }

            if (CheckDraw())
            {
                Console.WriteLine("The game is a draw.");
                isGameOver = true;
            }

            SwitchPlayer();
        }
    }




    public bool MakeMove(int column)
    {
        // Check if the selected column is within the valid range
        if (column < 0 || column > 6)
        {
            Console.WriteLine("Invalid number.");
            return false;
        }

        // Check if the move is valid (i.e., the column isn't full)
        if (board[0, column] != 0)
        {
            Console.WriteLine("Column is Full.");
            return false;
        }

        // Find the lowest available row in the column
        for (int row = 5; row >= 0; row--)
        {
            if (board[row, column] == 0)
            {
                // Place the current player's piece in the found row
                board[row, column] = currentPlayer;

                // Check if the game is won
                if (CheckWinner())
                {
                    Console.WriteLine("The game is over.");
                    isGameOver = true;
                }

                // Check for a draw
                if (CheckDraw())
                {
                    Console.WriteLine("The game is a draw.");
                    isGameOver = true;
                }

                return true;
            }
        }

        // If no empty row was found in the selected column, the move is invalid
        return false;
    }

    public void SwitchPlayer()
    {
        // Switch to the other player
        currentPlayer = 3 - currentPlayer;
    }

    public bool CheckWinner()
    {
        // Check horizontal, vertical, and diagonal lines for a win
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                // Ignore cells that are still empty
                if (board[i, j] == 0)
                {
                    continue;
                }

                if (CheckLine(i, j, 0, 1) || // Checks a horizontal line from left to right.
                    CheckLine(i, j, 1, 0) || // Checks a vertical line from top to bottom.
                    CheckLine(i, j, 1, 1) || // Checks a diagonal line from top-left to bottom-right.
                    CheckLine(i, j, 1, -1))  // Checks an anti-diagonal line from bottom-left to top-right.
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckDraw()
    {
        for (int c = 0; c < 7; c++)
        {
            if (board[0, c] == 0) // Check against 0 not '\0'
            {
                return false; // There is an empty space, game not over
            }
        }
        return true; // All spaces filled, game is a draw
    }


    public void PrintBoard()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }


}
