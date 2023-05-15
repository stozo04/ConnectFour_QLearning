using ConnectFour;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
/**
 * UCT stands for Upper Confidence Bound 1 applied to Trees.
 * It's a strategy used in the selection phase of Monte Carlo Tree Search (MCTS) to decide which child node to explore.
 * 
 * The general formula for UCT is:
 * UCT = average value + C * sqrt(ln(total number of parent node visits) / number of node visits)
 * 
 * In this formula:
 * The average value is the average result of the simulations that have passed through the node.
 * C is an exploration parameter—this is the balance between exploiting nodes with high average value and exploring less visited nodes.
 * The number of node visits and the total number of parent node visits come from the MCTS procedure.
 */
namespace ConnectFour
{
    public class UCT
    {
        private static double explorationParameter = Math.Sqrt(2);

        public static double UCTValue(int totalVisit, double nodeWinScore, int nodeVisit)
        {
            if (nodeVisit == 0)
            {
                return int.MaxValue;
            }
            return (nodeWinScore / (double)nodeVisit)
                   + explorationParameter * Math.Sqrt(Math.Log(totalVisit) / (double)nodeVisit);
        }

        public static int FindBestMove(Node rootNode, int parentVisit)
        {
            int bestIndex = -1;
            double bestValue = double.MinValue;
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                double temp = UCTValue(parentVisit, rootNode.ChildNodes[i].WinScore, rootNode.ChildNodes[i].State.VisitCount);
                if (temp > bestValue)
                {
                    bestIndex = i;
                    bestValue = temp;
                }
            }

            return bestIndex;
        }

        //This method goes through all child nodes of the given node, calculates the UCT value for each, and returns the child node with the highest UCT value.
        public static Node FindBestNodeWithUCT(Node node)
        {
            int parentVisit = node.State.VisitCount;
            double bestValue = double.MinValue;
            Node bestNode = null;

            foreach (Node childNode in node.ChildNodes)
            {
                double uctValue = UCTValue(parentVisit, childNode.WinScore, childNode.State.VisitCount);
                if (uctValue > bestValue)
                {
                    bestValue = uctValue;
                    bestNode = childNode;
                }
            }

            return bestNode;
        }
    }
}
