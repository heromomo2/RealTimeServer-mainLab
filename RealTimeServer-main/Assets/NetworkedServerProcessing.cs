using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{
    
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientID);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.asd)
        {

        }
        else if (signifier == ClientToServerSignifiers.BalloonClicked)
        {
            gameLogic.PrcossBalloonClick(int.Parse(csv[1]));
            gameLogic.GivePlayerPoints(clientID);
        }
        //else if (signifier == ClientToServerSignifiers.asd)
        // {

        // }

        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int id)
    {
        networkedServer.SendMessageToClient(msg, id);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        gameLogic.AddClientConnection(clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);

        gameLogic.RemoveClientConnection(clientConnectionID);
    }
   

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int asd = 1;

    public const  int BalloonClicked = 2;
}

static public class ServerToClientSignifiers
{
    public const int asd = 1;

    public const int spawnBallon = 2;

    public const int BalloonPopped = 3;

    public const int ActiviePlayers = 4;

    public const int RemoveActiviePlayers = 5;
}

#endregion

