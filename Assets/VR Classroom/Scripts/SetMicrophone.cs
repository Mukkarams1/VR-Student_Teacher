using Photon.Pun;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Voice;

namespace ChiliGames
{
    public class SetMicrophone : MonoBehaviourPun
    {
        //detects device microphone and sets it to "Recorder" component from Photon Voice
        private void Start()
        {
            string[] devices = Microphone.devices;
            if (devices.Length > 0)
            {
                GetComponent<Recorder>().MicrophoneDevice = new DeviceInfo(devices[0]);
            }
        }
    }
}
