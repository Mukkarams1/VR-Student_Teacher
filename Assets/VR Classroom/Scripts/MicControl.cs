using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class MicControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button muteButton; // Button to toggle mute/unmute
    private bool isMuted = false; // Track mute state

    // Make PhotonVoiceView private
    private PhotonVoiceView voiceView;

    private void Start()
    {
        Debug.Log("MicControl1 script started.");

        // Check if this client is the master client (teacher)
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("This client is the Master Client (Teacher).");
            muteButton.gameObject.SetActive(true); // Show button for master client
            muteButton.onClick.AddListener(ToggleMute); // Add listener to button
            Debug.Log("Mute button is enabled for the Teacher.");
        }
        else
        {
            Debug.Log("This client is not the Master Client (Student).");
            muteButton.gameObject.SetActive(false); // Hide button for students
            Debug.Log("Mute button is disabled for the Student.");
        }
    }

    // Make the function public for external use
    public void ToggleMute()
    {
        isMuted = !isMuted; // Toggle mute state
        string action = isMuted ? "Muted" : "Unmuted";
        Debug.Log($"Teacher toggled mute state. Current state: {action}");

        MuteOrUnmuteAllStudents(action);
    }

    public void MuteOrUnmuteAllStudents(string action)
    {
        foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
        {
            // Check if the PhotonView belongs to a student
            if (photonView.Owner != null && photonView.Owner.CustomProperties.ContainsKey("Role") &&
                photonView.Owner.CustomProperties["Role"].ToString() == "Student")
            {
                Debug.Log($"Student found: {photonView.Owner.NickName}");

                // Access PhotonVoiceView
                voiceView = photonView.GetComponent<PhotonVoiceView>();
                if (voiceView != null)
                {
                    Debug.Log($"PhotonVoiceView found for {photonView.Owner.NickName}");

                    // Mute/unmute Speaker (for receiving audio from others)
                    if (voiceView.SpeakerInUse != null)
                    {
                        ToggleMuteSpeaker(voiceView.SpeakerInUse, isMuted); // Mute/unmute using ToggleMuteSpeaker
                        Debug.Log($"{photonView.Owner.NickName}'s speaker has been {action}");
                    }
                    else
                    {
                        Debug.LogWarning($"SpeakerInUse not found for {photonView.Owner.NickName}");
                    }

                    // used this code to mute the player by using the speaker 
            //        if (voiceView.SpeakerInUse != null)
            //        {
            //            voiceView.SpeakerInUse.enabled = !isMuted;  // Muting or unmuting the student's speaker
            //            Debug.Log($"{photonView.Owner.NickName}'s speaker has been {action}");
            //        }
            //        else
            //        {
            //            Debug.LogWarning($"SpeakerInUse not found for 
            //        }

                    // Mute/unmute Microphone (for sending audio to others)  
                Recorder recorder = voiceView.GetComponent<Recorder>(); // Access the Recorder component
                    if (recorder != null)
                    {
                        recorder.TransmitEnabled = !isMuted; // Muting or unmuting the student's microphone
                        Debug.Log($"{photonView.Owner.NickName}'s microphone has been {action}");
                    }
                    else
                    {
                        Debug.LogWarning($"Recorder not found for {photonView.Owner.NickName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"PhotonVoiceView not found for {photonView.Owner.NickName}");
                }
            }
        }

        Debug.Log($"All students have been {action}.");
    }

    // Function to mute/unmute the student's speaker using AudioSource.mute
    public bool ToggleMuteSpeaker(Speaker speaker, bool mute)
    {
        AudioSource audioSource = speaker.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.mute = mute;  // Mute or unmute the speaker's audio
            Debug.Log($"Speaker audio for {speaker.gameObject.name} has been {(mute ? "muted" : "unmuted")}");

            return true;
        }
        else
        {
            Debug.LogWarning($"AudioSource not found for {speaker.name}");
            return false;
        }
    }





    // Called when a new player enters the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // Check if the new player is a student and mute/unmute accordingly
        if (newPlayer.CustomProperties.ContainsKey("Role") && newPlayer.CustomProperties["Role"].ToString() == "Student")
        {
            MuteOrUnmuteAllStudents(isMuted ? "Muted" : "Unmuted");
        }
    }
}
