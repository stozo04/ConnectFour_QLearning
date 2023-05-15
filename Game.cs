using ConnectFour;

public class Game
{
    private int[,] board; // 2D array to represent the game board
    private int currentPlayer; // Current player's turn (1 or 2)
    private bool isGameOver; // Flag to indicate if the game is over
    private MonteCarloTreeSearch mcts; // MCTS object

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


    public void PlayGame()
    {
        while (!isGameOver)
        {
            Console.Clear();
            PrintBoard();
            if (currentPlayer == 1)
            {
                Console.Write("Your move (enter column number): ");
                int move = Convert.ToInt32(Console.ReadLine());

                // Check if the move is valid
                while (!IsValidMove(move))
                {
                    Console.Write("Invalid move. Try again: ");
                    move = Convert.ToInt32(Console.ReadLine());
                }

                MakeMove(move);
                PrintBoard();
                Console.Write("Player 1 Moved.");
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

            }
            else
            {
                State state = new State(this.board, 2);
                Node node = new Node(state);
                Tree tree = new Tree(node);
                int action = mcts.FindNextMove(tree, -1);
                MakeMove(action);
                PrintBoard();
                Console.Write("Player 2 Moved.");

            }

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

    public bool IsValidMove(int column)
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

        return true;
    }

    public void MakeMove(int column)
    {
      

        // Find the lowest available row in the column
        for (int row = 5; row >= 0; row--)
        {
            if (board[row, column] == 0)
            {
                // Place the current player's piece in the found row
                board[row, column] = currentPlayer;
            }
        }

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

    private bool CheckLine(int row, int col, int dRow, int dCol)
    {
        // Starting from (row, col), checks for four identical pieces in the direction (dRow, dCol)
        double start = board[row, col];

        // Ignore lines that start with an empty cell
        if (start == 0)
        {
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            int curRow = row + i * dRow;
            int curCol = col + i * dCol;

            // Check if out of bounds or not the same player
            if (curRow < 0 || curRow >= 6 || curCol < 0 || curCol >= 7 || board[curRow, curCol] != start)
            {
                return false;
            }
        }

        return true;
    }
}
