using System;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public bool UsingOrbitalCamera { get; private set; } = false;
    
    private InputHandler inputHandler;

    [SerializeField] private float _cameraZoomModifier = 32f;
    
    [SerializeField] private float _minCameraZoom = 1f;
    [SerializeField] private float _minOrbitCameraZoomDistance = 1f;
    [SerializeField] private float _maxCameraZoom = 12f;
    [SerializeField] private float _maxOrbitCameraZoomDistance = 36f;
    
    public CinemachineVirtualCameraBase _activeCamera;
    public CinemachineBrain CinemachineBrain { get; private set; }
    private int _activeCameraPriorityModifier = 1337;
    
    public Camera MainCamera { get; private set; }
    public CinemachineVirtualCamera thirdPersonCam;
    private CinemachineFramingTransposer _thirdPersonCamTransposer;
    public CinemachineVirtualCamera firstPersonCam;
    public CinemachineVirtualCamera orbitCam;
    private CinemachineFramingTransposer _orbitCamTransposer;

    private void Awake()
    {
        MainCamera = Camera.main;
        inputHandler = GetComponent<InputHandler>();
        CinemachineBrain = MainCamera.GetComponent<CinemachineBrain>();
        _thirdPersonCamTransposer = thirdPersonCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _orbitCamTransposer = orbitCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        ChangeCamera();
    }

    private void Update()
    {
        if(inputHandler.ZoomCameraInput != 0) ZoomCamera();
        if(inputHandler.CameraChangePressed) ChangeCamera();
    }

    private void ChangeCamera()
    {
        if (thirdPersonCam == _activeCamera)
        {
            SetCameraPriority(thirdPersonCam, firstPersonCam);
            UsingOrbitalCamera = false;
        }else if(firstPersonCam == _activeCamera){
            SetCameraPriority(firstPersonCam, orbitCam);
            UsingOrbitalCamera = true;
        }else if (orbitCam == _activeCamera)
        {
            SetCameraPriority(orbitCam, thirdPersonCam);
            UsingOrbitalCamera = false;
        }else 
        {
            thirdPersonCam.Priority += _activeCameraPriorityModifier;
            _activeCamera = thirdPersonCam;
            UsingOrbitalCamera = false;
        }
        CinemachineBrain.m_DefaultBlend.m_Time = 0.5f;

    }
    
    public void SetCameraTarget(Transform target)
    {
        CinemachineBrain.m_DefaultBlend.m_Time = 2f;
        thirdPersonCam.Follow = target;
        thirdPersonCam.LookAt = target;
        firstPersonCam.Follow = target;
        firstPersonCam.LookAt = target;
        orbitCam.Follow = target;
        orbitCam.LookAt = target;
    }
    
    private void SetCameraPriority(CinemachineVirtualCameraBase current, CinemachineVirtualCameraBase next)
    {
        current.Priority -= _activeCameraPriorityModifier;
        next.Priority += _activeCameraPriorityModifier;
        _activeCamera = next;
    }

    private void ZoomCamera()
    {
        if (_activeCamera == thirdPersonCam)
        {
            _thirdPersonCamTransposer.m_CameraDistance = Mathf.Clamp(_thirdPersonCamTransposer.m_CameraDistance +(inputHandler.InvertCameraZoom ? inputHandler.ZoomCameraInput : -inputHandler.ZoomCameraInput) /
                _cameraZoomModifier, _minCameraZoom, _maxCameraZoom);
                ;
        }else if (_activeCamera == orbitCam)
        {
            _orbitCamTransposer.m_CameraDistance =Mathf.Clamp(_orbitCamTransposer.m_CameraDistance +(inputHandler.InvertCameraZoom ? inputHandler.ZoomCameraInput : -inputHandler.ZoomCameraInput) /
                _cameraZoomModifier, _minOrbitCameraZoomDistance, _maxOrbitCameraZoomDistance);
        }
    }
}
