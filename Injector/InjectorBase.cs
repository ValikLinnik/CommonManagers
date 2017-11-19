using UnityEngine;
using Injection;

namespace Injection
{
    public class InjectorBase<T> : MonoBehaviour where T : Component
    {
        protected virtual void Awake()
        {
            Context.Instance.Inject(this as T);
        }
    }
}
