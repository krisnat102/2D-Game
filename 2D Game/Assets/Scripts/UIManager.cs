using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public Text ammoText;

    void Update()
    {
        ammoText.text = Weapon.currentMagCapacity.ToString() + " / " + Weapon.maxMagCapacity;
    }
}
