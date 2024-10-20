using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseClick
{
    public int x;
    public int y;
}


public class ADB_Control : MonoBehaviour
{

    [Header("Main Obj")]
    public App app;
    public List<string> list_command;
    private int index_comand_cur=0;
    private float timer_step=0;
    private bool is_play=false;
    public List<MouseClick> mouseClicks = new List<MouseClick>();
    public GameObject item_control_prefab;

    void ReadFileAndParseData(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                this.app.Clear_contain(this.app.tr_all_item);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ParseLine(line);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("File could not be read: " + e.Message);
        }
    }

    void ParseLine(string line)
    {
        string[] parts = line.Split(':');
        if (parts.Length == 3)
        {
            if (int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
            {
                Debug.Log("Mouse click x:"+x+",y:"+y);
                MouseClick click = new MouseClick { x = x, y = y };
                mouseClicks.Add(click);

                GameObject obj_contron=Instantiate(this.item_control_prefab);
                obj_contron.transform.SetParent(this.app.tr_all_item);
                obj_contron.transform.localScale=new Vector3(1f,1f,1f);
                obj_contron.transform.localPosition=new Vector3(1f,1f,1f);

                Carrot.Carrot_Box_Item cr_item=obj_contron.GetComponent<Carrot.Carrot_Box_Item>();
                cr_item.set_title("Mouse Click");
                cr_item.set_tip("X:"+x+" , Y:"+y);
                cr_item.check_type();
            }
        }
    }
    public void On_Start(string filePath){
        this.ReadFileAndParseData(filePath);
        this.index_comand_cur=0;
        this.is_play=true;
    }

    void Update()
    {
        if(this.is_play){
            this.timer_step+=1f*Time.deltaTime;
            if(this.timer_step>=2f){
                this.timer_step=0;
                this.index_comand_cur++;
                Debug.Log("Done Step");
            }
        }
    }

    public void On_Stop(){
        this.is_play=false;
    }

    public bool get_status(){
        return this.is_play;
    }
}
