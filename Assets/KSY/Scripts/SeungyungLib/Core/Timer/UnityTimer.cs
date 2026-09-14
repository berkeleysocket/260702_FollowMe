using UnityEngine;

namespace SeungyungLib.Core.Timer
{
    public class UnityTimer
    {
        public float CurrentTime => Time.time - _startTime;
        
        private float _startTime;
        private float _checkTime;

        public void Initialize(float time)
        {
            this._checkTime = time;
        }
        
        public void Start()
        {
            _startTime = Time.time;
        }
        
        public bool Check()
        {
            return Time.time - _startTime >= _checkTime;
        }
    }
}

