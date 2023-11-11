using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    [SerializeField] private Text ammoText;

    void Update()
    {
        ammoText.text = weapon.GetCurrentCapacity().ToString() +  "/"  + weapon.GetMaxCapacity().ToString();
    }
}
