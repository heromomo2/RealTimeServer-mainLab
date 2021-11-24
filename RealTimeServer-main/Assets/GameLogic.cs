using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    float durationUntilNextBalloon;

    LinkedList<int> connetedClinetIDs;

    int lastUsedID;

    LinkedList<Ballooninfo> activeballoons;

    void Start()
    {
        connetedClinetIDs = new LinkedList<int>();
        NetworkedServerProcessing.SetGameLogic(this);
        activeballoons = new LinkedList<Ballooninfo>();
    }

    void Update()
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

            

            string msg = ServerToClientSignifiers.spawnBallon +","+ screenPositionXPercent + "," + screenPositionYPercent + "," + lastUsedID ;

            foreach (int cid in connetedClinetIDs)
              NetworkedServerProcessing.SendMessageToClient(msg, cid);

            activeballoons.AddLast(new Ballooninfo(screenPositionXPercent, screenPositionYPercent, lastUsedID));
        }
    }

    public void AddClientConnection( int id) 
    {
        connetedClinetIDs.AddLast(id);
        foreach (Ballooninfo Balloon in activeballoons) 
        {
            string msg = ServerToClientSignifiers.spawnBallon + "," + Balloon.percentX + "," + Balloon.percentY + "," + Balloon.id;

            NetworkedServerProcessing.SendMessageToClient(msg, id);
        }
    }
    public void RemoveClientConnection(int id)
    {
        connetedClinetIDs.Remove(id);
    }

    public void PrcossBalloonClick(int BalloonID) 
    {
        Ballooninfo bi = FindBalloonwithID( BalloonID);

        if (bi != null) 
        {
            activeballoons.Remove(bi);
            string msg = ServerToClientSignifiers.BalloonPopped+","+ BalloonID;
            foreach(int cid in connetedClinetIDs)
            NetworkedServerProcessing.SendMessageToClient(msg, cid);
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
}


public class Ballooninfo
{
    public float percentX;

    public float percentY;

    public int id;


   public  Ballooninfo(float PercentX, float PercentY, int ID )
    {
        percentX = PercentX;
        percentY = PercentY;
        id = ID;

    }
}