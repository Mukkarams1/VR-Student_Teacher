using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using static PlayFabLogin;

//
//This script connects to PHOTON servers and creates a room if there is none, then automatically joins
//
namespace Networking.Pun2
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        bool triesToConnectToMaster = false;
        bool triesToConnectToRoom = false;
        bool isInlobby = false;

        private void Update()
        {
            if (!PhotonNetwork.IsConnected && !triesToConnectToMaster)
            {
                ConnectToMaster();
            }
            if (PhotonNetwork.IsConnected && !triesToConnectToMaster && !triesToConnectToRoom && isInlobby)
            {
                StartCoroutine(WaitFrameAndConnect());
            }
        }

        public void ConnectToMaster()
        {
            //PhotonNetwork.OfflineMode = false; //true would "fake" an online connection
            //PhotonNetwork.NickName = "PlayerName"; //we can use a input to change this 
            //PhotonNetwork.AutomaticallySyncScene = true; //To call PhotonNetwork.LoadLevel()
            //PhotonNetwork.GameVersion = "v1"; //only people with the same game version can play together
            //PhotonNetwork.UseRpcMonoBehaviourCache = true; //For better performance

            triesToConnectToMaster = true;
            //PhotonNetwork.ConnectToMaster(ip, port, appid); //manual connection
            PhotonNetwork.ConnectUsingSettings(); //automatic connection based on the config file
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            triesToConnectToMaster = false;
            triesToConnectToRoom = false;
            Debug.Log(cause);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            triesToConnectToMaster = false;
            Debug.Log("Connected to master!");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            isInlobby = true;
            Debug.Log("Joined Lobby");
            MenuManager.instance.setState(menuState.Login);
        }

        IEnumerator WaitFrameAndConnect()
        {
            triesToConnectToRoom = true;
            yield return new WaitForEndOfFrame();
            Debug.Log("Connecting");
        }

        public void CreateRoom()
        {
            if(isInlobby && MenuManager.instance.mode == ChiliGames.VRClassroom.PlatformManager.Mode.Teacher)
            {
                PhotonNetwork.CreateRoom(UserData.Subject, new RoomOptions { MaxPlayers = 15 });
                MenuManager.instance.loadingPanel.SetActive(true);
            }
        }

        public void JoinRoom()
        {
            if (isInlobby && MenuManager.instance.mode != ChiliGames.VRClassroom.PlatformManager.Mode.Teacher)
                PhotonNetwork.JoinRoom(MenuManager.instance.selectedSubject_student);
        }
        public override void OnJoinedRoom()
        {
            //Go to next scene after joining the room
            base.OnJoinedRoom();
            Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
            
            SceneManager.LoadScene("Classroom"); //go to the room scene
            MenuManager.instance.loadingPanel.SetActive(false);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("No room available, creating one");
            base.OnJoinRandomFailed(returnCode, message);
        }
    }
}