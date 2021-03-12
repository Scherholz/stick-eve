using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

public class RealTimeCombatGameManager : RealTimeCombatNetworkManagerBehavior
{
    public List<GameObject> players;

    private static RealTimeCombatGameManager _instance;

    public static RealTimeCombatGameManager Instance { get { return _instance; } }
    // Start is called before the first frame update
    void Start()
    {
        if (!networkObject.IsServer)
        {
            Debug.Log("Start RealTimeCombatGameManager");
            NetworkManager.Instance.InstantiatePlayerMovement();
            //networkObject.AssignOwnership(NetworkManager.Instance.);
        }
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
