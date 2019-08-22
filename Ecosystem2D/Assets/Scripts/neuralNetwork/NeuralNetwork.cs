using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class NeuralNetwork
    {
        private List<InputNeuron> inputNeurons = new List<InputNeuron>();
        public List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        public List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();
        private bool fullMeshGenerated = false;
        private bool debugged;


        public void addInputNeuron(InputNeuron neuron)
        {
            inputNeurons.Add(neuron);
        }

        public void addHiddenNeuron(WorkingNeuron neuron)
        {
            hiddenNeurons.Add(neuron);
        }

        public void addOutputNeuron(WorkingNeuron neuron)
        {
            outputNeurons.Add(neuron);
        }

        //Helperfunction:
        public void generateHiddenNeurons(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                hiddenNeurons.Add(new WorkingNeuron());
            }
        }

        public void generateFullMesh()
        {
            foreach (WorkingNeuron hn in hiddenNeurons)
            {
                foreach (InputNeuron input in inputNeurons)
                {
                    hn.addNeuronConnection(input, Random.Range(-1f, 1f));
                }
            }

            foreach (WorkingNeuron on in outputNeurons)
            {
                foreach (WorkingNeuron hn in hiddenNeurons)
                {
                    on.addNeuronConnection(hn, Random.Range(-1f, 1f));
                }
            }

            fullMeshGenerated = true;
        }

        public void invalidate()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                wn.invalidate();
            }

            foreach (WorkingNeuron wn in outputNeurons)
            {
                wn.invalidate();
            }
        }

        public void randomizeAllWeights()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                wn.randomizeWeight();
            }

            foreach (WorkingNeuron wn in outputNeurons)
            {
                wn.randomizeWeight();
            }
        }


        public InputNeuron getInputNeuronFromName(String name)
        {
            foreach (InputNeuron neuron in inputNeurons)
            {
                if (name == neuron.getName())
                {
                    return neuron;
                }
            }

            return null;
        }



        public WorkingNeuron getOutputNeuronFromName(String name)
        {
            foreach (WorkingNeuron neuron in outputNeurons)
            {
                if (name == neuron.getName())
                {
                    return neuron;
                }
            }

            return null;
        }

        public NeuralNetwork cloneFullMesh()
        {
            if (!this.fullMeshGenerated)
            {
                throw new NeuralNetworkNotFullMeshedException();
            }

            NeuralNetwork copy = new NeuralNetwork();
            foreach (InputNeuron input in inputNeurons)
            {
                copy.addInputNeuron((InputNeuron) input.nameCopy());
            }

            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                copy.addHiddenNeuron((WorkingNeuron) wn.nameCopy());
            }

            foreach (WorkingNeuron wn in outputNeurons)
            {
                copy.addOutputNeuron((WorkingNeuron) wn.nameCopy());
            }

            copy.generateFullMesh();

            for (int i = 0; i < hiddenNeurons.Count; i++)
            {
                List<Connection> connectionsOriginal = hiddenNeurons[i].getConnections();
                List<Connection> connectionsCopy = copy.hiddenNeurons[i].getConnections();
                if (connectionsOriginal.Count != connectionsCopy.Count)
                {
                    throw new NotSameAmountOfNeuronsException();
                }

                for (int j = 0; j < connectionsOriginal.Count; j++)
                {
                    connectionsCopy[j].weight = connectionsOriginal[j].weight;
                }
            }

            for (int i = 0; i < outputNeurons.Count - 1; i++)
            {
                List<Connection> connectionsOriginal = outputNeurons[i].getConnections();
                List<Connection> connectionsCopy = copy.outputNeurons[i].getConnections();
                if (connectionsOriginal.Count != connectionsCopy.Count)
                {
                    throw new NotSameAmountOfNeuronsException();
                }

                for (int j = 0; j < connectionsOriginal.Count; j++)
                {
                    connectionsCopy[j].weight = connectionsOriginal[j].weight;
                }
            }

            return copy;
        }


        public void RandomMutation(float mutationRate)
        {
//            int index = Random.Range(0, hiddenNeurons.Count - 1 + outputNeurons.Count - 1);
            int index = Random.Range(0, hiddenNeurons.Count + outputNeurons.Count);
            if (index < hiddenNeurons.Count)
            {
                hiddenNeurons[index].RandomMutation(mutationRate);
            }
            else
            {
                outputNeurons[index - hiddenNeurons.Count].RandomMutation(mutationRate);
            }
        }
    }

    public class NotSameAmountOfNeuronsException : Exception
    {
    }

    public class NeuralNetworkNotFullMeshedException : Exception
    {
    }
}