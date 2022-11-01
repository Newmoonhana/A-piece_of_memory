using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    InventoryMenu _invMenu;
    InventoryScript _invScript;
    GameObject _inv;    // CanvasInventory/Inventory object

    private void Awake()
    {
        _inv = GameObject.Find("CanvasInventory").transform.Find("Inventory").gameObject;
    }

    private void OnMouseEnter()
    {
        //인벤토리 아이템 칸 위에 마우스.
        _invMenu = transform.parent.parent.GetComponent<InventoryMenu>();
        if (_invMenu != null)
        {
            if (!_invMenu.isPlay)
            {
                _invMenu.item = transform;
            }
        }
    }

    private void OnMouseExit()
    {
        //인벤토리 아이템 칸 위에 마우스.
        if (_invMenu != null)
            if (!_invMenu.isPlay)
            {
                _invMenu.item = null;
            }
    }

    private void Update()
    {
        //우클릭 아이템 사용.
        if (Input.GetMouseButtonDown(1))
        {
            if (!Input.GetMouseButton(0))
            {
                if (transform.parent.parent.GetComponent<InventoryMenu>() != null)
                {
                    _invMenu = transform.parent.parent.GetComponent<InventoryMenu>();
                    if (_invMenu.item != null)
                    {
                        //추후 이름이 아니라 다른 파일에서 아이템 Id를 불러야함.
                        switch (_invMenu.item.name)
                        {
                            case "Portion00":
                                ItemSpend(000, -1, 1);
                                break;
                            case "Portion01":
                                ItemSpend(001, -1, 2);
                                break;
                        }
                    }
                }
            }
        }
    }

    //아이템 사용 함수.
    public void ItemSpend(int id, int _short, int hpup)   //id = id, _short = 단축키 버튼(-1: 비사용, 0: 왼쪽 마우스, 1: 오른쪽 마우스), hpup = hp 회복량.
    {
        //단축키 시 item을 단축키 오브젝트로 변경.
        _invMenu = _inv.transform.Find("Menu_0").GetComponent<InventoryMenu>();
        if (_short == 0)
            _invMenu.item = GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0);
        if (_short == 1)
            _invMenu.item = GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(1).GetChild(0);

        _invScript = _inv.transform.parent.GetComponent<InventoryScript>();
        for (int j = 0; j < _inv.transform.GetChild(1).childCount; j++)
        {
            if (_invScript._slotList[j] != null)
            {
                if (_invScript._slotList[j].name == _invMenu.item.gameObject.name)  //단축키도 구별을 위해 name으로.
                {
                    _invScript._itemM[id] -= 1;
                    //체력 포션 기준 체력 업.
                    Player_script._player_hp += hpup;
                    Debug.Log("플레이어 HP" + Player_script._player_hp);

                    //아이템 개수 표기 & 소지수 0개 시 삭제.
                    if (_invScript._itemM[id] > 0)
                    {
                        _invMenu.item.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _invScript._itemM[id].ToString();
                        _invMenu.item.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;

                        if (_short == -1)
                        {
                            if (_invMenu.item.name == GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).name)
                            {
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _invScript._itemM[id].ToString();
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                            }
                            else if (_invMenu.item.name == GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).name)
                            {
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _invScript._itemM[id].ToString();
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                            }
                        }
                        else
                        {
                            GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).transform.GetChild(j).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _invScript._itemM[id].ToString();
                            GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).transform.GetChild(j).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                        }
                    }
                    else   //소지수 0개.
                    {
                        if (_short == -1)
                        {
                            _invScript._slotList[j] = null;
                            Destroy(_invMenu.item.gameObject);
                            if (_invMenu.item.name == GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).name)
                            {
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).gameObject.name = "SlotItem";
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            }
                            if (_invMenu.item.name != GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).name)
                            {
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).gameObject.name = "SlotItem";
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            }
                            _invMenu.item = null;
                        }
                        else   //단축키 일때.
                        {
                            _invScript._slotList[j] = null;
                            _invMenu.item.gameObject.name = "SlotItem";
                            _invMenu.item.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                            _invMenu.item.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            if (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetChild(j).childCount != 0)
                                Destroy(GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetChild(j).GetChild(0).gameObject);
                        }
                    }
                    break;
                }
            }
        }
    }
}
