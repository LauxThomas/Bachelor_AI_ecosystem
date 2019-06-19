using System;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class Neuron
    {
        private String name = "NO NAME";
        public abstract float getValue();
        public abstract Neuron nameCopy();

        public static float Sigmoid(float x)
        {
            float et = (float) Math.Pow(Math.E, x);
            return et / (1 + et);
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }
    }
}