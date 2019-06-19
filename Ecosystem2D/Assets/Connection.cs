namespace DefaultNamespace
{
    public class Connection
    {
        public float weight = 0;
        private Neuron entrieNeuron;

        public Connection(Neuron neuron, float weight)
        {
            this.weight = weight;
            this.entrieNeuron = neuron;
        }

        public float getValue()
        {
            return weight * entrieNeuron.getValue();
        }
    }
}