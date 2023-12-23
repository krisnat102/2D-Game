using UnityEngine;

namespace Bardent.CoreSystem
{
    public class CoreComponent : MonoBehaviour, ILogicUpdate
    {
        public Core Core { get; private set; }

        protected virtual void Awake()
        {
            Core = transform.parent.GetComponent<Core>();

            if(Core == null) { Debug.LogError("There is no Core on the parent"); }
            Core.AddComponent(this);
        }

        public virtual void LogicUpdate() { }

    }
}
