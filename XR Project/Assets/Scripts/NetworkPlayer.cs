using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    [SerializeField] Vector3 HeadBodyOffset;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("CameraOffset/Main Camera");
        leftHandRig = rig.transform.Find("CameraOffset/LeftHand");
        rightHandRig = rig.transform.Find("CameraOffset/RightHand");

        if (photonView.IsMine)
        {            
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                if (item.gameObject.tag == "Invisible")
                {
                    item.gameObject.layer = LayerMask.NameToLayer("Invisible");
                    //item.enabled = false;
                }
            } 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            //Used for hand model only.Not applicable to 3D avatar's hands.
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        if(rigTransform == headRig)
        {
            target.position = rigTransform.position - HeadBodyOffset;
            target.rotation = rigTransform.rotation;
        }
        else if((rigTransform == leftHandRig) || (rigTransform == rightHandRig))
        {
            target.position = rigTransform.position;
            target.rotation = rigTransform.rotation;
        }
    }
}