using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class NeuralNetworkTest : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("BEGIN NN TEST");

            NeuralNetwork nn = new NeuralNetwork();
            InputNeuron in1 = new InputNeuron();
            InputNeuron in2 = new InputNeuron();
            InputNeuron in3 = new InputNeuron();

            WorkingNeuron out1 = new WorkingNeuron();
            WorkingNeuron out2 = new WorkingNeuron();
            WorkingNeuron out3 = new WorkingNeuron();

            nn.addInputNeuron(in1);
            nn.addInputNeuron(in2);
            nn.addInputNeuron(in3);

            nn.generateHiddenNeurons(3);

            nn.addOutputNeuron(out1);
            nn.addOutputNeuron(out2);
            nn.addOutputNeuron(out3);

            nn.generateFullMesh();

            nn.randomizeAllWeights();

            NeuralNetwork nn2 = nn.cloneFullMesh();

            Debug.Log("NN TEST SUCCESS!");
        }
    }
}