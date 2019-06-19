using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class WorkingNeuron : Neuron
    {
        private float? value = null;
        public List<Connection> connections = new List<Connection>();

        public void addNeuronConnection(Neuron n, float weight)
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
            float value = 0;
            foreach (Connection c in connections)
            {
                value += c.getValue();
            }

            value = Neuron.Sigmoid(value);
            this.value = value;
        }

        public override float getValue()
        {
            if (value == null)
            {
                calculate();
            }

            return (float) value;
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
                c.weight = Random.value;
            }
        }

        public void RandomMutation(float mutationRate)
        {
            Connection c = connections[Random.Range(0, connections.Count)];
            c.weight += Random.value * 2 * mutationRate - mutationRate;
        }
    }
}