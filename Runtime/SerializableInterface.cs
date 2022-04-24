using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TNRD
{
    /// <summary>
    /// A wrapper around an interface that supports serialization for both UnityEngine.Object and regular object types
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface you want to serialize</typeparam>
    [Serializable]
    public class SerializableInterface<TInterface>
    {
        [HideInInspector, SerializeField] private ReferenceMode mode = ReferenceMode.Unity;
        [HideInInspector, SerializeField] private UnityEngine.Object unityReference;
        [SerializeReference, UsedImplicitly] private object rawReference;

        public TInterface Value
        {
            get
            {
                return mode switch
                {
                    ReferenceMode.Raw => (TInterface)rawReference,
                    ReferenceMode.Unity => (TInterface)(object)unityReference,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}