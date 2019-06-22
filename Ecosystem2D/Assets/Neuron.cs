using System;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class Neuron
    {
        private String name = "NO NAME";
        public abstract double getValue();
        public abstract Neuron nameCopy();

        public static double Sigmoid(double x)
        {
            //with x>10, sigmoid will always be almost 1, analog with -10
            //when x gets too large, e.g. around 10000, still double will be NaN or infinity
            if (x > 10)
            {
                x = 10;
            }
            else if (x < -10)
            {
                x = -10;
            }
            else if (double.IsNaN(x))
            {
//                Debug.Log("x is NaN, setting x back to 0");
                x = 0;
            }

            double et = Math.Pow(Math.E, x);
            et = et / (1 + et);
            et = et * 2 - 1;    //probably tanh. nvm


            et = Math.Tanh(x);
            if (double.IsNaN(et))
            {
                Debug.LogError("et = NAN!! \n" +
                               "x is: " + x);
            }

            return et;
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