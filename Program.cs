using System;

Game game = new Game();
game.TrainAndEvaluate();

//int counter = 0;
//while (true)
//{
//    Game game = new Game();
//    int currentState = game.GetCurrentGameState();

//    try
//    {
//        while (true)
//        {
//            int action;

//            // Player 1's turn (Human)
//            if (game.CurrentPlayer == 1)
//            {
//                // Uncomment the following code if you want to have a human player
//                /* 
//                game.PrintBoard();
//                Console.WriteLine("Enter a column number (0-6):");
//                int column = Convert.ToInt32(Console.ReadLine());
//                if (!game.MakeMove(column))
//                {
//                    Console.WriteLine("Invalid move, try again.");
//                    continue;
//                }
//                */

//                // AI playing as Player 1
//                action = game.ChooseAction(currentState);
//                if (!game.MakeMove(action))
//                {
//                    Console.WriteLine("Invalid move. AI made an invalid move.");
//                    continue;
//                }
//            }
//            // Player 2's turn (AI)
//            else
//            {
//                action = game.ChooseAction(currentState);
//                if (!game.MakeMove(action))
//                {
//                    Console.WriteLine("Invalid move. AI made an invalid move.");
//                    continue;
//                }
//            }

//            int nextState = game.GetCurrentGameState();
//            double reward = game.GetReward();

//            // Update the Q-value for the previous state-action pair using the Bellman equation
//            game.UpdateQValue(reward, currentState, nextState, action);

//            currentState = nextState;

//            if (game.CheckWinner())
//            {
//                Console.WriteLine("Player " + game.CurrentPlayer + " wins!");
//                break;
//            }

//            // Switch to the next player
//            game.SwitchPlayer();
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("An error occurred: " + ex.Message);
//        Console.WriteLine(ex.StackTrace);
//    }

//    counter++;
//    Console.WriteLine("Game finished! Total games played: " + counter);
//    if (counter == 100)
//    {
//        Console.WriteLine("Stopping after 100 games. Press any key to exit...");
//        Console.ReadLine();
//        break;
//    }
//}
