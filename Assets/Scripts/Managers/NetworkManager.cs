using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    public Photon.Realtime.Player player;

    private List<RoomInfo> servers;
    private bool connected;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        player = PhotonNetwork.LocalPlayer;
        servers = new List<RoomInfo>();
        connected = false;
    }

    override
    public void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        GameObject.Find("MenuManager").GetComponent<MenuManager>().LoadPlayers();
    }

    override
    public void OnPlayerLeftRoom(Photon.Realtime.Player player)
    {
        GameObject.Find("MenuManager").GetComponent<MenuManager>().LoadPlayers();
        if(GameManager.Instance != null)
        {
            GameManager.Instance.HandlePlayerLeftGame(player);
        }
    }

    override
    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            servers.Add(room);
        }
    }

    override
    public void OnConnectedToMaster()
    {
        connected = true;
    }

    override
    public void OnJoinedRoom()
    {
        GameObject.Find("MenuManager").GetComponent<MenuManager>().OpenRoom();
    }

    public List<RoomInfo> GetServerList()
    {
        return servers;
    }


    public void CreateRoom(string room)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(room, roomOptions, null);
    }

    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void LoadLevel(int level)
    {
        PhotonNetwork.LoadLevel(level);
        Destroy(gameObject);
    }

    public void DisconnectFromLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public bool IsInLobby()
    {
        return PhotonNetwork.InLobby;
    }

    public bool IsInRoom()
    {
        return PhotonNetwork.InRoom;
    }

    public bool IsConnected()
    {
        return connected;
    }

    public bool IsMaster()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public string GetRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }

    public Photon.Realtime.Player[] GetRoomPlayers()
    {
        return PhotonNetwork.PlayerList;
    }
}
