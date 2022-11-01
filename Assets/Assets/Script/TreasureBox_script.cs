using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox_script : MonoBehaviour
{
    public int _boxID;  //보물상자 ID(ItemDrop의 _tresureboxOpen에 대입. 상자 개봉 여부 체크 용(인스펙터에서 직접 써줘야함)).

    GameObject _itemDrop;
    public Sprite _boxOpen;
    public Sprite _boxClose;
    int[] Count;   //드롭 목록.
    int _num;   //아이템 드롭 개수.

    private void Awake()
    {
        _itemDrop = GameObject.Find("ItemDrop");
        if (!_itemDrop.GetComponent<ItemDrop>()._tresureboxOpen[_boxID])
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = _boxOpen;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = _boxClose;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 11)
        {
            if (!_itemDrop.GetComponent<ItemDrop>()._tresureboxOpen[_boxID])
            {
                if (!transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(true);

                if (Input.GetKeyDown("f"))
                {
                    Count = new int[11];
                    Count[0] = 000;
                    Count[1] = 001;
                    Count[2] = 100;
                    Count[3] = 101;
                    Count[4] = 200;
                    Count[5] = 201;
                    Count[6] = 300;
                    Count[7] = 400;
                    Count[8] = 500;
                    Count[9] = 600;
                    Count[10] = 700;
                    _num = 3;
                    _itemDrop.GetComponent<ItemDrop>().ItemRandom(transform.position, false, Count, _num);
                    gameObject.GetComponent<SpriteRenderer>().sprite = _boxClose;
                    _itemDrop.GetComponent<ItemDrop>()._tresureboxOpen[_boxID] = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //단축키 표시.
        if (other.gameObject.layer == 9 || other.gameObject.layer == 11)
        {
            if (transform.GetChild(0).gameObject.activeSelf)
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
