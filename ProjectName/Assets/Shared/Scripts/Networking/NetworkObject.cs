using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Generic
{
    /// <summary>
    /// Base class for all asset objects of the game. Extend from this to get a ScriptableObject that can be sent over the internet with an ID.
    /// </summary>
    public abstract class NetworkObject : ScriptableObject
    {
        public string id = System.Guid.NewGuid().ToString();
    }
}
