using Unity.Cinemachine;
using UnityEngine;

public class CameraComposer : MonoBehaviour
{
    [SerializeField] CinemachineMixingCamera mixingCamera;
    [SerializeField] CinemachineOrbitalFollow orbitalCam;
    [SerializeField] CinemachineCamera dragCamera;
    [SerializeField] GameObject cameraControl;
    private float topRingRadius;
    [SerializeField,Range(0,1)] float minProximityBeforeBlend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
       topRingRadius = orbitalCam.Orbits.Top.Radius;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceRatio = Vector3.Distance(cameraControl.transform.position, new Vector3 (0, cameraControl.transform.position.y, 0))/topRingRadius;
        distanceRatio = Mathf.Clamp01(distanceRatio);
        if (distanceRatio >= minProximityBeforeBlend)
        {
            mixingCamera.Weight0 = distanceRatio;
            mixingCamera.Weight1 = 1 - distanceRatio;
        }
        else
        {
            mixingCamera.Weight1 = 1;
            mixingCamera.Weight0 = 0;
        }
    }

}
