using System;
using System.Collections.Generic;
using System.Linq;
using ConnectFour;

namespace ConnectFour
{

    public class MonteCarloTreeSearch
    {
        private static readonly int WIN_SCORE = 10;
        private int level;
        private int opponent;

        public MonteCarloTreeSearch()
        {
            this.level = 0;
        }

        //FindNextMove: This method should use MCTS to find the best next move given the current game state.
        public int FindNextMove(Tree tree, int playerNo)
        {
            opponent = 3 - playerNo;
            Node rootNode = tree.Root;

            int parentVisit = rootNode.State.VisitCount;
            return UCT.FindBestMove(rootNode, parentVisit);
        }

        //SelectPromisingNode: This method should select the most promising node from the current game state.
        private Node SelectPromisingNode(Node rootNode)
        {
            Node node = rootNode;
            while (node.ChildNodes.Count != 0)
            {
                node = UCT.FindBestNodeWithUCT(node);
            }

            return node;
        }

        //ExpandNode: This method should expand a node by creating child nodes for all possible moves.
        private void ExpandNode(Node node)
        {
            List<State> possibleStates = GetPossibleStates(node);
            possibleStates.ForEach(state =>
            {
                Node newNode = new Node(state);
                newNode.ParentNode = node;
                newNode.State.PlayerNo = node.State.PlayerNo == 1 ? 2 : 1;
                node.ChildNodes.Add(newNode);
            });
        }

        //BackPropagate: This method should backpropagate from the end of a simulation up to the root node, updating the statistics along the path.
        private void BackPropagate(Node nodeToExplore, int playerNo)
        {
            Node tempNode = nodeToExplore;
            while (tempNode != null)
            {
                tempNode.State.VisitCount++;
                if (tempNode.State.PlayerNo == playerNo)
                {
                    tempNode.State.WinScore += WIN_SCORE;
                }
                tempNode = tempNode.ParentNode;
            }
        }

        private List<State> GetPossibleStates(Node node)
        {
            int[,] boardCopy = (int[,])node.State.Board.Clone();
            List<State> possibleStates = new List<State>();
            for (int col = 0; col < 7; col++)
            {
                for (int row = 5; row >= 0; row--)
                {
                    if (boardCopy[row, col] == 0)
                    {
                        int[,] newBoard = (int[,])boardCopy.Clone();
                        newBoard[row, col] = node.State.PlayerNo;
                        possibleStates.Add(new State(newBoard, 3 - node.State.PlayerNo));
                        break;
                    }
                }
            }
            return possibleStates;
        }


        //SimulateRandomPlayout: This method should simulate a random playout from a node and then return the winning player.
        private void SimulateRandomPlayout(Node node)
        {
            Node tempNode = new Node()
            {
                State = new State
                {
                    Board = (int[,])node.State.Board.Clone(),
                    PlayerNo = node.State.PlayerNo
                }
            };

            State tempState = tempNode.State;
            int boardStatus = CheckBoardStatus(tempState);

            if (boardStatus == opponent)
            {
                tempNode.ParentNode.State.WinScore = -1 * WIN_SCORE;
                return;
            }

            while (boardStatus == 0)
            {
                tempState.PlayerNo = 3 - tempState.PlayerNo;
                Random rnd = new Random();
                List<State> possiblePositions = GetPossibleStates(tempNode);
                tempState = possiblePositions[rnd.Next(possiblePositions.Count)];
                boardStatus = CheckBoardStatus(tempState);
            }

            if (boardStatus == tempState.PlayerNo)
            {
                node.ParentNode.State.WinScore += WIN_SCORE;
            }
            else
            {
                node.ParentNode.State.WinScore -= WIN_SCORE;
            }
        }

        /**
         * Please note that in this function:
         * It first checks for vertical winning combinations (line 7 to 13).
         * Then it checks for horizontal winning combinations (line 16 to 22).
         * After that, it checks for diagonal winning combinations from bottom left to top right (line 25 to 31).
         * Then it checks for diagonal winning combinations from top left to bottom right (line 34 to 40).
         * If no winner is found, it checks if there's any empty space in the board. If yes, it returns 0 meaning the game is ongoing (line 43 to 48).
         * If no winner and no empty space is found, it returns -1
         */
        private int CheckBoardStatus(State state)
        {
            int[,] board = state.Board;
            int playerNo = state.PlayerNo;

            // Check vertical lines
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col] == playerNo && board[row + 1, col] == playerNo && board[row + 2, col] == playerNo && board[row + 3, col] == playerNo)
                    {
                        return playerNo;
                    }
                }
            }

            // Check horizontal lines
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] == playerNo && board[row, col + 1] == playerNo && board[row, col + 2] == playerNo && board[row, col + 3] == playerNo)
                    {
                        return playerNo;
                    }
                }
            }

            // Check for diagonal (bottom left to top right)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] == playerNo && board[row + 1, col + 1] == playerNo && board[row + 2, col + 2] == playerNo && board[row + 3, col + 3] == playerNo)
                    {
                        return playerNo;
                    }
                }
            }

            // Check for diagonals (top left to bottom right)
            for (int row = 3; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] == playerNo && board[row - 1, col + 1] == playerNo && board[row - 2, col + 2] == playerNo && board[row - 3, col + 3] == playerNo)
                    {
                        return playerNo;
                    }
                }
            }

            // Check for draw
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    // If there's an empty space, return 0 for ongoing game
                    if (board[row, col] == 0)
                    {
                        return 0;
                    }
                }
            }

            // If no empty spaces and no winner, it's a draw
            return -1;
        }
    }
}


