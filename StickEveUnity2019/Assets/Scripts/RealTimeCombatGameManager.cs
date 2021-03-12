using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.SceneManagement;

public class RealTimeCombatGameManager : RealTimeCombatNetworkManagerBehavior
{
    public List<GameObject> players;

    private static RealTimeCombatGameManager _instance;
    private PlayerMovementNetworkObject m_player;
    public static RealTimeCombatGameManager Instance { get { return _instance; } }
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.objectInitialized += ObjectInitialized;

        if (networkObject!=null)
        {
            if (networkObject.IsServer)
            {
                return;
            }
        }

        Debug.Log("Start RealTimeCombatGameManager");
        NetworkManager.Instance.InstantiatePlayerMovement();

        
        NetworkManager.Instance.Networker.disconnected += DisconnectedFromServer;

    }
    private void ObjectInitialized(INetworkBehavior behavior, NetworkObject obj)
    {
        if (!(obj is PlayerMovementNetworkObject))
            return;

        // When this object is destroyed we need to decrement the player count as the camera
        // represents the player
        /*obj.onDestroy += (sender) =>
        {
            playerCount--;
        };*/

        if (NetworkManager.Instance.Networker is IServer)
            obj.Owner.disconnected += (sender) => { obj.Destroy(); };

        m_player = obj as PlayerMovementNetworkObject;
    }

    private void DisconnectedFromServer(NetWorker sender)
    {
        MainThreadManager.Run(() =>
        {
            NetworkManager.Instance.Disconnect();
            SceneManager.LoadScene(0);
        });
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        NetworkManager.Instance.objectInitialized -= ObjectInitialized;

        if (networkObject != null)
            networkObject.Destroy();

        if (m_player != null)
            m_player.Destroy();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
