namespace ConnectFour
{

    public class MCTS
    {
        private readonly Random random;
        private const int iterations = 4000;
        private const double explorationConstant = 2;

        public MCTS()
        {
            random = new Random();
        }

        public TreeNode Search(Board board)
        {
            TreeNode root = new TreeNode(board, null);
            if (!root.IsTerminal)
            {
                for (var i = 0; i < 50 && !root.IsTerminal; i++)
                {
                    TreeNode selectedNode = Select(root);
                    TreeNode expandedNode = Expand(selectedNode);
                    int score = Rollout(expandedNode.Board);
                    Backpropagate(expandedNode, score);
                }

                return GetBestMove(root);
            }
            else
            {
                Console.WriteLine("Not performing search. Game is already over.");
                return new TreeNode();
            }
        }

        private TreeNode Select(TreeNode node)
        {
            try
            {
                while (!node.IsTerminal)
                {
                    if (node.IsFullyExpanded)
                    {
                        return GetNodeByUCB1(node);
                    }
                    else
                    {
                        return Expand(node);
                    }
                }
                return node;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return new TreeNode();
            }

        }

        private TreeNode Expand(TreeNode node)
        {
            try
            {
                List<Board> possibleStates = node.Board.GenerateStates();
                if (possibleStates.Count == 0)
                {
                    node.IsFullyExpanded = true;
                    return node;
                }

                TreeNode newNode;
                bool alreadyAdded;
                do
                {
                    newNode = new TreeNode(possibleStates[random.Next(possibleStates.Count)], node);
                    alreadyAdded = node.Children.ContainsKey(ComputeMoveKey(node.Board, newNode.Board));
                } while (alreadyAdded);

                node.Children.Add(ComputeMoveKey(node.Board, newNode.Board), newNode);
                node.IsFullyExpanded = node.Children.Count == possibleStates.Count;
                return newNode;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return new TreeNode();
            }

        }

        private string ComputeMoveKey(Board parentBoard, Board childBoard)
        {
            foreach (var position in parentBoard.Position.Keys)
            {
                if (parentBoard.Position[position] != childBoard.Position[position])
                {
                    return $"{position.Item1},{position.Item2}";
                }
            }

            throw new Exception("No move found between parent and child boards");
        }

        private int Rollout(Board board)
        {
            try
            {
                while (!board.IsGameOver())
                {
                    List<Board> availableStates = board.GenerateStates();

                    if (availableStates.Count == 0)
                    {
                        break;
                    }
                    board = availableStates[random.Next(availableStates.Count)];
                }

                return (board.IsWinner(board) && board.CurrentPlayer == "Black") ? -1 : 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return -1;
            }
        }

        private void Backpropagate(TreeNode node, int score)
        {
            try
            {

                while (node != null)
                {
                    node.Visits++;
                    node.Score += score;
                    node = node.ParentNode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");

            }
        }

        private TreeNode GetBestMove(TreeNode node)
        {
            return GetNodeByUCB1(node);
        }

        private TreeNode GetNodeByUCB1(TreeNode node)
        {
            try
            {
                double bestScore = Double.NegativeInfinity;
                List<TreeNode> bestMoves = new List<TreeNode>();

                foreach (KeyValuePair<string, TreeNode> entry in node.Children)
                {
                    TreeNode child = entry.Value;
                    double exploitation = child.Score / child.Visits;
                    double exploration = Math.Sqrt(2 * Math.Log(node.Visits) / child.Visits);
                    double total = exploitation + (explorationConstant * exploration);

                    if (total > bestScore)
                    {
                        bestScore = total;
                        bestMoves.Clear();
                        bestMoves.Add(child);
                    }
                    else if (total == bestScore)
                    {
                        bestMoves.Add(child);
                    }
                }

                int randomMove = random.Next(bestMoves.Count);
                return bestMoves[randomMove];
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return new TreeNode();
            }
        }
    }

    public class TreeNode
    {
        public Board Board { get; set; }
        public bool IsTerminal { get; set; }
        public bool IsFullyExpanded { get; set; }
        public TreeNode ParentNode { get; set; }
        public int Visits { get; set; }
        public int Score { get; set; }
        public Dictionary<string, TreeNode> Children { get; set; }
        public TreeNode(Board board, TreeNode treeNode = null)
        {
            this.Board = board;
            this.IsTerminal = Board.IsGameOver();
            this.IsFullyExpanded = IsTerminal;
            this.ParentNode = treeNode;
            this.Visits = 0;
            this.Children = new Dictionary<string, TreeNode>();
        }

        public TreeNode()
        {
            // This should never occur unless Player 1 wins
        }

        //public TreeNode SelectChild()
        //{
        //    return Children.ArgMax(child => UCB1(child));
        //}

        public double UCB1(TreeNode childNode)
        {
            double C = Math.Sqrt(2);
            if (childNode.Visits == 0)
            {
                return double.MaxValue;
            }// Ensure unvisited nodes are selected first
            return (childNode.Score / (double)childNode.Visits) + (C * Math.Sqrt(Math.Log(Visits) / childNode.Visits));

        }
    }

    public static class ExtensionMethods
    {
        public static TValue ArgMax<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<TValue, double> func)
        {
            return dictionary.Values.Aggregate((bestItem, nextItem) =>
                func(nextItem) > func(bestItem) ? nextItem : bestItem);
        }
    }
}
