using ConnectFour;
using System;

public class Game
{
    private int[,] board; // 2D array to represent the game board
    private QLearning qLearning;
    private Dictionary<string, double[]> qTable; // Dictionary to store the Q-values for each state-action pair
    private int currentPlayer; // Current player's turn (1 or 2)
    private bool isGameOver; // Flag to indicate if the game is over
    private double epsilon = 0.1; // for ε-greedy action selection

    public int CurrentPlayer
    {
        get { return currentPlayer; }
    }

    public Game()
    {
        // 6 rows and 7 column=s for the game board
        board = new int[6, 7];
        // Initialize the Q-Learning algorithm
        qLearning = new QLearning();
        
        if(qTable != null)
        {
            // Print each entry in the Q-table
            foreach (var entry in qTable)
            {
                Console.WriteLine($"State {entry.Key}: {string.Join(", ", entry.Value)}");
            }
        }
        

        // Player '1' starts the game
        currentPlayer = 1;
        // Game starts as not over
        isGameOver = false;
    }

    //public void PlayGame()
    //{
    //    while (!isGameOver)
    //    {
    //        int currentState = GetCurrentGameState();
    //        int action = ChooseAction(currentState);
    //        MakeMove(action);
    //        int nextState = GetCurrentGameState();
    //        double reward = GetReward();
    //        qLearning.UpdateQValue(reward, currentState, nextState, action);
    //        SwitchPlayer();
    //    }
    //}

    public double PlayGame()
    {
        board = new int[6, 7];
        bool isGameOver = false;
        double totalReward = 0.0;

        while (!isGameOver)
        {
            int currentState = GetCurrentGameState();
            int action = ChooseAction(currentState);
            isGameOver = MakeMove(action);
            PrintBoard();
            totalReward += GetReward();
            int nextState = GetCurrentGameState();
            qLearning.UpdateQValue(totalReward, currentState, nextState, action);


            // Player 2's move (you could use a random move, another AI, etc.)
            Random random = new Random();
            int randomNumber = random.Next(0, 7); // Generates a random number between 0 and 6 (exclusive of 7)
            isGameOver = MakeMove(randomNumber);
            PrintBoard();
        }

        return totalReward;
    }

    public int ChooseAction(int state)
    {
        // Epsilon - greedy action selection
        Random rand = new Random();
        if (rand.NextDouble() < epsilon)
        {
            // Explore: choose a random action
            return rand.Next(0, 7);
        }
        else
        {
            // Get the Q-values for the state
            double[] qValues = qLearning.GetQValues(state);

            // Exploit: choose the action with the max Q-value
            return GetMaxAction(qValues);
        }
    }

    public int GetMaxAction(double[] qValues)
    {
        // Find the action with the highest Q-value
        int maxAction = 0;

        if(qValues.Length == 0)
        {
            return maxAction;
        }

        double maxQValue = qValues[0];

        for (int action = 1; action < qValues.Length; action++)
        {
            if (qValues[action] > maxQValue)
            {
                maxAction = action;
                maxQValue = qValues[action];
            }
        }

        return maxAction;
    }

    public bool MakeMove(int column)
    {
        // Check if the selected column is within the valid range
        if (column < 0 || column > 6)
        {
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

    public double GetReward()
    {
        double reward = 0.0;

        // Check if the game has ended
        if (isGameOver)
        {
            // Check the current player who made the winning move
            int winningPlayer = 3 - currentPlayer;

            // Assign a big reward for the winning player
            if (winningPlayer == 1)
            {
                reward = -100.0; // Big negative reward for AI when Player 1 wins
            }
            else if (winningPlayer == 2)
            {
                reward = 100.0; // Big positive reward for AI when it wins
            }
        }

        // Add intermediate rewards based on the current board state
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                // Ignore cells that are still empty
                if (board[i, j] == 0)
                {
                    continue;
                }

                // Calculate the intermediate reward for each cell
                // This will require additional helper functions.
                reward += CalculateIntermediateReward(i, j);
            }
        }

        return reward;
    }

    private double CalculateIntermediateReward(int row, int col)
    {
        double reward = 0.0;
        int player = board[row, col];

        // Check all lines that pass through the current cell
        int[] dRows = { -1, -1, 0, 1 };
        int[] dCols = { 0, 1, 1, 1 };
        for (int d = 0; d < 4; d++)
        {
            int contiguousPieces = 1;
            for (int i = 1; i < 4; i++)
            {
                int curRow = row + i * dRows[d];
                int curCol = col + i * dCols[d];
                if (curRow < 0 || curRow >= 6 || curCol < 0 || curCol >= 7 || board[curRow, curCol] != player)
                {
                    break;
                }
                contiguousPieces++;
            }
            for (int i = 1; i < 4; i++)
            {
                int curRow = row - i * dRows[d];
                int curCol = col - i * dCols[d];
                if (curRow < 0 || curRow >= 6 || curCol < 0 || curCol >= 7 || board[curRow, curCol] != player)
                {
                    break;
                }
                contiguousPieces++;
            }

            // Calculate the intermediate reward for this line
            if (contiguousPieces >= 2)
            {
                reward += Math.Pow(2, contiguousPieces - 1);
            }
        }

        return reward;
    }

    public int GetCurrentGameState()
    {
        // Convert the current board configuration into a state representation
        int state = 0;
        int stateFactor = 1;

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                state += (int)(board[row, col] * stateFactor);
                stateFactor *= 3; // Assuming there are 3 possible values (0, 1, 2) for each cell
            }
        }

        return state;
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

    public void UpdateQValue(double reward, int currentState, int nextState, int action)
    {
        qLearning.UpdateQValue(reward, currentState, nextState, action);
    }

    public void TrainAndEvaluate()
    {
        double[] learningRates = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
        double[] discountFactors = new double[] { 0.8, 0.85, 0.9, 0.95, 1.0 };

        foreach (double learningRate in learningRates)
        {
            foreach (double discountFactor in discountFactors)
            {
                Console.WriteLine($"Training with learning rate: {learningRate}, discount factor: {discountFactor}");

                qLearning.SetLearningRate(learningRate);
                qLearning.SetDiscountFactor(discountFactor);

                for (int i = 0; i < 1000; i++) // Train over 1000 games
                {
                    if (i % 100 == 0) // Log every 100 games
                    {
                        Console.WriteLine($"Game {i}...");
                    }
                    double reward = PlayGame();
                    Console.WriteLine($"Game {i}, Reward: {reward}");
                }

                string filePath = $"qvalues_lr{learningRate}_df{discountFactor}.txt";
                long[,] doubleArray = new long[1, 2] { { (long)learningRate, (long)discountFactor } };
                qLearning.SaveQValues(doubleArray, filePath);

                int wins = 0;
                for (int i = 0; i < 100; i++) // Evaluate over 100 games
                {
                    if (PlayGame() == 2)
                    {
                        wins++;
                    }
                }

                double winRate = wins / 100.0;
                Console.WriteLine($"Learning rate: {learningRate}, Discount factor: {discountFactor}, Win rate: {winRate}");
            }
        }
    }

}
