using ConnectFour;

public class Board
{

    // Define players
    public string Player1 { get; set; }
    public string Player2 { get; set; }
    public string EmptySpot { get; set; } = ".";
    public MCTS MCTS { get; set; }
    public string CurrentPlayer { get; set; }
    public (int, int) SelectedPosition { get; set; }
    // Define board position
    public Dictionary<(int, int), string> Position { get; set; }

    // Default constructor
    public Board(Board board = null)
    {
        if (board == null)
        {
            // initialize players
            Player1 = "x";
            Player2 = "o";
            EmptySpot = EmptySpot;
            CurrentPlayer = Player1;
            // define board position
            Position = new Dictionary<(int, int), string>();
            SelectedPosition = (-1, -1);
            // init MCTS
            MCTS = new MCTS();

            // init (reset) board
            InitBoard();

        }
        else
        {
            Player1 = board.Player1;
            Player2 = board.Player2;
            EmptySpot = board.EmptySpot;
            MCTS = board.MCTS;
            CurrentPlayer = board.CurrentPlayer;
            SelectedPosition = board.SelectedPosition;
            Position = new Dictionary<(int, int), string>(board.Position);
        }
    }
    // Init board
    public void InitBoard()
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                Position[(row, col)] = EmptySpot;
            }
        }
    }

    public void GameLoop()
    {
        Board newBoard = new Board();
        Console.WriteLine("\n   Connect Four by Steven Gates\n");
        Console.WriteLine("   Type 'exit' to quit the game");
        Console.WriteLine("   Select column: 0 being first column and 6 the last\n");

        // Print Board
        Print();
        bool isGameOver = false;
        while (!isGameOver)
        {
            var userInput = Console.ReadLine().ToLower();

            // Check if user wants to exit
            if (userInput == "exit") { Environment.Exit(0); }

            // Skip empty input
            if (userInput == "") { continue; }

            // Parse user input (move format: [col (x), row (y)]: 1,2)
            try
            {
                CurrentPlayer = Player1;
                int selectedMove = Convert.ToInt32(userInput);

                if (!newBoard.IsValidMove(selectedMove, newBoard))
                {
                    Console.WriteLine("This move is not valid. Please select another column");
                    continue;
                }

                // Make Move
                newBoard = Move(selectedMove);
                isGameOver = newBoard.IsGameOver();
                if (isGameOver)
                {
                    break;
                }
                //Console.WriteLine($"\n {CurrentPlayer} made move {newBoard.SelectedPosition}");
                Position = newBoard.Position;
                newBoard.CurrentPlayer = Player2;
                newBoard.Print();
                // Make AI move on board

                // 1. Search for the best move
                TreeNode bestMove = newBoard.MCTS.Search(newBoard);

                if (bestMove.Board != null) // Could be null if game is over
                {
                    // 2. Make the Best Move for AI
                    newBoard = bestMove.Board;
                    Position = newBoard.Position;
                    CurrentPlayer = newBoard.Player1;
                    // Print Board
                    newBoard.Print();
                }
                newBoard = Move(selectedMove);
                isGameOver = newBoard.IsGameOver();
                if (isGameOver)
                {
                    break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message}");
                Console.WriteLine("Illegal Command!");
                Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");
            }
        }
        PlayAgain();
    }

    public Board Move(int selectedMove)
    {
        Board newBoard = Clone();
        int availableRow = 0;
        // Find the lowest empty row in the selected column
        for (int row = 5; row >= 0; row--)
        {
            if (newBoard.Position[(row, selectedMove)] == EmptySpot) // or any other indicator of an empty space
            {
                availableRow = row;
                break; // Stop searching, we've made the move
            }
        }
        newBoard.Position[(availableRow, selectedMove)] = Player1;
        newBoard.SelectedPosition = (availableRow, selectedMove);
        var temp = newBoard.Player1;
        //newBoard.Player1 = newBoard.Player2;
        //newBoard.Player2 = temp;
        newBoard.CurrentPlayer = newBoard.Player1;
        return newBoard;

    }

    // Generate legal moves to play in the current position
    public List<Board> GenerateStates()
    {
        try
        {
            // Define states list (move list - list of available actions to consider)
            List<Board> actions = new List<Board>();
            for (int col = 0; col < 7; col++)
            {
                for (int row = 5; row >= 0; row--) // Start from the bottom row and move up
                {
                    if (Position[(row, col)] == EmptySpot) // Check if the space is empty
                    {
                        // Check if there is no empty space below the current spot
                        bool hasCoinBelow = false;
                        for (int r = row + 1; r < 6; r++)
                        {
                            if (Position[(r, col)] != EmptySpot)
                            {
                                hasCoinBelow = true;
                                break;
                            }
                        }

                        if (!hasCoinBelow)
                        {
                            actions.Add(GenerateStatesMove(row, col)); ; // Add the available space to the list
                            break; // Move to the next column
                        }
                    }
                    else
                    {
                        if (row > 0 && Position[(row - 1, col)] == EmptySpot)
                        {
                            actions.Add(GenerateStatesMove(row - 1, col)); // Add the available space to the list
                            break; // Move to the next column
                        }
                    }
                }
            }
            // Return the list of available actions (board class instances)
            return actions;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception {e.Message}");
            // throw new Exception();
            return new List<Board>();
        }
    }



    public bool IsGameOver()
    {
        Board verifyBoard = Clone();
        bool isWinner = IsWinner(verifyBoard);
        if (isWinner)
        {
            return true;
        }
        bool isFreeSpaceAvailable = IsFreeSpaceAvailable(verifyBoard);

        if (!isFreeSpaceAvailable)
        {
            return true;
        }
        return false;
    }

    public bool IsFreeSpaceAvailable(Board board)
    {
        for (int col = 0; col < 7; col++)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (board.Position[(row, col)] == EmptySpot)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsWinner(Board board)
    {
        // Horizontal win
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 4; col++) // Only need to check up to column 4
            {
                if (board.Position[(row, col)] == board.CurrentPlayer &&
                    board.Position[(row, col + 1)] == board.CurrentPlayer &&
                    board.Position[(row, col + 2)] == board.CurrentPlayer &&
                    board.Position[(row, col + 3)] == board.CurrentPlayer)
                {
                    return true;
                }
            }
        }

        // Vertical win
        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row < 3; row++) // Only need to check up to row 3
            {
                if (board.Position[(row, col)] == board.CurrentPlayer &&
                    board.Position[(row + 1, col)] == board.CurrentPlayer &&
                    board.Position[(row + 2, col)] == board.CurrentPlayer &&
                    board.Position[(row + 3, col)] == board.CurrentPlayer)
                {
                    return true;
                }
            }
        }

        // Diagonal win (bottom-left to top-right)
        for (int row = 5; row > 2; row--) // Start from the bottom row and move up
        {
            for (int col = 0; col < 4; col++)
            {
                if (board.Position[(row, col)] == board.CurrentPlayer &&
                    board.Position[(row - 1, col + 1)] == board.CurrentPlayer &&
                    board.Position[(row - 2, col + 2)] == board.CurrentPlayer &&
                    board.Position[(row - 3, col + 3)] == board.CurrentPlayer)
                {
                    return true;
                }
            }
        }

        // Diagonal win (bottom-right to top-left)
        for (int row = 5; row > 2; row--) // Start from the bottom row and move up
        {
            for (int col = 3; col < 7; col++)
            {
                if (board.Position[(row, col)] == board.CurrentPlayer &&
                    board.Position[(row - 1, col - 1)] == board.CurrentPlayer &&
                    board.Position[(row - 2, col - 2)] == board.CurrentPlayer &&
                    board.Position[(row - 3, col - 3)] == board.CurrentPlayer)
                {
                    return true;
                }
            }
        }

        return false; // No winner found
    }

    public void PlayAgain()
    {
        Console.WriteLine($"Play again? Press 'y' to continue else any other character.");
        var playAgain = Console.ReadLine().ToLower();
        if (playAgain == "y")
        {
            // Create board instance
            Board board = new Board();
            board.GameLoop();
        }
        else
        {
            Environment.Exit(0);
        }
    }

    public void Print()
    {
        string boardString = String.Empty;
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                boardString += $" {Position[(row, col)]}";
            }
            boardString += "\n";
        }
        Console.WriteLine($"{boardString}");

        string playerString = String.Empty;
        if (CurrentPlayer == "x")
        {
            playerString = $"\n ---------- \n 'x' Turn: \n ---------- \n";
        }
        else
        {
            playerString = $"\n ---------- \n 'o' Turn: \n ---------- \n";
        }
        Console.WriteLine($"{playerString}");
    }

    public Board Clone()
    {
        Board newBoard = new Board()
        {
            Player1 = string.Copy(Player1),
            Player2 = string.Copy(Player2),
            EmptySpot = string.Copy(EmptySpot),
            CurrentPlayer = string.Copy(CurrentPlayer),
            Position = new Dictionary<(int, int), string>(Position),
            SelectedPosition = SelectedPosition
        };
        return newBoard;
    }

    public bool IsValidMove(int col, Board board)
    {
        for (int row = 0; row < 6; row++)
        {
            if (board.Position[(row, col)] == EmptySpot) // or any other indicator of an empty space
            {
                return true; // Move is valid
            }
        }
        return false; // Move is invalid (column is full)
    }

    public Board GenerateStatesMove(int row, int col)
    {
        Board newBoard = Clone();
        newBoard.Position[(row, col)] = CurrentPlayer;
        newBoard.SelectedPosition = (row, col);
        newBoard.CurrentPlayer = (newBoard.CurrentPlayer == Player1) ? Player2 : Player1;
        return newBoard;
    }
}
