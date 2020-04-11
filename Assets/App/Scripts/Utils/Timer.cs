using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utils
{
    public class Timer : MonoBehaviour
    {
        public event Action<long> OnUpdate;
        public event Action<Timer> OnComplete;

        public string Id => id;
        public long TimeLeft => timeLeft;
        public bool IsComplete => isComplete;

        private string id;
        private long startTime;
        private long endTime;
        private long timeLeft;
        private bool isComplete = false;
        private long _preTime;
        private Coroutine myself = null;

        public void Init(string id)
        {
            this.id = id;
        }

        public void RestartTimer(long startTime, long endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            timeLeft = endTime - startTime;
            isComplete = false;
            if (myself != null)
            {
                StopCoroutine(myself);
            }
            myself = StartCoroutine(UpdateCoroutine());
        }

        private long GetTimeLeft()
        {
            var time = endTime - GameTimers.GetNowTimestampSeconds();
            return time < 0 ? 0 : time;
        }

        private IEnumerator UpdateCoroutine()
        {
            while (timeLeft > 0 && !isComplete)
            {
                yield return null;

                _preTime = timeLeft;
                timeLeft = GetTimeLeft();

                if (_preTime != timeLeft)
                {
                    OnUpdate?.Invoke(timeLeft);
                }
            }

            if (isComplete)
            {
                yield break;
            }

            Debug.Log(GameTimers.TIMER + id + " Completed!");
            isComplete = true;
            GameTimers.RemoveTimer(id);

            OnComplete?.Invoke(this);
        }

        public void Stop()
        {
            Debug.Log(GameTimers.TIMER + id + " Stopped!");
            StopCoroutine(myself);
            OnUpdate?.Invoke(0);
            isComplete = true;
            GameTimers.RemoveTimer(id);
        }
    }
}
