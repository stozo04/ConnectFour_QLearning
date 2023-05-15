using ConnectFour;
using System;

int currentPlayer = 1; // Start with player 1
Game game = new Game();
MonteCarloTreeSearch0o mcts = new MonteCarloTreeSearch();

while (!game.IsGameOver())
{
    Console.Clear();
    game.PrintBoard();

    if (currentPlayer == 1)
    {
        // Human player's turn
        Console.Write("Your move (enter column number): ");
        int move = Convert.ToInt32(Console.ReadLine());

        // Check if the move is valid
        while (!game.IsValidMove(move))
        {
            Console.Write("Invalid move. Try again: ");
            move = Convert.ToInt32(Console.ReadLine());
        }

        game.MakeMove(move, currentPlayer);
    }
    else
    {
        // AI's turn
        Console.WriteLine("AI is thinking...");
        Tree tree = new Tree();
        tree.Root = new Node { State = new State { Board = game.GetBoardCopy(), PlayerNo = currentPlayer } };
        int aiMove = mcts.FindNextMove(tree, currentPlayer);
        game.MakeMove(aiMove, currentPlayer);
    }

    // Switch player
    currentPlayer = 3 - currentPlayer;
}

game.PrintBoard();

if (game.GetWinner() == 0)
{
    Console.WriteLine("It's a draw!");
}
else
{
    Console.WriteLine($"Player {game.GetWinner()} has won the game!");
}

