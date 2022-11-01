using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_script : MonoBehaviour
{
    public int[] _roomXY;   //0: 방 x값, 1: 방 y값.
    public GameObject[] _warpRoom = new GameObject[4];   //연결된 방(0: up, 1: left, 2:down, 3:right).
    GameObject _mapManager;
    Rigidbody2D _rig2d;
    PolygonCollider2D _poly;
    BoxCollider2D _box;

    void Awake()
    {
        _mapManager = GameObject.Find("MapManager");
        _poly = gameObject.transform.Find("Background").GetComponent<PolygonCollider2D>();
    }
}
