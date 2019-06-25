namespace DefaultNamespace
{
    public class Connection
    {
        public double weight = 0;
        private Neuron entrieNeuron;

        public Connection(Neuron neuron, double weight)
        {
            this.weight = weight;
            this.entrieNeuron = neuron;
        }

        public double getValue()
        {
            return weight * entrieNeuron.getValue();
        }
    }
}