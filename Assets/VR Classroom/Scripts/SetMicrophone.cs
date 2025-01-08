using Photon.Pun;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Voice;
using ExitGames.Client.Photon;
using Photon.Voice.PUN;

namespace ChiliGames
{
    public class SetMicrophone : MonoBehaviourPun
    {
        public PhotonVoiceView voiceView;
        //detects device microphone and sets it to "Recorder" component from Photon Voice
        private void Start()
        {
            string[] devices = Microphone.devices;
            if (devices.Length > 0)
            {
                GetComponent<Recorder>().MicrophoneDevice = new DeviceInfo(devices[0]);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        private void OnEventReceived(EventData photonEvent)
        {
            if (photonEvent.Code == 1) // Mute/Unmute event
            {
                object[] data = (object[])photonEvent.CustomData;
                int targetActorNumber = (int)data[0];
                bool shouldMute = (bool)data[1];

                if (PhotonNetwork.LocalPlayer.ActorNumber == targetActorNumber)
                {
                    if (voiceView != null )
                    {
                        voiceView.RecorderInUse.TransmitEnabled = !shouldMute; // Toggle TransmitEnabled
                    }
                }
            }
        }

    }
}
