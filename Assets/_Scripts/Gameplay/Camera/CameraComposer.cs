using Unity.Cinemachine;
using UnityEngine;

public class CameraComposer : MonoBehaviour
{
    [SerializeField] CinemachineMixingCamera mixingCamera;
    [SerializeField] CinemachineOrbitalFollow orbitalCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //mixingCamera.ActiveBlend = orbitalCam.Orbits.Top.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
