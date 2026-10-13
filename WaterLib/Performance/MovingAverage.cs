namespace Water
{
    public sealed class MovingAverage
    {
        private readonly double _weight;
        private double _value;
        private bool _ready;

        public MovingAverage(double weight)
        {
            if (weight < 0.01)
                weight = 0.01;
            if (weight > 0.99)
                weight = 0.99;
            _weight = weight;
        }

        public double Value
        {
            get { return _value; }
        }

        public double Push(double sample)
        {
            if (!_ready)
            {
                _value = sample;
                _ready = true;
                return _value;
            }

            _value = (_value * (1d - _weight)) + (sample * _weight);
            return _value;
        }

        public void Reset()
        {
            _value = 0;
            _ready = false;
        }
    }
}
