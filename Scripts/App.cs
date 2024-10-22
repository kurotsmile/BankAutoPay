using System;
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
    public Image img_icon_btn_start;

    [Header("Bank")]
    public Bank_Item[] bank_items;

    [Header("Asset")]
    public Sprite sp_icon_start;
    public Sprite sp_icon_stop;
    private int index_sel_bank;

    void Start()
    {
        this.cr.Load_Carrot();
        this.index_sel_bank=PlayerPrefs.GetInt("index_sel_bank",0);
        this.Update_ui_list_bank();
        if(this.is_load_banck) this.bankcl.On_Load();
        this.adb_tasks.On_Load();
    }

    public void Quit_App()
    {
        this.cr.play_sound_click();
        this.bankcl.OnDisable();
        this.cr.delay_function(2f,()=>{
            Application.Quit();
        });
    }

    public void Clear_contain(Transform tr)
    {
        foreach(Transform child in tr)
        {
            Destroy(child.gameObject);
        }
    }

    public void Btn_start_Memu(){
        this.adb.is_memu=true;
        this.txt_status_app.text="Memu emulator launched";
        this.adb.RunCommandWithMemu("start");
    }

    private void Update_ui_list_bank(){
        for(int i=0;i<this.bank_items.Length;i++) this.bank_items[i].img_bk.color=this.color_nomal;
        this.bank_items[this.index_sel_bank].img_bk.color=this.color_sel;
    }

    public void Select_bank(int index){
        PlayerPrefs.SetInt("index_sel_bank",index);
        this.index_sel_bank=index;
        this.Update_ui_list_bank();
        this.cr.play_sound_click();
    }

    public void Btn_start_auto(){
        if(this.adb.get_status()){
            this.adb.is_memu=false;
            this.adb.On_Stop();
            this.txt_btn_start.text="Start";
            this.img_icon_btn_start.sprite=this.sp_icon_start;
        }else{
            this.adb.is_memu=true;
            this.adb.On_Start(this.bank_items[this.index_sel_bank].name_file_macro);
            this.txt_btn_start.text="Stop";
            this.img_icon_btn_start.sprite=this.sp_icon_stop;
        }
        this.cr.play_sound_click();
    }

    public void Btn_stop_memu(){
        this.adb.is_memu=false;
        this.adb.RunCommandWithMemu("stop");
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
}
