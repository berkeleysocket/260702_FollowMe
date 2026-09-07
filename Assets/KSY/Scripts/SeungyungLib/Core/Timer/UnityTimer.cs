using UnityEngine;

namespace SeungyungLib.Core.Timer
{
    public class UnityTimer
    {
        private float _startTime;
        private float _time;

        public void Initialize(float time)
        {
            this._time = time;
        }
        
        public void Start()
        {
            _startTime = Time.time;
        }
        
        public bool Check()
        {
            return Time.time - _startTime >= _time;
        }
    }
}

