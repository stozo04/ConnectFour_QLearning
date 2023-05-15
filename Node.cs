namespace ConnectFour
{
    public class Node
    {
        public State State { get; set; }
        public Node ParentNode { get; set; }
        public List<Node> ChildNodes { get; set; }
        public int VisitCount { get; set; }
        public double WinScore { get; set; }

        public Node(State state)
        {
            this.State = state;
            ChildNodes = new List<Node>();
        }

        private void ExpandNode(Node node)
        {
            List<State> possibleStates = GetPossibleStates(node);
            possibleStates.ForEach(state =>
            {
                Node newNode = new Node(state) { ParentNode = node };
                newNode.State.PlayerNo = node.State.PlayerNo == 1 ? 2 : 1;
                node.ChildNodes.Add(newNode);
            });
        }

        private List<State> GetPossibleStates(Node node)
        {
            List<State> possibleStates = new List<State>();
            int[,] board = node.State.Board;
            int playerNo = node.State.PlayerNo;

            // Iterate over each column.
            for (int i = 0; i < board.GetLength(0); i++)
            {
                // Find the first empty slot in this column.
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == 0)
                    {
                        // Copy the current board state.
                        int[,] newBoard = (int[,])board.Clone();

                        // Add a piece in the first empty slot of this column.
                        newBoard[i, j] = playerNo;

                        // Create a new state with the new board and the opponent's player number.
                        State newState = new State
                        {
                            Board = newBoard,
                            PlayerNo = playerNo == 1 ? 2 : 1
                        };

                        // Add this state to the list of possible states.
                        possibleStates.Add(newState);

                        // We've added a piece to this column, so we can break out of the inner loop.
                        break;
                    }
                }
            }

            return possibleStates;
        }
        public Node AddChild(Node childNode)
        {
            ChildNodes.Add(childNode);
            childNode.ParentNode = this;
            return childNode;
        }

        public void UpdateNode(double winScore)
        {
            this.VisitCount++;
            this.WinScore += winScore;
        }
    }

    public class Tree
    {
        public Node Root { get; set; }

        public Tree(Node root)
        {
            this.Root = root;
        }
    }

    public class State
    {
        public int[,] Board { get; set; }
        public int PlayerNo { get; set; }
        public int VisitCount { get; set; }
        public double WinScore { get; set; }

        public State(int[,] board, int playerNo)
        {
            this.Board = (int[,])board.Clone();
            this.PlayerNo = playerNo;
        }

        public State GetNextState(int[] position, int playerNo)
        {
            State nextState = new State(this.Board, playerNo);
            nextState.Board[position[0], position[1]] = playerNo;
            return nextState;
        }
    }
}
