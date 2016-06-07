using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;

public class MultiplayerController : RealTimeMultiplayerListener
{
    static MultiplayerController _instance;

    public static MultiplayerController Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MultiplayerController();

            return _instance;
        }
    }

    public MPLobbyListener lobbyListener;

    uint _minimumOpp = 1;
    uint _maximumOpp = 5;
    uint _gameVariation = 0;

    MultiplayerController()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public void SignInAndStartMPGame()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
                {
                    if (success)
                    {
                        Debug.Log("We're signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                        StartMatchMaking();
                    }
                    else
                    {
                        Debug.Log("Oh... we're not signed in.");
                    }
                });
        }
        else
        {
            Debug.Log("You're already signed in.");
            StartMatchMaking();
        }
    }

    public bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    void StartMatchMaking()
    {
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(_minimumOpp, _maximumOpp, _gameVariation, this);
    }

    private void ShowMPStatus(string message)
    {
        Debug.Log(message);

        if (lobbyListener != null)
            lobbyListener.SetLobbyStatusMessage(message);
    }

    public List<Participant> GetAllPlayers()
    {
        return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
    }

    public string GetMyParticipantId()
    {
        return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
    }

    #region RealTimeMultiplayerListener implementation

    public void OnRoomSetupProgress(float percent)
    {
        ShowMPStatus("We are " + percent + "% done with setup");
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            ShowMPStatus("We are connected to the room! I would probably start our game now.");

            lobbyListener.HideLobby();
            lobbyListener = null;
        }
        else
        {
            ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
        }
    }

    public void OnLeftRoom()
    {
        ShowMPStatus("We have left the room. We should probably perform some clean-up tasks.");
    }

    public void OnParticipantLeft(Participant participant)
    {
        throw new System.NotImplementedException();
    }

    public void OnPeersConnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has joined.");
        }
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has left.");
        }
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);
    }

    #endregion
}
