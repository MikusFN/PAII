using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Generic
{
    [Serializable]
    public class CardEvent : UnityEvent<int, Card>
    {

    }

    [Serializable]
    public class NpcEvent : UnityEvent<GameObject>{

    }
}
