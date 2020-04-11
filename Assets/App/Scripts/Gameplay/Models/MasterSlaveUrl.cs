using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Gameplay.Models
{
    [System.Serializable]
    public class MasterSlaveUrl
    {
        [System.Serializable]
        public class Slave
        {
            public string Url = null;
            public string Name = null;

        }

        [SerializeField] public List<Slave> Urls = null;
    }
}