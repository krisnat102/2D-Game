using System;
using System.Collections.Generic;
using System.Linq;
using Bardent.Weapons.Components;
using Inventory;
using UnityEngine;

namespace Bardent.Weapons
{
    public class WeaponGenerator : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private int weaponCount;
        private WeaponDataSO data;

        private List<WeaponComponent> componentAlreadyOnWeapon = new List<WeaponComponent>();

        private List<WeaponComponent> componentsAddedToWeapon = new List<WeaponComponent>();

        private List<Type> componentDependencies = new List<Type>();

        private Animator anim;

        public WeaponDataSO Data { get => data; private set => data = value; }

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }
        private void Update()
        {
            if(weaponCount == 1)
            {
                Data = InventoryManager.Instance.Weapon1Bn.GetComponent<ItemController>().GetItem()?.weaponData;
            }
            if (weaponCount == 2)
            {
                Data = InventoryManager.Instance.Weapon2Bn.GetComponent<ItemController>().GetItem()?.weaponData;
            }
            if(Data != null)
            {
                GenerateWeapon(Data);
            }
        }

        [ContextMenu("Test Generate")]
        private void TestGeneration()
        {
            GenerateWeapon(Data);
        }

        public void GenerateWeapon(WeaponDataSO data)
        {
            weapon.SetData(data);


            componentAlreadyOnWeapon.Clear();
            componentsAddedToWeapon.Clear();
            componentDependencies.Clear();

            componentAlreadyOnWeapon = GetComponents<WeaponComponent>().ToList();

            componentDependencies = data.GetAllDependencies();

            foreach (var dependency in componentDependencies)
            {
                if (componentsAddedToWeapon.FirstOrDefault(component => component.GetType() == dependency))
                    continue;

                var weaponComponent =
                    componentAlreadyOnWeapon.FirstOrDefault(component => component.GetType() == dependency);

                if (weaponComponent == null)
                {
                    weaponComponent = gameObject.AddComponent(dependency) as WeaponComponent;
                }


                weaponComponent.Init();


                componentsAddedToWeapon.Add(weaponComponent);
            }

            var componentsToRemove = componentAlreadyOnWeapon.Except(componentsAddedToWeapon);


            foreach (var weaponComponent in componentsToRemove)
            {
                Destroy(weaponComponent);
            }

            anim.runtimeAnimatorController = data.AnimatorController;
        }
    }
}