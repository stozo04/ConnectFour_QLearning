using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    public class QLearning
    {
        private long[,] qTable;
        private double learningRate; // DEFAULT: 0.3
        private double discountFactor; // DEFAULT: 0.95
        private const string filePath = "values.txt";

        public QLearning()
        {
            this.qTable = LoadQValues();
        } 

        public void SaveQValues(long[,] qValues, string filePathToSave)
        {
            int numRows = qValues.GetLength(0);
            int numColumns = qValues.GetLength(1);
            string[] lines = new string[numRows];

            for (int i = 0; i < numRows; i++)
            {
                double[] rowValues = new double[numColumns];
                for (int j = 0; j < numColumns; j++)
                {
                    rowValues[j] = qValues[i, j];
                }
                lines[i] = string.Join(",", rowValues);
            }

            File.WriteAllLines(filePathToSave, lines);
        }

        public void UpdateQValue(double reward, int currentState, long nextState, int action)
        {
            // Retrieve the Q-value for the current state-action pair
            double currentQValue = qTable[currentState, action];

            // Retrieve the Q-values for the next state
            double[] qValuesNextState = GetQValues(nextState);
            // Retrieve the maximum Q-value for the next state
            double maxNextQValue = qValuesNextState.Max();

            // Update the Q-value for the current state-action pair using the Bellman equation
            double val = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);
            long newQValue = (long)val;
            qTable[currentState, action] = newQValue;

            // Save the updated Q-table to a file
           // TODO: SaveQValues(qTable);
        }

        private long[,] LoadQValues()
        {
            int numStates = 7; /* number of states */
            long numActions = Int32.MaxValue; /* number of actions */

            if (File.Exists(filePath))
            {
       
                string[] lines = File.ReadAllLines(filePath);
                int numRows = lines.Length;
                int numColumns = lines[0].Split(',').Length;
                long[,] qValues = new long[numRows, numColumns];

                for (int i = 0; i < numRows; i++)
                {
                    double[] doubleArray = lines[i].Split(',').Select(double.Parse).ToArray();

                    long[] rowValues = new long[doubleArray.Length];
                    for (int d = 0; d < doubleArray.Length; d++)
                    {
                        rowValues[d] = (long)doubleArray[d];
                    }

                    for (int j = 0; j < numColumns; j++)
                    {
                        qValues[i, j] = rowValues[j];
                    }
                }

                return qValues;
               
            }
            else
            {
                // If the file doesn't exist, initialize an empty Q-table
                return new long[numStates, numActions];
            }
        }

        public double[] GetQValues(long state)
        {
            // Check if the state is a valid index for the Q-table
            if (state >= 0 && state < qTable.GetLength(0))
            {
                // Get the row corresponding to the state
                double[] qValues = new double[qTable.GetLength(1)];
                for (int i = 0; i < qTable.GetLength(1); i++)
                {
                    qValues[i] = qTable[state, i];
                }
                return qValues;
            }
            else
            {
                // State not found, return default Q-values
                return GetInitialQValues(7);
            }
        }

        public double[] GetInitialQValues(int numberOfActions)
        {
            return new double[numberOfActions];
        }

        internal void SetLearningRate(double qlearningRate)
        {
            learningRate = qlearningRate;
        }

        internal void SetDiscountFactor(double qdiscountFactor)
        {
            discountFactor = qdiscountFactor;
        }
    }
}
