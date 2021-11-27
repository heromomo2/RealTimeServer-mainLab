using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    public int MaxinumBalloon = 150;
    public int Point = 10;

    float durationUntilNextBalloon;

    LinkedList<Playerinfo> ActiviePlayers;


    int lastUsedID;

    LinkedList<Ballooninfo> activeballoons;

    LinkedList<Ballooninfo> oldactiveballoons;

    void Start()
    {
        ActiviePlayers = new LinkedList<Playerinfo>();
        NetworkedServerProcessing.SetGameLogic(this);
        activeballoons = new LinkedList<Ballooninfo>();
        oldactiveballoons = new LinkedList<Ballooninfo>();
    }

    void Update()
    {

        // spawnning the balloons
        if ( (activeballoons.Count + oldactiveballoons.Count)< MaxinumBalloon )
        {
            durationUntilNextBalloon -= Time.deltaTime;

            if (durationUntilNextBalloon < 0)
            {
                lastUsedID++;



                durationUntilNextBalloon = 1f;

                float screenPositionXPercent = Random.Range(0.0f, 1.0f);
                float screenPositionYPercent = Random.Range(0.0f, 1.0f);
                //Vector2 screenPosition = new Vector2(screenPositionXPercent * (float)Screen.width, screenPositionYPercent * (float)Screen.height);
                // SpawnNewBalloon(screenPosition);



                string msg = ServerToClientSignifiers.spawnBallon + "," + screenPositionXPercent + "," + screenPositionYPercent + "," + lastUsedID;

                foreach (Playerinfo pi in ActiviePlayers)
                    NetworkedServerProcessing.SendMessageToClient(msg, pi.connetedClinetID);

                activeballoons.AddLast(new Ballooninfo(screenPositionXPercent, screenPositionYPercent, lastUsedID));
            } 
        
        }

        Debug.LogWarning("Current Balloon count : -> " + activeballoons.Count.ToString());

        /// When balloons is move old actives balloon list. 
        /// 

        if (activeballoons.Count > 0)
        {
            Ballooninfo b = null;
            b = activeballoons.First.Value;

            if (b != null)
            {
                Debug.LogWarning("b id : -> " + b.id.ToString());

                b.durationUntilIsToOldBalloon -= Time.deltaTime;

                if (b.durationUntilIsToOldBalloon < 0)
                {
                    oldactiveballoons.AddLast(b);
                    activeballoons.Remove(b);

                    string msg = ServerToClientSignifiers.OldBalloon + "," + b.id;

                    foreach (Playerinfo pi in ActiviePlayers)
                    {
                        NetworkedServerProcessing.SendMessageToClient(msg, pi.connetedClinetID);
                    }
                }

            }

        }
        // Debug.LogWarning("Current old Balloon count : -> " + oldactiveballoons.Count.ToString());

        // dele
        if (oldactiveballoons.Count > 0)
        {
            Ballooninfo b = null;
            b = oldactiveballoons.First.Value;

            if (b != null)
            {
                b.durationUntilBalloonSelfDestroy -= Time.deltaTime;

                if (b.durationUntilBalloonSelfDestroy < 0)
                {
                    string msg = ServerToClientSignifiers.BalloonPopped + "," + b.id;
                    oldactiveballoons.Remove(b);
                    foreach (Playerinfo pi in ActiviePlayers)
                    {
                        NetworkedServerProcessing.SendMessageToClient(msg, pi.connetedClinetID);
                    }
                }

            }

        }
    }

    public void AddClientConnection( int id) 
    {
        ActiviePlayers.AddLast( new Playerinfo (0,id));
        foreach (Ballooninfo Balloon in activeballoons) 
        {
            string msg = ServerToClientSignifiers.spawnBallon + "," + Balloon.percentX + "," + Balloon.percentY + "," + Balloon.id;

            NetworkedServerProcessing.SendMessageToClient(msg, id);
        }

        foreach (Ballooninfo Balloon in oldactiveballoons)
        {
            string msg = ServerToClientSignifiers.spawnOldBalloon + "," + Balloon.percentX + "," + Balloon.percentY + "," + Balloon.id;

            NetworkedServerProcessing.SendMessageToClient(msg, id);
        }
        // Update all the Client on ActiviePlayer Changes
        SendActiviePlayersToAllClient();
    }
    public void RemoveClientConnection(int id)
    {
        //PlayersInfos.Remove(id);
        Playerinfo SeachPlayer = FindPlayerwithID(id);

        if (SeachPlayer != null) 
        {
            ActiviePlayers.Remove(SeachPlayer);
        }

        // Update all the Client on ActiviePlayer Changes
        SendActiviePlayersToAllClient();
    }

    public void PrcossBalloonClick(int BalloonID) 
    {
        Ballooninfo bi = FindBalloonwithID( BalloonID);

        if (bi != null) 
        {
            activeballoons.Remove(bi);
            string msg = ServerToClientSignifiers.BalloonPopped+","+ BalloonID;
            foreach(Playerinfo pi in ActiviePlayers)
            NetworkedServerProcessing.SendMessageToClient(msg, pi.connetedClinetID);
        }
    }
    private Ballooninfo FindBalloonwithID (int BalloonID)
    {
        foreach (Ballooninfo  bi in activeballoons) 
        { 
            if(bi.id == BalloonID) 
            {
                return bi;
            }
        }
        return null;
    }

    private Playerinfo FindPlayerwithID(int PlayerID)
    {
        foreach (Playerinfo pi in ActiviePlayers)
        {
            if (pi.connetedClinetID == PlayerID)
            {
                return pi;
            }
        }
        return null;
    }

    public void GivePlayerPoints(int id)
    {
        Playerinfo SearchPlayerForPoints = FindPlayerwithID(id);

        if (SearchPlayerForPoints != null)
        {
            SearchPlayerForPoints.Score = SearchPlayerForPoints.Score + Point;
        }
        // Update all the Client on ActiviePlayer Changes
        SendActiviePlayersToAllClient();
    }


    private void SendActiviePlayersToAllClient()
    {
        
        foreach (Playerinfo PlayerCid in ActiviePlayers)
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.RemoveActiviePlayers +",", PlayerCid.connetedClinetID);
            foreach (Playerinfo playerinfo in ActiviePlayers) 
            {
                string msg = ServerToClientSignifiers.ActiviePlayers + "," + playerinfo.connetedClinetID + "," + playerinfo.Score;
                NetworkedServerProcessing.SendMessageToClient(msg, PlayerCid.connetedClinetID); 
            }
        }
    }
}


public class Ballooninfo
{
    public float percentX;

    public float percentY;

    public float durationUntilIsToOldBalloon = 2.0f;

    public float durationUntilBalloonSelfDestroy = 5.5f;

    public int id;


   public  Ballooninfo(float PercentX, float PercentY, int ID )
    {
        percentX = PercentX;
        percentY = PercentY;
        id = ID;
    }
}


public class Playerinfo
{
    

    public int Score;

    public int connetedClinetID;


    public Playerinfo( int score, int ID)
    {
        Score = score;
        connetedClinetID = ID;
    }
}