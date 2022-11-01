using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private static ItemDrop _Instance = null;
    public List<bool> _tresureboxOpen = new List<bool>();   //보물상자 스위치.

    public GameObject item;
    GameObject[] items; //대입용 변수.
    //아이템 id 목록.
    public GameObject[] items00;    //소비류(0~99).
    public GameObject[] items01;    //무기1류(100~199).
    public GameObject[] items02;    //무기2류(200~299).
    public GameObject[] items03;    //옷류(300~399).
    public GameObject[] items04;    //활류(400~499).
    public GameObject[] items05;    //방패류(500~599).
    public GameObject[] items06;    //반지류(600~699).
    public GameObject[] items07;    //기타류(700~799).
    public GameObject[] items08;    //퀘스트 템류(800~899).

    private void Awake()
    {
        if (_Instance != null)   //싱글톤.
        {
            Destroy(gameObject);
            return;
        }
        _Instance = this;
        DontDestroyOnLoad(this);    //씬 이동 후 오브젝트 보존.

        for (int i = 0; i <= 1; i++)    //i <= 상자 개수(0~End).
        {
            _tresureboxOpen.Add(false);
        }
    }

    //아이템 드롭 함수.
    public void ItemRandom(Vector3 pos, bool _minus1, int []Counts, int _num)   //생성 포지션값, 아무것도 안나올 수도 있는지, 생성 아이템 목록, 아이템 드롭 개수.
    {
        int Count = -1;
        for (int i = 0; i < _num; _num--)
        {
            int count = Random.Range(-1, 6); //Random.Range(0, max);했을 때, 0 ~ (max - 1)의 랜덤 값을 반환.
            if (count == -1)
            {
                if (_minus1)
                    break;
                else
                    count = Random.Range(0, 6);
            }
            if (count == 0)
                Count = Counts[0];
            else if (count == 1)
                Count = Counts[1];
            else if (count == 2)
                Count = Counts[2];
            else if (count == 3)
                Count = Counts[3];
            else if (count == 4)
                Count = Counts[4];
            else if (count == 5)
                Count = Counts[5];
            else if (count == 6)
                Count = Counts[6];
            else if (count == 7)
                Count = Counts[7];
            else if (count == 8)
                Count = Counts[8];

            if (Count == -1)
                break;
            else if (Count < 100)
                items = items00;
            else if (Count < 200)
                items = items01;
            else if (Count < 300)
                items = items02;
            else if (Count < 400)
                items = items03;
            else if (Count < 500)
                items = items04;
            else if (Count < 600)
                items = items05;
            else if (Count < 700)
                items = items06;
            else if (Count < 800)
                items = items07;
            else if (Count < 900)
                items = items08;

            if (Count >= 100)
                item = items[Count % 100];
            else
                item = items[Count];

            GameObject itemCurrent = Instantiate(item);
            itemCurrent.transform.position = new Vector3(pos.x, pos.y, -10f); //pos.y는 일단 너무 바닥이랑 멀어서 일단 줄여놓음.
        }
        //여러 개일 시 or ID값 -1로 중단됬을 때 반복 코드.
        if (Count == -1)
            ItemRandom(pos, _minus1, Counts, _num);
    }
}
