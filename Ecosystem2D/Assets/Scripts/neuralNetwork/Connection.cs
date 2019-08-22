namespace DefaultNamespace
{
    public class Connection
    {
        public double weight;
        private Neuron usedNeuron;

        public Connection(Neuron neuron, double weight)
        {
            this.weight = weight;
            this.usedNeuron = neuron;
        }

        public double getValue()
        {
            return weight * usedNeuron.getValue();
        }
    }
}