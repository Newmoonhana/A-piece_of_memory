using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemObj : MonoBehaviour
{
    Rigidbody2D _rid2d;
    public GameObject _inv;
    public GameObject _slotItem;
    bool playIspush;

    public int[] _itemM = new int[5];    //0:id, 1:타입(1:소비, 2:장비, 3:기타, 4:퀘스트 템), 2:소지수, 3:소지상한, 4:중복 여부(0:중복 off, 1:중복 on).

    void Awake()
    {
        playIspush = true;
        //기존 AddForce 사용 아이템 드랍.
        _rid2d = GetComponent<Rigidbody2D>();
        //_rid2d.AddForce(new Vector2(0, 300f), ForceMode2D.Impulse);
        _inv = GameObject.Find("CanvasInventory").transform.Find("Inventory").gameObject;
        switch (this.gameObject.name)
        {
            case "Portion00":
            case "Portion00(Clone)":
                this.gameObject.name = "Portion00";
                _itemM[0] = 000;
                _itemM[1] = 1;
                _itemM[3] = 3;
                _itemM[4] = 1;
                break;
            case "Portion01":
            case "Portion01(Clone)":
                this.gameObject.name = "Portion01";
                _itemM[0] = 001;
                _itemM[1] = 1;
                _itemM[3] = 10;
                _itemM[4] = 1;
                break;
            case "Weapon01_00":
            case "Weapon01_00(Clone)":
                this.gameObject.name = "Weapon01_00";
                _itemM[0] = 100;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Weapon01_01":
            case "Weapon01_01(Clone)":
                this.gameObject.name = "Weapon01_01";
                _itemM[0] = 101;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Weapon02_00":
            case "Weapon02_00(Clone)":
                this.gameObject.name = "Weapon02_00";
                _itemM[0] = 200;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Weapon02_01":
            case "Weapon02_01(Clone)":
                this.gameObject.name = "Weapon02_01";
                _itemM[0] = 201;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Clothes00":
            case "Clothes00(Clone)":
                this.gameObject.name = "Clothes00";
                _itemM[0] = 300;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Bow00":
            case "Bow00(Clone)":
                this.gameObject.name = "Bow00";
                _itemM[0] = 400;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Shild00":
            case "Shild00(Clone)":
                this.gameObject.name = "Shild00";
                _itemM[0] = 500;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Ring00":
            case "Ring00(Clone)":
                this.gameObject.name = "Ring00";
                _itemM[0] = 600;
                _itemM[1] = 2;
                _itemM[3] = 16;
                _itemM[4] = 0;
                break;
            case "Shoes00":
            case "Shoes00(Clone)":
                this.gameObject.name = "Shoes00";
                _itemM[0] = 700;
                _itemM[1] = 3;
                _itemM[3] = 2;
                _itemM[4] = 1;
                break;
        }
    }

    private void Start()
    {
        //transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y - 150f, Time.deltaTime * 3f));
        if (playIspush)
        {
            StartCoroutine("JumpDown");
        }
    }

    private void LateUpdate()
    {
        //transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y - 150f, Time.time));
    }

    //아이템 드랍.
    IEnumerator JumpDown()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 velocity = transform.position;
        Vector3 itemPos = transform.position;
        bool isPush = true;

        while (playIspush)
        {
            if (Mathf.Abs(itemPos.y - transform.position.y) >= 60f) //상승하는 y값.
            {
                isPush = false;
                velocity.y -= Time.deltaTime * 200f;    //하락.
                transform.position = velocity;
                yield return null;
            }
            else if(isPush)
            {
                velocity.y += Time.deltaTime * 150f;    //상승.
                transform.position = velocity;
                yield return null;
            }
            else
            {
                velocity.y -= Time.deltaTime * 200f;    //하락.
                transform.position = velocity;
                yield return null;
                //if ((itemPos.y-11f) > transform.position.y)
                //    break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 12)   //땅 충돌 체크.
        {
            if (playIspush)
            {
                _rid2d.bodyType = RigidbodyType2D.Static;
            }
            playIspush = false;
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        //아이템 획득.
        if (other.gameObject.layer == 9 || other.gameObject.layer == 11)
        {
            if (!playIspush)
            {
                _itemM[2] = _inv.transform.parent.gameObject.GetComponent<InventoryScript>()._itemM[_itemM[0]];
                for (int j = 0; j < _inv.transform.GetChild(_itemM[1]).childCount; j++)
                {
                    Transform _item = _inv.transform.GetChild(_itemM[1]).GetComponent<InventoryMenu>().item;
                    //칸 공간 여부.
                    if (_inv.transform.parent.gameObject.GetComponent<InventoryScript>()._slotList[_itemM[1] * 16 - 16 + j] == null && (_itemM[4] == 0 || _itemM[2] == 0))  //슬롯이 비어있음 && (중복 비허용 || 장비 아이템).
                    {
                        if (_itemM[2] < _itemM[3])
                        {
                            GameObject _sloTem = Instantiate(_slotItem);
                            _sloTem.name = this.name;
                            _sloTem.GetComponent<Image>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                            _sloTem.transform.SetParent(_inv.transform.GetChild(_itemM[1]).GetChild(j));
                            _sloTem.transform.localPosition = Vector3.zero;
                            _sloTem.transform.localScale = Vector3.one;
                            _inv.transform.parent.gameObject.GetComponent<InventoryScript>()._slotList[_itemM[1] * 16 - 16 + j] = _sloTem;

                            //개수 계산.
                            _itemM[2] += 1;
                            _inv.transform.parent.gameObject.GetComponent<InventoryScript>()._itemM[_itemM[0]] = _itemM[2];
                            //중복 비허용일 시 개수 표기.
                            if (_itemM[4] == 0)
                                _sloTem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(0, 0, 0, 0);

                            Destroy(this.gameObject);
                            return;
                        }
                    }
                    else if (_inv.transform.parent.gameObject.GetComponent<InventoryScript>()._slotList[_itemM[1] * 16 - 16 + j] != null && _itemM[4] == 1) //슬롯이 비어있지 않음 && 중복 허용.
                    {
                        if (_inv.transform.parent.gameObject.GetComponent<InventoryScript>()._slotList[_itemM[1] * 16 - 16 + j].name == this.name)
                        {
                            if (_itemM[2] < _itemM[3])  //소지수 < 소지상한.
                            {
                                if (_inv.transform.GetChild(_itemM[1]).GetChild(j).childCount != 0)
                                {
                                    GameObject _sloTem = _inv.transform.GetChild(_itemM[1]).GetChild(j).GetChild(0).gameObject;
                                    //개수 계산.
                                    _itemM[2] += 1;
                                    _inv.transform.parent.gameObject.GetComponent<InventoryScript>()._itemM[_itemM[0]] = _itemM[2];

                                    //아이템 개수 표기.
                                    _sloTem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _itemM[2].ToString();
                                    if (_itemM[2] == _itemM[3])
                                        _sloTem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                                    if (GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0).gameObject.name == gameObject.name)
                                    {
                                        GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _itemM[2].ToString();
                                    }

                                    Destroy(this.gameObject);
                                }
                                else
                                {
                                    //인벤토리에서 아이템 들고 있을 때.
                                    _item.SetParent(_inv.transform.GetChild(_itemM[1]).GetComponent<InventoryMenu>().select);
                                }
                            }
                            else
                            {
                                //소지수 >= 소지 상한일 때.
                                break;
                            }
                            return;
                        }
                    }
                }

                //빈 슬롯이 없을 때.
                Debug.Log("소지품이 가득 찾습니다");
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == 12)   //땅 충돌 체크.
        {
            //playIspush = true;
        }
    }

    void Update()
    {
        if (_inv.transform.parent.gameObject.GetComponent<InventoryScript>()._itemM[_itemM[0]] >= _itemM[3])
        {
            //아이템 드롭 오브젝트 플레이어 레이어 충돌 여부.
            if (Physics2D.GetIgnoreLayerCollision(this.gameObject.layer, 9) == false && !playIspush)
            {
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, 9, true);
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, 11, true);
            }
        }
        else
        {
            if (Physics2D.GetIgnoreLayerCollision(this.gameObject.layer, 9) == true)
            {
                //아이템 드롭 오브젝트 플레이어 레이어 충돌 여부.
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, 9, false);
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, 11, false);
            }
        }
    }
}
