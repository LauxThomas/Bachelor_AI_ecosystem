using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class WorkingNeuron : Neuron
    {
        private double? value = null;
        public List<Connection> connections = new List<Connection>();

        public void addNeuronConnection(Neuron n, double weight)
        {
            addNeuronConnection(new Connection(n, weight));
        }

        private void addNeuronConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public void invalidate()
        {
            this.value = null;
        }

        private void calculate()
        {
            double value = 0;
            foreach (Connection c in connections)
            {
                value += c.getValue();
            }

            value = activationFunction(value);
//            value = Sigmoid(value/connections.Count);    //average

            this.value = value;
        }

        public override double getValue()
        {
            if (value == null)
            {
                calculate();
            }

            return  (double)value;
        }

        public override Neuron nameCopy()
        {
            WorkingNeuron clone = new WorkingNeuron();
            clone.setName(getName());
            return clone;
        }

        public List<Connection> getConnections()
        {
            return connections;
        }

        public void randomizeWeight()
        {
            foreach (Connection c in connections)
            {
                c.weight = Random.Range(-1f,1f);
//                c.weight = 0;
            }
        }

        public void RandomMutation(float mutationRate)
        {
            Connection c = connections[Random.Range(0, connections.Count)];
            c.weight += Random.value * 2 * mutationRate - mutationRate;
        }
    }
}