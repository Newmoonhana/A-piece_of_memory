using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
public class Monster_Item
{
    //xml파일에 들어가있는 정보의 종류
    public string c;
    public int Id;
    public string t;

    public static string item2;
}

public class x : MonoBehaviour
{    // Resources/XML/TestItem.XML 파일.
    string xmlFileName = "Test_Data";

    public static List<Monster_Item> i = new List<Monster_Item>(); //파싱한 정보를 담을 리스트

    // Use this for initialization
    void Start()
    {
       i = Read(xmlFileName); //확장자를 뺸 이름
    }

    public static List<Monster_Item> Read(string _fileName)
    {
        TextAsset textxml = (TextAsset)Resources.Load("XML/" + _fileName);
        XmlDocument Document = new XmlDocument();
        Debug.Log(textxml.text);
        Document.LoadXml(textxml.text);

        XmlElement ItemListElement = Document["Monster"];

        List<Monster_Item> ItemList = new List<Monster_Item>();

        foreach (XmlElement ItemElement in ItemListElement.ChildNodes)
        {
            Monster_Item Item = new Monster_Item();
            Item.Id = System.Convert.ToInt32(ItemElement.GetAttribute("Id"));
            Item.c = ItemElement.GetAttribute("Character");
            Item.t = ItemElement.GetAttribute("Text");
            ItemList.Add(Item);
        }
        return ItemList;
        //Debug.Log(itemList[0].Rank);
    }
}