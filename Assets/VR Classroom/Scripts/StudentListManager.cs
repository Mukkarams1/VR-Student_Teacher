using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;

public class StudentListManager : MonoBehaviourPunCallbacks
{
    public GameObject studentEntryPrefab; // Prefab for each student entry
    public Transform contentArea;         // Parent object for the list (ScrollView Content)
    private Dictionary<int, GameObject> studentEntries = new Dictionary<int, GameObject>();


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if(contentArea.transform.childCount > 0)
        {
            foreach (Transform child in contentArea.transform)
            {
                Destroy(child.gameObject);
            }
        }
        UpdateStudentList();
    }

    // Update the student list when players join/leave
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateStudentList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateStudentList();
    }

    private void UpdateStudentList()
    {
        // Clear existing entries
        foreach (var entry in studentEntries.Values)
        {
            Destroy(entry);
        }
        studentEntries.Clear();

        // Populate with current players
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.IsMasterClient)
            {
                GameObject entry = Instantiate(studentEntryPrefab, contentArea);
                TextMeshProUGUI nameText = entry.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
                Button muteButton = entry.transform.Find("MuteButton").GetComponent<Button>();

                // Set name and button behavior
                nameText.text = player.NickName;
                bool isMuted = false; // Initially not muted

                muteButton.onClick.AddListener(() =>
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        ToggleMute(player, muteButton);
                    }
                });

                muteButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mute";

                // Store the entry
                studentEntries[player.ActorNumber] = entry;
            }
        }
           
    }

    private void ToggleMute(Player player, Button button)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Determine mute or unmute
            bool isCurrentlyMuted = button.GetComponentInChildren<TextMeshProUGUI>().text == "Mute";

            PhotonNetwork.RaiseEvent(
                eventCode: 1, // Mute/unmute event code
                eventContent: new object[] { player.ActorNumber, isCurrentlyMuted },
                raiseEventOptions: new RaiseEventOptions { Receivers = ReceiverGroup.All },
                sendOptions: SendOptions.SendReliable
            );

            // Update button label
            button.GetComponentInChildren<TextMeshProUGUI>().text = isCurrentlyMuted ? "Unmute" : "Mute";
        }
    }

}
