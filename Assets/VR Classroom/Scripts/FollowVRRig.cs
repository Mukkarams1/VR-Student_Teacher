using UnityEngine;
using Photon.Pun;

namespace ChiliGames.VRClassroom {
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class FollowVRRig : MonoBehaviour {
        public Transform[] body;
        [SerializeField] SkinnedMeshRenderer lHand;
        [SerializeField] SkinnedMeshRenderer rHand;


        PhotonView pv;

        private void Awake() {

            pv = GetComponent<PhotonView>();

            if(pv.IsMine)
            if (PlatformManager.instance.mode == PlatformManager.Mode.Teacher)
            {
                EnableRenderers();
            }


        }

        void EnableRenderers() {
            lHand.enabled = true;
            rHand.enabled = true;
        }

      
    }
}
