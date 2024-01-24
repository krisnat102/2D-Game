using System;
using Bardent.ObjectPoolSystem;
using UnityEngine;

namespace Bardent.Interfaces
{
    public interface IObjectPoolItem
    {
        public void SetObjectPool<T>(Bardent.ObjectPoolSystem.ObjectPool pool, T comp) where T : Component;

        public void Release();
    }
}