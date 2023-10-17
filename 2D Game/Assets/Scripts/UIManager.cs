using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public Text ammoText;
    public Text hpText;
    public Text stamText;
    public Text manaText;

    void Update()
    {

        ammoText.text = Weapon.currentMagCapacity.ToString() + " / " + Weapon.maxMagCapacity;

        hpText.text = PlayerStats.hp.ToString() + " / " + PlayerStats.maxHP.ToString();
        stamText.text = PlayerStats.stam.ToString() + " / " + PlayerStats.maxStam.ToString();
        manaText.text = PlayerStats.mana.ToString() + " / " + PlayerStats.maxMana.ToString();
    }
}
