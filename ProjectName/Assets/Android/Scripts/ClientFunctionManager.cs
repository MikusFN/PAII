using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Android
{
    public class ClientFunctionManager : MonoBehaviour
    {
        public void Template(){
            DebugApplication.DebugConsole.Instance.Log("Ran template!");            
        }  
    }
}
