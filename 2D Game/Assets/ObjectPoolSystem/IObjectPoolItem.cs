using System;
using Bardent.ObjectPoolSystem;
using UnityEngine;

namespace Bardent.Interfaces
{
    public interface IObjectPoolItem
    {
        void SetObjectPool<T>(ObjectPoolSystem.ObjectPool pool, T comp) where T : Component;

        void Release();
    }
}