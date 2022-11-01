using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIevent : MonoBehaviour
{
    public GameObject _HP_Bar;
    Image _HP_Bar_Img;

    GameObject _Canvas_Fever;
    public GameObject _Fever_sprite;
    public List<GameObject> _Fever_sprite_List = new List<GameObject>();

    public Sprite _feverSprite;
    public Sprite _feverSprite_half;
    public Sprite _feverSpriteX;

    public GameObject _Canvas;
    public GameObject _Monster;
    public GameObject _Monster_HP;
    Image _Monster_HPImg;
    public List<GameObject> _Monster_HP_List = new List<GameObject>();
    int iCount = 0;

    GameObject _inventory;
    public Sprite _playerIdle;

    public GameObject _Boss;
    public GameObject _BossHP;
    Image _BossHP_Bar_Img;

    void Awake()
    {
        _inventory = GameObject.Find("CanvasInventory");
        _Canvas_Fever = GameObject.Find("Canvas/Fever");

        for (int i = 0; i * 2 != Player_script._player_maxfever; i++)   //피버 UI 생성.
        {
            //HP UI 생성.
            _Fever_sprite_List.Add(Instantiate(_Fever_sprite));
            _Fever_sprite_List[i].transform.SetParent(_Canvas_Fever.transform);
            _Fever_sprite_List[i].transform.localScale = new Vector3(1, 1, 1);
            _Fever_sprite_List[i].transform.position = new Vector3(0, 0, 1);
        }

        _HP_Bar_Img = _HP_Bar.GetComponent<Image>();

        //몬스터 HP바 생성.
        if (_Monster != null)
        {
            iCount = _Monster.transform.childCount;
        }
        for (int i = 0; i != iCount; i++)
        {
            _Monster_HP_List.Add(Instantiate(_Monster_HP));
            _Monster_HP_List[i].transform.SetParent(_Canvas.transform.Find("Monster_HP"));
            _Monster_HP_List[i].transform.localScale = new Vector3(1, 1, 1);
        }

        //보스 HP바 설정.
        if (GameObject.Find("Canvas/BossHP") != null)
        {
            _BossHP = GameObject.Find("Canvas/BossHP");
            _BossHP_Bar_Img = _BossHP.transform.GetChild(0).GetComponent<Image>();
        }
    }

    void Update()
    {
        //HP바 조절.
        _HP_Bar_Img.fillAmount = (float)Player_script._player_hp / Player_script._player_maxhp;
        _HP_Bar.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Player_script._player_hp + "/" + Player_script._player_maxhp;

        //피버 게이지 조절.
        for (int i = 0; i * 2 < Player_script._player_maxfever; i++)   //Player_script._player_hp == Player_script._player_maxhp
        {
            _Fever_sprite_List[i].GetComponent<Image>().sprite = _feverSprite;
        }
        if (Player_script._player_fever < Player_script._player_maxfever)
        {
            if (Player_script._player_fever > 0)
            {
                //X.
                for (int i = Player_script._player_fever + 1; i * 2 < Player_script._player_maxfever * 2; i++)
                    _Fever_sprite_List[i / 2].GetComponent<Image>().sprite = _feverSpriteX;

                //반 칸 여부.
                if (Player_script._player_fever % 2 == 0)
                    _Fever_sprite_List[Player_script._player_fever / 2].GetComponent<Image>().sprite = _feverSpriteX;
                else
                    _Fever_sprite_List[Player_script._player_fever / 2].GetComponent<Image>().sprite = _feverSprite_half;
            }
            else
            {
                //X.
                for (int i = 0; i * 2 < Player_script._player_maxfever; i++)
                    _Fever_sprite_List[i].GetComponent<Image>().sprite = _feverSpriteX;
            }
        }

        //몬스터 HP바 조절.
        for (int i = 0; i != iCount; i++)
        {
            if (_Monster.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                Vector3 hit = Camera.main.WorldToViewportPoint(new Vector3(_Monster.transform.GetChild(i).position.x, _Monster.transform.GetChild(i).position.y + 45f, 1));
                _Monster_HP_List[i].transform.position = Camera.main.ViewportToWorldPoint(hit);
                _Monster_HP_List[i].transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)_Monster.transform.GetChild(i).GetComponent<Monster_script>()._monster_hp / _Monster.transform.GetChild(i).GetComponent<Monster_script>()._monster_maxhp;
            }
            else   //몬스터 hp = 0.
                _Monster_HP_List[i].SetActive(false);
        }
        //보스 HP바 조절.
        if (_BossHP != null)
        {
            switch (_Boss.name)
            {
                case "Boss01":
                    if (Boss01_script._monster_hp > 0)
                    {
                        _BossHP_Bar_Img.fillAmount = (float)Boss01_script._monster_hp / Boss01_script._monster_maxhp;
                    }
                    else
                    {
                        Destroy(_BossHP.transform.GetChild(0).gameObject);
                        Destroy(_BossHP);
                    }
                    break;
            }
        }

        //인벤토리 & 퀘스트 키.
        if (!DialogueManager.pauseOn && !Player_script._isFade && !Player_script._isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.J) && !GameObject.Find("Canvas/Dialogue").GetComponent<Animator>().GetBool("IsOpen"))
            {
                InventoryScript.isActive = !InventoryScript.isActive;
                InvSet();
                _inventory.transform.Find("BackBook01").gameObject.SetActive(InventoryScript.isActive);
                _inventory.transform.Find("Inventory").gameObject.SetActive(InventoryScript.isActive);
                _inventory.transform.Find("Equip").gameObject.SetActive(InventoryScript.isActive);
                _inventory.transform.Find("EquipText").gameObject.SetActive(InventoryScript.isActive);
                _inventory.transform.Find("Page").gameObject.SetActive(InventoryScript.isActive);
                if (!InventoryScript.isActive)
                {
                    _inventory.transform.Find("Chara").gameObject.SetActive(false);
                    _inventory.transform.Find("ShortSlot").gameObject.SetActive(true);
                    _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
                    _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;

                    if (InventoryScript._bookPage == 1)
                        GameObject.Find("CanvasInventory").transform.Find("Page01").gameObject.SetActive(false);
                }
                else
                {
                    _inventory.transform.Find("Chara").gameObject.SetActive(true);
                    _inventory.transform.Find("Chara").GetChild(0).GetComponent<Image>().sprite = _playerIdle;
                    _inventory.transform.Find("ShortSlot").gameObject.SetActive(true);
                    _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-111, -51, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
                    _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 10;
                }

                if (!DialogueManager.pauseOn && !Player_script._isFade)
                {
                    Time.timeScale = Time.timeScale == 1.0f ? 0f : 1.0f;
                }
                _inventory.transform.Find("Inventory").Find("Menu_0").GetComponent<InventoryMenu>().item = null;
                _inventory.transform.Find("Inventory").Find("Menu_1").GetComponent<InventoryMenu>().item = null;
                _inventory.transform.Find("Inventory").Find("Menu_2").GetComponent<InventoryMenu>().item = null;
                _inventory.transform.Find("Inventory").Find("Menu_3").GetComponent<InventoryMenu>().item = null;

                //퀘스트 키일 시.
                if (Input.GetKeyDown(KeyCode.J))
                {
                    if (InventoryScript.isActive)
                    {
                        InventoryScript._bookPage = 1;
                        _inventory.GetComponent<InventoryScript>().ToggleButton(_inventory.transform.Find("Page").GetChild(1).gameObject);
                    }
                    else
                    {
                        InventoryScript._bookPage = 0;
                        _inventory.transform.Find("Page01").gameObject.SetActive(false);
                    }
                }
            }

            //단축키 1.
            if (Input.GetButtonDown("Shortcuts 1"))
            {
                int id = -1;
                int hpup = 0;
                switch (GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0).gameObject.name)
                {
                    case "Portion00":
                        id = 000;
                        hpup = 1;
                        break;
                    case "Portion01":
                        id = 001;
                        hpup = 2;
                        break;
                }
                switch (GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0).gameObject.name)
                {
                    case "Portion00":
                    case "Portion01":
                        GameObject.Find("CanvasInventory").transform.Find("Inventory").gameObject.SetActive(true);
                        for (int j = 0; j < GameObject.Find("CanvasInventory/Inventory/Menu_0").transform.childCount; j++)
                        {
                            if (GameObject.Find("CanvasInventory/Inventory/Menu_0").transform.GetChild(j).childCount != 0)
                                if (GameObject.Find("CanvasInventory/Inventory/Menu_0").transform.GetChild(j).GetChild(0).name == GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(0).GetChild(0).gameObject.name)
                                {
                                    GameObject.Find("CanvasInventory/Inventory/Menu_0").transform.GetChild(j).GetChild(0).GetComponent<ItemScript>().ItemSpend(id, 0, hpup);
                                    GameObject.Find("CanvasInventory/Inventory/Menu_0").GetComponent<InventoryMenu>().item = null;
                                    break;
                                }
                        }
                        if (!InventoryScript.isActive)
                            GameObject.Find("CanvasInventory/Inventory").SetActive(false);
                        break;
                }
            }
            //단축키 2.
            if (Input.GetButtonDown("Shortcuts 2"))
            {
                //switch (GameObject.Find("CanvasInventory/ShortSlot").transform.GetChild(1).GetChild(0).gameObject.name)처럼 1로 바꿔서 단축키 1처럼 구현.
                Debug.Log("버프템 단축키 사용");
            }
        }

        //몬스터 HP바 안보이게.
        if (InventoryScript.isActive || Player_script._isFade)
            for (int i = 0; i != iCount; i++)
            {
                _Monster_HP_List[i].GetComponent<Image>().color = new Color32(50, 0, 0, 0);
                _Monster_HP_List[i].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 0, 0, 0);
            }
        else
            for (int i = 0; i != iCount; i++)
            {
                _Monster_HP_List[i].GetComponent<Image>().color = new Color32(50, 0, 0, 255);
                _Monster_HP_List[i].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 0, 0, 255);
            }
    }

    //인벤토리 단축키 함수.
    void InvSet()
    {
        _inventory.transform.Find("Inventory").gameObject.SetActive(false);
        for (int i = 1; i < _inventory.transform.Find("Inventory").childCount; i++)
        {
            Transform _item = _inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().item;
            if (_item != null)
            {
                _item.SetParent(_inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().select);
                _item.localPosition = Vector3.zero;
                if (_inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().slot != _inventory.transform.Find("ShortSlot").gameObject)
                    _inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().slot = null;
            }
        }
    }

}
