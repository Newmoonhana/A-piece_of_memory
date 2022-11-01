using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Portal_script : MonoBehaviour
{
    GameObject _room;
    Map_script _roomScript;

    GameObject _player;
    Animator _playerAni;

    void Awake()
    {
        _room = transform.parent.parent.gameObject;
        _roomScript = _room.GetComponent<Map_script>();
        _player = GameObject.Find("Player");
        _playerAni = _player.transform.GetChild(0).GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            if (Input.GetKeyDown("f"))
            {
                Debug.Log("f");
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).gameObject == gameObject)
                    {
                        Map_script _roomScript = transform.parent.parent.gameObject.GetComponent<Map_script>();
                        StartCoroutine("PlayerMoveRoom", i);
                        break;
                    }
                }
            }
        }
    }

    IEnumerator PlayerMoveRoom(int i)
    {
        Player_script._dontmove = true;
        _player.SetActive(false);

        GameObject _cine = GameObject.Find("CM vcam1");

        _cine.GetComponent<CinemachineConfiner>().m_BoundingShape2D = null;
        _cine.GetComponent<CinemachineConfiner>().m_ConfineScreenEdges = false;
        _cine.GetComponent<CinemachineVirtualCamera>().m_Follow = null;
        _cine.transform.position = GameObject.Find("Main Camera").transform.position;

        Vector3 _playerPos = Camera.main.WorldToViewportPoint(_player.transform.position);
        Vector3 _cinePos = Camera.main.WorldToViewportPoint(_cine.transform.position);
        Vector3 _portalPos = Camera.main.WorldToViewportPoint(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(3).position);

        switch (i)
        {
            case 0: //up
                for (int j = 0; _cinePos.y < _portalPos.y; j++)
                {
                    _cinePos = Camera.main.WorldToViewportPoint(_cine.transform.position);
                    _portalPos = Camera.main.WorldToViewportPoint(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(2).position);
                    _cine.transform.position = new Vector3(_cine.transform.position.x, Mathf.MoveTowards(_cine.transform.position.y, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(2).position.y, 300 * Time.deltaTime), _cine.transform.position.z);
                    yield return null;
                }
                _player.transform.position = new Vector3(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(2).position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(2).position.y, _player.transform.position.z);
                break;
            case 1: //left
                for (int j = 0; _cinePos.x > _portalPos.x; j++)
                {
                    _cinePos = Camera.main.WorldToViewportPoint(_cine.transform.position);
                    _portalPos = Camera.main.WorldToViewportPoint(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(3).position);
                    _cine.transform.position = new Vector3(Mathf.MoveTowards(_cine.transform.position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(3).position.x, 300 * Time.deltaTime), _cine.transform.position.y, _cine.transform.position.z);
                    yield return null;
                }
                _player.transform.position = new Vector3(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(3).position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(3).position.y, _player.transform.position.z);
                break;
            case 2: //down
                for (int j = 0; _cinePos.y > _portalPos.y; j++)
                {
                    _cinePos = Camera.main.WorldToViewportPoint(_cine.transform.position);
                    _portalPos = Camera.main.WorldToViewportPoint(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(0).position);
                    _cine.transform.position = new Vector3(_cine.transform.position.x, Mathf.MoveTowards(_cine.transform.position.y, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(0).position.y, 300 * Time.deltaTime), _cine.transform.position.z);
                    yield return null;
                }
                _player.transform.position = new Vector3(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(0).position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(0).position.y, _player.transform.position.z);
                break;
            case 3: //right
                for (int j = 0; _cinePos.x < _portalPos.x; j++)
                {
                    _cinePos = Camera.main.WorldToViewportPoint(_cine.transform.position);
                    _portalPos = Camera.main.WorldToViewportPoint(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(1).position);
                    _cine.transform.position = new Vector3(Mathf.MoveTowards(_cine.transform.position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(1).position.x, 300 * Time.deltaTime), _cine.transform.position.y, _cine.transform.position.z);
                    yield return null;
                }
                _player.transform.position = new Vector3(_roomScript._warpRoom[i].transform.Find("Portal").GetChild(1).position.x, _roomScript._warpRoom[i].transform.Find("Portal").GetChild(1).position.y, _player.transform.position.z);
                break;
        }

        _player.SetActive(true);
        GameObject _newcine = Instantiate(_cine, Vector3.zero, Quaternion.identity);
        Destroy(GameObject.Find("CM vcam1"));
        _newcine.name = "CM vcam1";
        _cine = _newcine;
        _cine.GetComponent<CinemachineConfiner>().m_BoundingShape2D = _roomScript._warpRoom[i].transform.Find("Background").GetComponent<Collider2D>();
        _cine.GetComponent<CinemachineConfiner>().m_ConfineScreenEdges = true;
        _cine.GetComponent<CinemachineVirtualCamera>().m_Follow = _player.transform;
        _player.SetActive(false);

        yield return new WaitForSeconds(1f);

        _player.SetActive(true);
        switch (i)
        {
            case 0: //up
            case 2: //down
                _playerAni.SetInteger("Direction", _playerAni.GetInteger("Direction"));
                break;
            case 1: //left
                _playerAni.SetInteger("Direction", -1);
                break;
            case 3: //right
                _playerAni.SetInteger("Direction", 1);
                break;
        }

        Player_script._dontmove = false;
    }
}
