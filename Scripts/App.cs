﻿using System;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Main Object")]
    public bool is_load_banck;
    public GameObject prefab_item_bill;
    public BankCL bankcl;
    public Color32 color_nomal;
    public Color32 color_sel;
    public Color32 color_colum_a;
    public Color32 color_colum_b;
    public ADB_Control adb;
    public ADB_Editor adb_editor;
    public ADB_List_task adb_tasks;
    public Carrot.Carrot cr;
    public Carrot_File file;
    public GameObject item_box_prefab;

    [Header("UI")]
    public Transform tr_all_item;
    public Transform tr_all_item_right;
    public Text txt_status_app;
    public Text txt_btn_start;
    public Text txt_btn_memu;
    public Image img_icon_btn_memu;
    public Image img_icon_btn_start;

    [Header("Asset")]
    public Sprite sp_icon_start;
    public Sprite sp_icon_stop;
    public Sprite sp_icon_start_simulador;
    public Sprite sp_icon_stop_simulador;
    private bool is_play_simulador=false;

    void Start()
    {
        this.cr.Load_Carrot();
        this.adb_tasks.On_Load();
        this.adb_editor.On_Load();

        if(this.is_load_banck) 
            this.bankcl.On_Load();
        else
            this.adb_editor.Load_Method_Menu_Right();

    }

    public void Quit_App()
    {
        this.cr.play_sound_click();
        this.cr.delay_function(2f,()=>{
            Application.Quit();
        });
    }

    public void Btn_start_or_stop_Memu(){
        this.cr.play_sound_click();
        if(this.is_play_simulador){
            this.txt_btn_memu.text="Play";
            this.img_icon_btn_memu.sprite=this.sp_icon_start_simulador;
            this.adb.RunCommandWithMemu("stop");
            this.adb.is_memu=false;
            this.is_play_simulador=false;
        }else{
            this.adb.is_memu=true;
            this.txt_btn_memu.text="Stop";
            this.img_icon_btn_memu.sprite=this.sp_icon_stop_simulador;
            this.adb.RunCommandWithMemu("start");
            this.is_play_simulador=true;
        }
    }

    public void Btn_start_auto(){
        if(this.adb.get_status()){
            this.adb.is_memu=false;
            this.adb.On_Stop();
            this.txt_btn_start.text="Start";
            this.img_icon_btn_start.sprite=this.sp_icon_start;
        }else{
            this.adb.is_memu=true;
            this.txt_btn_start.text="Stop";
            this.img_icon_btn_start.sprite=this.sp_icon_stop;
        }
        this.cr.play_sound_click();
    }

    public void Btn_save_data(){
        this.file.Set_filter(Carrot_File_Data.JsonData);
        this.adb_editor.Save_data_json_control();
        this.cr.play_sound_click();
    }

    public void Btn_show_list_app(){
        this.adb.Get_list_app();
    }

    public Carrot_Box_Item Add_item_main(){
        GameObject obj_item=Instantiate(this.item_box_prefab);
        obj_item.transform.SetParent(this.tr_all_item);
        obj_item.transform.localScale=new Vector3(1f,1f,1f);
        obj_item.transform.localPosition=new Vector3(1f,1f,1f);
        Carrot_Box_Item box_item=obj_item.GetComponent<Carrot_Box_Item>();
        obj_item.GetComponent<Image>().color=this.color_colum_b;
        box_item.check_type();
        return box_item;
    }

    public Carrot_Box_Item Add_Item_Right(string s_title,string s_tip,Sprite s_icon){
        GameObject obj_item=Instantiate(this.item_box_prefab);
        obj_item.transform.SetParent(this.tr_all_item_right);
        obj_item.transform.localPosition=new Vector3(1f,1f,1f);
        obj_item.transform.localScale=new Vector3(1f,1f,1f);

        Carrot_Box_Item item_box=obj_item.GetComponent<Carrot_Box_Item>();
        item_box.set_icon_white(s_icon);
        item_box.set_title(s_title);
        item_box.set_tip(s_tip);
        item_box.txt_name.color=Color.white;
        item_box.GetComponent<Image>().color=this.color_colum_a;
        item_box.check_type();
        return item_box;
    }
}
