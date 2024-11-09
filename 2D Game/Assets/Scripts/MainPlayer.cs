using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreClass;

public class MainPlayer : MonoBehaviour
{
    protected PlayerInputHandler inputHandler;
    protected MainPlayer player;
    protected DataPlayer dataPlayer;

    public MainPlayer(PlayerInputHandler inputHandler, MainPlayer player, DataPlayer dataPlayer)
    {
        this.inputHandler = inputHandler;
        this.player = player;
        this.dataPlayer = dataPlayer;
    }

    //public InputManager InputManager { get; private set; }
    //public DataPlayer DataPlayer { get => dataPlayer; private set => dataPlayer = value; }

    //[SerializeField] private DataPlayer dataPlayer;


    private void Awake()
    {
        //InputManager = GetComponent<InputManager>();
    }
}
