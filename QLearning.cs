using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    public class QLearning
    {
        private double[,] qTable;
        private double learningRate; // DEFAULT: 0.3
        private double discountFactor; // DEFAULT: 0.95
        private const string filePath = "values.txt";

        public QLearning()
        {
            this.qTable = LoadQValues();
        } 

        public void SaveQValues(int[,] qValues, string filePathToSave)
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

        public void UpdateQValue(double reward, int currentState, int nextState, int action)
        {
            // Retrieve the Q-value for the current state-action pair
            double currentQValue = qTable[currentState, action];

            // Retrieve the Q-values for the next state
            double[] qValuesNextState = GetQValues(nextState);
            // Retrieve the maximum Q-value for the next state
            double maxNextQValue = qValuesNextState.Max();

            // Update the Q-value for the current state-action pair using the Bellman equation
            double newQValue = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);

            qTable[currentState, action] = newQValue;

            // Save the updated Q-table to a file
           // TODO: SaveQValues(qTable);
        }

        private double[,] LoadQValues()
        {
            int numStates = 7; /* number of states */
            int numActions = 100; /* number of abstracted actions */

            if (File.Exists(filePath))
            {
       
                string[] lines = File.ReadAllLines(filePath);
                int numRows = lines.Length;
                int numColumns = lines[0].Split(',').Length;
                double[,] qValues = new double[numRows, numColumns];

                for (int i = 0; i < numRows; i++)
                {
                    double[] doubleArray = lines[i].Split(',').Select(double.Parse).ToArray();

                    int[] rowValues = new int[doubleArray.Length];
                    for (int d = 0; d < doubleArray.Length; d++)
                    {
                        rowValues[d] = (int)doubleArray[d];
                    }

                    for (int j = 0; j < numColumns; j++)
                    {
                        qValues[i, j] = rowValues[j];
                    }
                }

                // Perform state abstraction to reduce the number of actions
                double[,] abstractedQValues = PerformStateAbstraction(qValues, numActions);

                return abstractedQValues;

            }
            else
            {
                // If the file doesn't exist, initialize an empty Q-table
                return new double[numStates, numActions];
            }
        }

        private double[,] PerformStateAbstraction(double[,] qValues, int numActions)
        {
            int numRows = qValues.GetLength(0);
            int numColumns = qValues.GetLength(1);
            double[,] abstractedQValues = new double[numRows, numActions];

            // Determine the range of Q-values for each abstracted action
            double qValueRange = 1.0 / numActions;

            // Iterate over each row of qValues
            for (int i = 0; i < numRows; i++)
            {
                // Iterate over each abstracted action
                for (int j = 0; j < numActions; j++)
                {
                    // Calculate the range of Q-values for the current abstracted action
                    double qValueMin = j * qValueRange;
                    double qValueMax = (j + 1) * qValueRange;

                    // Find the maximum Q-value within the range for the current row
                    double maxQValueInRange = double.MinValue;

                    // Iterate over each column of qValues for the current row
                    for (int k = 0; k < numColumns; k++)
                    {
                        double qValue = qValues[i, k];

                        // Check if the Q-value falls within the range for the current abstracted action
                        if (qValue >= qValueMin && qValue < qValueMax)
                        {
                            // Update the maximum Q-value within the range
                            maxQValueInRange = Math.Max(maxQValueInRange, qValue);
                        }
                    }

                    // Set the maximum Q-value within the range for the abstractedQValues
                    abstractedQValues[i, j] = maxQValueInRange;
                }
            }

            return abstractedQValues;
        }

        public double[] GetQValues(int state)
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
