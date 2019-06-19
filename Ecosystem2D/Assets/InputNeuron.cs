namespace DefaultNamespace
{
    public class InputNeuron : Neuron
    {
        private float value = 0;

        public void setValue(float x)
        {
            this.value = x;
        }

        public override float getValue()
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