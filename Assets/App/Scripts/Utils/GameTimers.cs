using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utils
{
    public static class GameTimers
    {
        public const string TIMERS_PARENT = "[Timers]";
        public const string TIMER = "[Timer] ";

        private static GameObject _parentTimers;
        public static GameObject parentTimers
        {
            get
            {
                if (_parentTimers == null)
                {
                    _parentTimers = new GameObject(TIMERS_PARENT);
                    GameObject.DontDestroyOnLoad(_parentTimers);
                }
                return _parentTimers;
            }
        }

        private static Dictionary<string, Timer> activeTimers = new Dictionary<string, Timer>();

        #region TIMERS MANAGE

        public static bool isTimer(string id)
        {
            return activeTimers.ContainsKey(id);
        }

        public static Timer GetTimer(string id)
        {
            if (activeTimers.ContainsKey(id)) return activeTimers[id];

            Debug.LogError(TIMERS_PARENT + " No Timer with id " + id);
            return null;
        }

        public static Timer AddNewTimer(string id, long startTime, long endTime, Action<Timer> OnComplete = null, Action<long> OnUpdate = null)
        {
            Debug.Log(TIMERS_PARENT + " Add New Timer id = " + id + ", time = " + (endTime - startTime) + ", startTime" + startTime);
            if (activeTimers.ContainsKey(id))
            {
                Debug.Log(TIMERS_PARENT + " already contain Timer with id " + id + "!");
            }
            else
            {
                GameObject timer_go = new GameObject(TIMER + id);
                timer_go.transform.SetParent(parentTimers.transform, false);
                Timer timer = timer_go.AddComponent<Timer>();
                timer.Init(id);
                activeTimers.Add(id, timer);

            }
            activeTimers[id].RestartTimer(startTime, endTime);
            activeTimers[id].OnComplete += OnComplete;
            activeTimers[id].OnUpdate += OnUpdate;

            return activeTimers[id];
        }
        public static long AddTimeTo(long currentSecond, TimeSpan addedTime)
        {
            return currentSecond + (long)addedTime.TotalSeconds;
        }

        public static bool RemoveTimer(string id)
        {
            if (activeTimers.ContainsKey(id))
            {
                if (!activeTimers[id].IsComplete)
                {
                    activeTimers[id].Stop();
                }
                activeTimers.Remove(id);
                return true;
            }
            return false;
        }

        #endregion

        #region TIME AND DATE FORMATS

        public static long GetNowTimestampTicks()
        {
            return (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        }

        public static long GetNowTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static long GetNowTimestampMiliSeconds()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static string SecondsToHHMMSS(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.Hours > 0 ?
                string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) :
                string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
        #endregion
    }
}
