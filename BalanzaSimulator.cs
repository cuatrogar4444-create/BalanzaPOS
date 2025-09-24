using System;
using System.Timers;

namespace BalanzaPOSNuevo
{
    public class BalanzaSimulator
    {
        private readonly Random random = new Random();
        private readonly Action<double> weightCallback;
        private bool isRunning;

        public BalanzaSimulator(Action<double> callback)
        {
            weightCallback = callback;
        }

        public void Start()
        {
            isRunning = true;
            UpdateWeight();
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void UpdateWeight()
        {
            if (isRunning)
            {
                double weight = random.NextDouble() * 10;
                weightCallback?.Invoke(weight);
            }
        }
    }
}