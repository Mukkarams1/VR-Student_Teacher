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
        // Example variables for storing lobby state
        private string savedLobbyName = "";
        private string savedRoomName = "";

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

            // Generate a unique 4-length name using random letters and digits
            string customLobbyName = GenerateUniqueLobbyName();
            // Save the generated custom lobby name
            savedLobbyName = customLobbyName;  
            Debug.Log("Joined Lobby: " + savedLobbyName);  // Log the lobby name to the console

            MenuManager.instance.setState(menuState.Login);
        }

        // Helper function to generate a unique 4-character name
        private string GenerateUniqueLobbyName()
        {
            const string chars = "ABCDEFG";  // Available characters
            System.Random random = new System.Random();
            char[] name = new char[4];

            for (int i = 0; i < name.Length; i++)
            {
                name[i] = chars[random.Next(chars.Length)];
            }

            return new string(name);  // Return the generated name as a string
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
            savedRoomName = PhotonNetwork.CurrentRoom.Name; // Store room name
            Debug.Log("Joined Room: " + savedRoomName);
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

        public void EndSession()
        {
            if (PhotonNetwork.IsMasterClient) // Ensure only the teacher can end the session
            {
                Debug.Log("Ending session. Closing room.");
                PhotonNetwork.CurrentRoom.IsOpen = false; // Prevent new players from joining
                PhotonNetwork.CurrentRoom.IsVisible = false; // Hide the room from the lobby
                PhotonNetwork.LeaveRoom(); // Disconnect the teacher (master client)

                // Optionally, you could use a coroutine here to delay loading
                StartCoroutine(ReloadLobbyAndReconnect());
            }
            else
            {
                Debug.LogWarning("Only the master client (teacher) can end the session.");
            }
        }

        IEnumerator ReloadLobbyAndReconnect()
        {
            // Wait until the teacher has left the room
            yield return new WaitForEndOfFrame();

            // Rejoin the saved lobby
            PhotonNetwork.JoinLobby(TypedLobby.Default); // Join the default lobby (or the saved one)

            // Optional: You could also join the exact same room if needed
            if (!string.IsNullOrEmpty(savedRoomName))
            {
                PhotonNetwork.JoinRoom(savedRoomName); // Rejoin the exact same room if desired
            }
        }


    }
}