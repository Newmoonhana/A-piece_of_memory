using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    public int stage = 0;   //스테이지.
    public int[] _roomCount = new int[2];  //방 개수(0: 총 방 개수(시작 방 포함), 1: 몬스터 방).
    public GameObject[] _firstRoom; //시작 방.
    public GameObject[] _monRoom;   //몬스터 방.

    GameObject _beforeRoom; //이전 방.
    GameObject _beforePortal;   //이전 방 포탈.
    GameObject _standRoom;  //현재 방.
    GameObject _standPortal;    //현재 방 포탈.

    public GameObject _cine;  //시네 머신.

    void Awake()
    {
        GameObject _Room = Instantiate(_firstRoom[stage], Vector3.zero, Quaternion.identity);
        _Room.name = _firstRoom[stage].name;
        _Room.transform.SetParent(transform);

        _beforeRoom = _firstRoom[stage];
        _beforePortal = _beforeRoom.transform.Find("Portal").gameObject;
        _standRoom = _beforeRoom;
        _standPortal = _beforePortal;
        _cine.GetComponent<CinemachineConfiner>().m_BoundingShape2D = _Room.transform.Find("Background").GetComponent<Collider2D>();

        switch (stage)
        {
            case 0:
                _roomCount[0] = 4;
                _roomCount[1] = 3;
                break;
        }
    }

    void Update()
    {
        if (transform.childCount < _roomCount[0])
        {
            Debug.Log(transform.childCount);
            Debug.Log(_roomCount[0]);
            RoomInstant();
        }
    }

    public void RoomInstant()
    {
        _beforeRoom = transform.GetChild(Random.Range(0, transform.childCount)).gameObject;
        _beforePortal = _beforeRoom.transform.Find("Portal").gameObject;
        int count = -1;
        //몬스터 방.
        if (transform.childCount - 1 < _roomCount[1])
        {
            count = Random.Range(0, _monRoom.Length);
            int count2 = count;
            for (int ij = 0; ij < transform.childCount; ij++)
            {
                Debug.Log("카운터" + count);
                Debug.Log("카운터" + transform.GetChild(ij).gameObject);
                Debug.Log("카운터" + _monRoom[count].gameObject);
                if (transform.GetChild(ij).gameObject == _monRoom[count].gameObject)
                {
                    while (count2 == count)
                        count = Random.Range(0, _monRoom.Length);
                    Debug.Log("카운터" + count);
                    ij -= 1;
                }
            }
        }
        else
        {
            Debug.Log("생성 방 이상 함수가 돌음");
            return;
        }
        int dir = Random.Range(0, 4); //Random.Range(0, max);했을 때, 0 ~ (max - 1)의 랜덤 값을 반환.

        while (_beforePortal.transform.parent.GetComponent<Map_script>()._warpRoom[dir] != null)   //맵 위치 중복 체크.
            dir = Random.Range(0, 4);

        GameObject _Room = Instantiate(_monRoom[count], Vector3.zero, Quaternion.identity);
        Debug.Log("Room" + _Room);

        Vector3 _RoomPos = Camera.main.ViewportToWorldPoint(_Room.transform.Find("Portal").GetChild(dir).position);
        Vector3 _beforeRoomPos = Camera.main.ViewportToWorldPoint(_beforePortal.transform.GetChild(dir).position);
        switch (dir)
        {
            case 0:
                _RoomPos = Camera.main.ViewportToWorldPoint(_Room.transform.Find("Portal").transform.GetChild(2).position);
                break;
            case 1:
                _RoomPos = Camera.main.ViewportToWorldPoint(_Room.transform.Find("Portal").transform.GetChild(3).position);
                break;
            case 2:
                _RoomPos = Camera.main.ViewportToWorldPoint(_Room.transform.Find("Portal").transform.GetChild(0).position);
                break;
            case 3:
                _RoomPos = Camera.main.ViewportToWorldPoint(_Room.transform.Find("Portal").transform.GetChild(1).position);
                break;
        }
        float i = 0;
        float j = 0;
        i = _beforeRoomPos.x - _RoomPos.x;
        j = _beforeRoomPos.y - _RoomPos.y;
        Vector3 _pos = Camera.main.WorldToViewportPoint(new Vector3(i, j, 0));

        switch (dir)
        {
            case 0: //up
                _Room.transform.position = new Vector3(_pos.x + 32, _beforeRoom.GetComponent<Map_script>()._roomXY[1] * 0.5f + _monRoom[count].GetComponent<Map_script>()._roomXY[1] * 0.5f + 32, 0);
                _Room.GetComponent<Map_script>()._warpRoom[2] = _beforeRoom;
                break;
            case 1: //left
                _Room.transform.position = new Vector3(-(_beforeRoom.GetComponent<Map_script>()._roomXY[0] * 0.5f + _monRoom[count].GetComponent<Map_script>()._roomXY[0] * 0.5f + 32), _pos.y + 32, 0);
                _Room.GetComponent<Map_script>()._warpRoom[3] = _beforeRoom;
                break;
            case 2: //down
                _Room.transform.position = new Vector3(_pos.x + 32, -(_beforeRoom.GetComponent<Map_script>()._roomXY[1] * 0.5f + _monRoom[count].GetComponent<Map_script>()._roomXY[1] * 0.5f + 32), 0);
                _Room.GetComponent<Map_script>()._warpRoom[0] = _beforeRoom;
                break;
            case 3: //right
                _Room.transform.position = new Vector3(_beforeRoom.GetComponent<Map_script>()._roomXY[0] * 0.5f + _monRoom[count].GetComponent<Map_script>()._roomXY[0] * 0.5f + 32, _pos.y + 32, 0);
                _Room.GetComponent<Map_script>()._warpRoom[1] = _beforeRoom;
                break;
        }
        _Room.name = _monRoom[count].name;
        _Room.transform.SetParent(transform);

        _beforeRoom.GetComponent<Map_script>()._warpRoom[dir] = _Room.gameObject;

        Rigidbody2D _roomRig2d = _Room.AddComponent<Rigidbody2D>();
        _roomRig2d.sleepMode = RigidbodySleepMode2D.NeverSleep;
        BoxCollider2D _box = _Room.AddComponent<BoxCollider2D>();
        _box.size = new Vector3(_Room.GetComponent<Map_script>()._roomXY[0], _Room.GetComponent<Map_script>()._roomXY[1], 0);
        if (_Room.transform.parent != null)
        {
            Debug.Log("01 " + _Room);
            if (_Room.transform.parent.gameObject == this.gameObject)
            {
                Debug.Log("02 " + _Room.transform.parent.gameObject);
                Debug.Log("box " + _box);
                _box.enabled = false;
                RaycastHit2D hit = Physics2D.BoxCast(_box.transform.position, new Vector2(_Room.GetComponent<Map_script>()._roomXY[0], _Room.GetComponent<Map_script>()._roomXY[1]), 0, new Vector2(-(_Room.GetComponent<Map_script>()._roomXY[0] * 0.5f), -(_Room.GetComponent<Map_script>()._roomXY[1]) * 0.5f), 0f, 1 << 18);
                Debug.Log("02.5 " + hit.collider);
                if (hit.collider != null)
                {
                    Debug.Log("03 " + hit.collider.transform.gameObject);
                    if (hit.collider.transform.parent.gameObject == this.gameObject)
                    {
                        Debug.Log("04 " + hit.collider.transform.gameObject);
                        for (int k = 1; k < transform.childCount; k++)
                        {
                            Debug.Log("05 " + hit.collider.transform.parent.gameObject);
                            Debug.Log("06 " + this.transform.GetChild(k).gameObject);
                            if (this.transform.GetChild(k).gameObject == hit.collider.transform.gameObject)
                            {
                                Debug.Log(this.transform.Find(hit.collider.transform.name));
                                if (transform.Find(_beforeRoom.name).GetComponent<Map_script>()._warpRoom[dir] != null)
                                {
                                    switch (dir)
                                    {
                                        case 0:
                                        case 2:
                                        case 3:
                                            transform.Find(_beforeRoom.name).GetComponent<Map_script>()._warpRoom[Mathf.Abs(dir - 2)] = null;
                                            break;
                                        case 1:
                                            transform.Find(_beforeRoom.name).GetComponent<Map_script>()._warpRoom[Mathf.Abs(3)] = null;
                                            break;
                                    }
                                }
                                Destroy(_Room);
                                _Room = null;
                                Debug.Log(transform.childCount - 1);
                                break;
                            }
                        }
                    }
                }
            }
        }
        if (_Room != null)
        {
            Destroy(_roomRig2d);
            _box.enabled = true;
            _standRoom = _Room;
            _standPortal = _standRoom.transform.Find("Portal").gameObject;
        }
        for (int o = 0; o < 4; o++)
        {
            if (_beforeRoom.GetComponent<Map_script>()._warpRoom[dir] == null)  //변수가 missing떠서 true되는 버그 있음.
                _beforePortal.transform.GetChild(o).gameObject.SetActive(false);
            else
                _beforePortal.transform.GetChild(o).gameObject.SetActive(true);

            if (_standRoom.GetComponent<Map_script>()._warpRoom[dir] == null)
                _standPortal.transform.GetChild(o).gameObject.SetActive(false);
            else
                _standPortal.transform.GetChild(o).gameObject.SetActive(true);
        }
    }
}
