using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnLocations;

    public static PlayerSpawnManager instance = null;

    public List<PlayerInput> playerList = new List<PlayerInput>();

    public event System.Action <PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    public bool canRespawn;

    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;


    void OnPlayerJoined(PlayerInput playerInput)
    {

        if (canRespawn)
            {
            Debug.Log("PlayerInput ID:" + playerInput.playerIndex + "Joined the Game!");

            playerInput.gameObject.GetComponent<PlayerDetails>().playerID = playerInput.playerIndex + 1;
            //gets the player ID from player details and adds +1 to it

            playerInput.gameObject.GetComponent<PlayerDetails>().startPos = spawnLocations[playerInput.playerIndex].position;
            //gets the starting position from playerdetails and sets it to the spawnlocation positions

            playerInput.gameObject.GetComponent<PlayerDetails>().startRot = spawnLocations[playerInput.playerIndex].rotation;
            //same as above but with rotation

            playerList.Add(playerInput); //adds the playerInput to the playerlist when joining

            if (PlayerJoinedGame != null)
            {
                PlayerJoinedGame(playerInput);
            }
        }
    }
    void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput ID:" + playerInput.playerIndex + "Left the Game");

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);

    }

    private void Start() //currently joining player 1 in start function
    {
        // PlayerInputManager.instance.JoinPlayer(0, -1, null); //looks for player index, splitscreen index, controlscheme index, and input device
        canRespawn = true;
    }

    private void Update()
    {
        foreach (var player in playerList)
        {
            if(player == null)
            {
                UnregisterPlayer(player);
            }
        }
    }

    void JoinAction(InputAction.CallbackContext context)
    {
        if (canRespawn)
        {
            PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
            CameraController.instance.SetCamera();
        }
    }
    void LeaveAction(InputAction.CallbackContext context)
    {
        if(playerList.Count > 1) //if there are more than 1 players
        {
            foreach( var player in playerList) 
            {
                foreach (var device in player.devices) 
                {
                    if (device != null && context.control.device == device)
                    {
                        UnregisterPlayer(player);
                        CameraController.instance.SetCamera();
                        return;
                    }
                }
            }
        }
    }

    void UnregisterPlayer(PlayerInput playerInput )
    {
        playerList.Remove(playerInput);
        
        if (PlayerLeftGame != null)
        {
            PlayerLeftGame(playerInput);
        }

        if (playerInput != null)
        {
            Destroy(playerInput.gameObject);
        }

    }
}
