namespace DefaultNamespace
{
    public class InputNeuron : Neuron
    {
        private double value = 0;

        public void setValue(double x)
        {
            this.value = x;
        }

        public override double getValue()
        {
            return this.value;
        }

        public override Neuron nameCopy()
        {
            InputNeuron clone = new InputNeuron();
            clone.setName(getName());
            return clone;
        }
    }
}