using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryMenu : MonoBehaviour
{
    public Transform item = null;
    public Transform select = null;
    public Transform slot = null;
    public bool isPlay;

    public int i;  //for문에서 메뉴 레이어 분류용.
    public int j;  //for문에서 슬롯 분류용.

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && item != null)    //아이템 칸 위에서 좌클릭 시.
        {
            select = item.parent;
            for (int a = 1; a < transform.parent.childCount; a++)
            {
                if (transform.parent.GetChild(a).gameObject == this.gameObject)
                {
                    i = a;
                }
            }
            for (int a = 0; a < transform.parent.GetChild(i).childCount; a++)
            {
                if (transform.GetChild(a).childCount != 0)
                    if (transform.GetChild(a).GetChild(0).gameObject == item.gameObject)
                        j = a;
            }
            item.SetParent(select.parent);
            isPlay = true;
            SelItemColliders(false);
        }
        if (Input.GetMouseButton(0) && item != null && isPlay)  //좌클릭 상태.
        {
            if (item.parent == select && slot != null)
            {
                item.SetParent(GameObject.Find("CanvasInventory/Inventory").transform.GetChild(i).GetComponent<InventoryMenu>().slot);
                item.SetParent(select.parent);
            }
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            item.position = new Vector3(pos.x,pos.y,item.position.z);
        }
        else if (item != null && isPlay)//if(Input.GetMouseButtonUp(0) && item != null && isPlay)    //좌클릭 놓음.
        {
            isPlay = false;
            SelItemColliders(true);
            if (slot == null || slot == transform.parent)   //슬롯 없음 또는 클릭 중.
            {
                item.SetParent(select); //제자리로.
                //인벤토리 밖에 내보냈을때 버리기 창 활성화.
                if (slot == null)
                    transform.parent.parent.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                if (slot.childCount != 0)
                {
                    slot.GetChild(0).SetParent(select);
                }
                item.SetParent(slot);

                //템 있을 시 템 자리 바꾸기.
                if (select.childCount != 0)
                    transform.parent.parent.GetComponent<InventoryScript>()._slotList[i * 16 - 16 + j] = select.GetChild(0).gameObject;
                else
                    transform.parent.parent.GetComponent<InventoryScript>()._slotList[i * 16 - 16 + j] = null;
                for (int a = 0; a < transform.parent.GetChild(i).childCount; a++)
                {
                    if (transform.GetChild(a).childCount != 0)
                        if (transform.GetChild(a).GetChild(0).gameObject == item.gameObject)
                            j = a;
                }
                transform.parent.parent.GetComponent<InventoryScript>()._slotList[i * 16 - 16 + j] = item.gameObject;
            }

            //위치 재설정.
            if (select.childCount != 0)
            {
                select.GetChild(0).localPosition = Vector3.zero;
            }
            item.localPosition = Vector3.zero;

            //값 초기화.
            item = null;
            select = slot;
            slot = null;
        }
    }
    //아이템 콜라이더 비활성화 함수.
    void SelItemColliders(bool isAll)
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item"))
        {
            item.GetComponent<Collider>().enabled = isAll;
        }
    }
}
