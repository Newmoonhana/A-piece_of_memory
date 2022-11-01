using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_script : MonoBehaviour
{
    public Sprite _Zsp;
    Animator _ani;
    int _dir;   //원래 방향.
    public bool _dirSwitch;    //방향고정: true.

    GameObject _player;
    float _playersDis;

    void Awake()
    {
        _player = GameObject.Find("Player");
        _ani = GetComponent<Animator>();
    }

    void Start ()
    {
        _dir = _ani.GetInteger("Direction");
	}

    void FixedUpdate()
    {
        //어그로 범위 설정.
        if (!Player_script._isFade && !Player_script._dontmove)
        {
            _playersDis = Vector3.Distance(_player.transform.position, transform.position);
            if (_playersDis <= 100)
            {
                //방향 플레이어.
                if (_dirSwitch)
                {
                    int j = _ani.GetInteger("Direction");  //애니메이션 값 대입 용 변수.
                    j = transform.position.x < _player.transform.position.x ? 1 : -1;
                    _ani.SetInteger("Direction", j);
                }

                //단축키 표시.
                if (!transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                //원래 방향.
                if (_dirSwitch)
                    _ani.SetInteger("Direction", _dir);

                //단축키 표시.
                if (transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            //단축키 표시.
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
