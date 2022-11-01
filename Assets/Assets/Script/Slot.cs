using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject _slotItem;    //슬롯 아이템 프리팹.
    public Sprite _thisSprite;

    private void OnMouseOver()
    {
        if (transform.parent.GetComponent<InventoryMenu>() != null) //슬롯.
        {
            if (transform.parent.GetComponent<InventoryMenu>().isPlay)
            {
                transform.parent.GetComponent<InventoryMenu>().slot = transform;
                transform.parent.parent.GetComponent<Collider2D>().enabled = false;
            }
        }
        else if (this.gameObject.name != "ShortSlot" && this.gameObject.name != "Equip" && this.transform.parent.name != "Equip")   //버리기.
        {
            for (int i = 1; i != 4; i++)
            {
                if (transform.GetChild(i).GetComponent<InventoryMenu>().isPlay)
                {
                    transform.GetChild(i).GetComponent<InventoryMenu>().slot = transform;
                }
            }
        }
        else if (this.gameObject.name == "ShortSlot")   //단축키 등록.
        {
            if (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetComponent<InventoryMenu>().item != null)
                if (GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().item != transform.GetChild(0))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        //추후 이름이 아니라 다른 파일에서 아이템 속성(회복/버프 구별)을 불러와서 그걸로 구별하게 바꿔야함.
                        int id = -1;
                        switch (GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().item.name)
                        {
                            case "Portion00":
                                id = 000;
                                break;
                            case "Portion01":
                                id = 001;
                                break;
                        }
                        switch (GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().item.name)
                        {
                            case "Portion00":
                            case "Portion01":
                                this.transform.parent.GetChild(0).GetChild(0).gameObject.name = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetComponent<InventoryMenu>().item.name;
                                this.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetComponent<InventoryMenu>().item.gameObject.GetComponent<Image>().sprite;
                                this.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                                this.transform.parent.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemM[id].ToString();
                                GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().slot = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetComponent<InventoryMenu>().select;
                                break;
                        }
                    }
                }
            if (Input.GetMouseButtonDown(1))
            {
                this.transform.GetChild(0).gameObject.name = "SlotItem";
                this.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _thisSprite;
                this.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                this.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        else if (this.gameObject.name == "Equip")   //장비 장착.
        {
            if (Input.GetMouseButtonUp(0) && GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item != null)
            {
                //나중에 이름이 아니라 장비 별로 구분하게 코드 바꿔야함.
                int A03 = -1;
                switch (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.name)
                {
                    //무기1 칸.
                    case "Weapon01_00":
                    case "Weapon01_01":
                        A03 = 0;
                        GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.GetComponent<Image>().sprite;
                        break;

                    //무기2 칸.
                    case "Weapon02_00":
                    case "Weapon02_01":
                        A03 = 1;
                        GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.GetComponent<Image>().sprite;
                        break;

                    //옷 칸.
                    case "Clothes00":
                        A03 = 2;
                        break;

                    //활 칸.
                    case "Bow00":
                        A03 = 3;
                        break;

                    //방패 칸.
                    case "Shild00":
                        A03 = 4;
                        break;

                    //반지 칸.
                    case "Ring00":
                        A03 = 5;
                        break;
                }
                GameObject.Find("Player").GetComponent<Player_script>()._playerSword = GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                GameObject.Find("Player").GetComponent<Player_script>()._playerSword2 = GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite;
                if (this.transform.GetChild(A03).GetChild(0).gameObject.name == "SlotItem")
                {
                    this.transform.GetChild(A03).GetChild(0).gameObject.name = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.name;
                    this.transform.GetChild(A03).GetChild(0).GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.GetComponent<Image>().sprite;

                    int id = -1;
                    switch (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject.name)
                    {
                        case "Weapon01_00":
                            id = 100;
                            break;
                        case "Weapon01_01":
                            id = 101;
                            break;
                        case "Weapon02_00":
                            id = 200;
                            break;
                        case "Weapon02_01":
                            id = 201;
                            break;
                        case "Clothes00":
                            id = 300;
                            break;
                        case "Bow00":
                            id = 400;
                            break;
                        case "Shild00":
                            id = 500;
                            break;
                        case "Ring00":
                            id = 600;
                            break;
                    }
                    GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemM[id] -= 1;

                    for (int j = 0; j < GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).childCount; j++)
                    {
                        if (GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j] != null)
                            if (GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j].gameObject == GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject)
                                GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j] = null;
                    }
                    Destroy(GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject);
                }
                else   //교체.
                {
                    //이름 교체.
                    string A01 = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject.name;
                    GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject.name = this.transform.GetChild(A03).GetChild(0).gameObject.name;
                    this.transform.GetChild(A03).GetChild(0).gameObject.name = A01;

                    Sprite A02 = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.GetComponent<Image>().sprite;
                    GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.GetComponent<Image>().sprite = this.transform.GetChild(A03).GetChild(0).GetComponent<Image>().sprite;
                    this.transform.GetChild(A03).GetChild(0).GetComponent<Image>().sprite = A02;

                    int id = -1;
                    switch (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject.name)
                    {
                        case "Weapon01_00":
                            id = 100;
                            break;
                        case "Weapon01_01":
                            id = 101;
                            break;
                        case "Weapon02_00":
                            id = 200;
                            break;
                        case "Weapon02_01":
                            id = 201;
                            break;
                        case "Clothes00":
                            id = 300;
                            break;
                        case "Bow00":
                            id = 400;
                            break;
                        case "Shild00":
                            id = 500;
                            break;
                        case "Ring00":
                            id = 600;
                            break;
                    }
                    GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemM[id] -= 1;
                    switch (this.transform.GetChild(A03).GetChild(0).gameObject.name)
                    {
                        case "Weapon01_00":
                            GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = this.transform.GetChild(A03).GetChild(0).gameObject.GetComponent<Image>().sprite;
                            id = 100;
                            break;
                        case "Weapon01_01":
                            GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = this.transform.GetChild(A03).GetChild(0).gameObject.GetComponent<Image>().sprite;
                            id = 101;
                            break;
                        case "Weapon02_00":
                            GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = this.transform.GetChild(A03).GetChild(0).gameObject.GetComponent<Image>().sprite;
                            id = 200;
                            break;
                        case "Weapon02_01":
                            GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = this.transform.GetChild(A03).GetChild(0).gameObject.GetComponent<Image>().sprite;
                            id = 201;
                            break;
                        case "Clothes00":
                            id = 300;
                            break;
                        case "Bow00":
                            id = 400;
                            break;
                        case "Shild00":
                            id = 500;
                            break;
                        case "Ring00":
                            id = 600;
                            break;
                    }
                    GameObject.Find("Player").GetComponent<Player_script>()._playerSword = GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                    GameObject.Find("Player").GetComponent<Player_script>()._playerSword2 = GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite;
                    GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemM[id] += 1;

                    for (int j = 0; j < GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).childCount; j++)
                    {
                        if (GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j] != null)
                            if (GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j].gameObject != GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject)
                                GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j] = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetComponent<InventoryMenu>().item.gameObject;
                    }
                }

                GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().slot = GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(1).GetComponent<InventoryMenu>().select;
            }
        }
        //장비 해제.
        else if (this.transform.parent.name == "Equip")
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (this.transform.gameObject.name != "Weapon01Slot" && this.transform.gameObject.name != "Weapon02Slot") //무기는 장착 해제 불가.
                    for (int j = 0; j < GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).childCount; j++)
                    {
                        if (GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetChild(j).childCount == 0)
                        {
                            GameObject _sloTem = Instantiate(_slotItem);
                            _sloTem.transform.SetParent(GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetChild(j));
                            _sloTem.transform.localPosition = Vector3.zero;
                            _sloTem.transform.localScale = Vector3.one;
                            _sloTem.name = this.transform.GetChild(0).gameObject.name;
                            _sloTem.GetComponent<Image>().sprite = this.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
                            GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._slotList[1 * 16 + j] = _sloTem;
                            GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).GetChild(j).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(0, 0, 0, 0);

                            int id = -1;
                            switch (this.transform.GetChild(0).gameObject.name)
                            {
                                case "Weapon02_00":
                                    id = 200;
                                    break;
                                case "Weapon02_01":
                                    id = 201;
                                    break;
                                case "Clothes00":
                                    id = 300;
                                    break;
                                case "Bow00":
                                    id = 400;
                                    break;
                                case "Shild00":
                                    id = 500;
                                    break;
                                case "Ring00":
                                    id = 600;
                                    break;
                            }
                            GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemM[id] += 1;
                            switch (this.gameObject.name)
                            {
                                //옷 칸.
                                case "ClothesSlot":
                                    this.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemIcon[4];
                                    break;
                                //활 칸.
                                case "BowSlot":
                                    this.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemIcon[3];
                                    break;
                                //방패 칸.
                                case "ShildSlot":
                                    this.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemIcon[2];
                                    break;
                                //링 칸.
                                case "RingSlot":
                                    this.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._itemIcon[5];
                                    break;
                            }
                            this.transform.GetChild(0).gameObject.name = "SlotItem";
                            break;
                        }
                        if (j >= GameObject.Find("CanvasInventory").transform.Find("Inventory").GetChild(2).childCount - 1)
                            Debug.Log("소지품이 가득 찼습니다.");
                    }
            }
        }
    }

    private void OnMouseExit()
    {
        if (transform.parent.GetComponent<InventoryMenu>() != null) //슬롯.
        {
            transform.parent.GetComponent<InventoryMenu>().slot = null;
            transform.parent.parent.GetComponent<Collider2D>().enabled = true;
        }
        else if (this.gameObject.name != "ShortSlot" && this.gameObject.name != "Equip" && this.transform.parent.name != "Equip")   //버리기.
        {
            for (int i = 1; i != 4; i++)
            {
                transform.GetChild(i).GetComponent<InventoryMenu>().slot = null;
            }
        }
    }
}
