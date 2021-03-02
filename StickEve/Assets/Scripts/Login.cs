using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using TMPro;

public class Login : MonoBehaviour
{
    public class MessageObj{
       public string action;
       public string user;
      public MessageObj(string actionIn, string userIn)
      {
         action = actionIn;
         user = userIn;
      }
    }
    private MessageObj _testCommand = new MessageObj("ServerStatus","Lulu");
    private WebSocket _testWs;
    private string _lastReceivedMessage = "";
    private TMP_Text m_textMeshPro;
    private WebSocket _websocket;
    // Establishes the connection's lifecycle callbacks.
   // Once the connection is established, OnOpen, it automatically attempts to create or join a game through the RequestStartOp code.
   private void SetupWebsocketCallbacks()
   {
      _testWs.OnOpen += () =>
      {
         Debug.Log("Connection open!");
         _lastReceivedMessage = "Connection open!";
      };

      _testWs.OnClose += (e) =>
      {
         Debug.Log("Connection closed!" + e);
         _lastReceivedMessage = "Connection closed";
      };

      _testWs.OnMessage += (bytes) =>
      {
         // Reading a plain text message
         string message = System.Text.Encoding.UTF8.GetString(bytes);
         Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
         _lastReceivedMessage= message;
      };

      _testWs.OnError += (e) =>
      {
         Debug.Log("Error! " + e);
      };
   }

   async public void ConnectToWs(){
      if(_testWs != null && (_testWs.State != WebSocketState.Open) && (_testWs.State != WebSocketState.Connecting)){
             Debug.Log("Connecting..."); 
            await _testWs.Connect();
      }
   }
   
    public async void SendWebSocketMessage(string message)
   {
      
      if (_testWs != null && _testWs.State == WebSocketState.Open)
      {
         //Debug.log("Sending Message: " + message);
         // Sending plain text
         await _testWs.SendText(message);
         
      }
   }

   public async void SendTestCommand(){
      if (_testWs != null && _testWs.State == WebSocketState.Open)
      {
         Debug.Log("Sending Message");
         await _testWs.SendText(JsonUtility.ToJson(_testCommand));
         Debug.Log("Message sent");
      }
   }

    // Start is called before the first frame update
    void Start()
    {
       Debug.Log("Start");
        _testWs = new WebSocket("wss://6322k0zjg2.execute-api.sa-east-1.amazonaws.com/demo");
        SetupWebsocketCallbacks();
        m_textMeshPro = GameObject.Find("result").GetComponent<TMP_Text>();
        _websocket = new WebSocket("ws://echo.websocket.org");
        Debug.Log("Finish Start");
    }

    // Update is called once per frame
    void Update()
    {
      //#if !UNITY_WEBGL || UNITY_EDITOR
      _websocket.DispatchMessageQueue();
      //#endif
      m_textMeshPro.SetText(_lastReceivedMessage);
    }
}
