using UnityEngine;

namespace Interfaces
{
    public interface IHookStrategy
    {
        public bool CanRun { get; set; }
        public Transform Hook { get; set; }
        public Transform BottomPivot { get; set; }
        public Transform TopPivot { get; set; }
       
        public void ResetHook();
    }
}