using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCamera_script : MonoBehaviour
{
    GameObject _player;
    CinemachineVirtualCamera _cinemaCam;
    public static Vector3 _camera_xyz;
    public static Vector3 _camera_xyzBefore;

    void Awake()
    {
        //씬에 따라 초기 카메라 위치 설정.
        _cinemaCam = GetComponent<CinemachineVirtualCamera>();
        if (GameObject.Find("UIManager") != null)
        {
            _player = GameObject.Find("Player");
            _cinemaCam.Follow = _player.transform;
            _cinemaCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = +100;
        }
        else
        {
            _player = GameObject.Find("PlayerActor");
            _cinemaCam.Follow = _player.transform;
            _cinemaCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = -95;
        }
        _camera_xyz = _cinemaCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        _camera_xyzBefore = _camera_xyz;
    }

    void FixedUpdate()
    {
        //이벤트에 따라 카메라 위치 설정(_camera_xyz 변수 값만 바꾸면 어디서든 변경됨).
        _cinemaCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _camera_xyz;
    }
}
