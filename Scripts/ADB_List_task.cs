using System.Collections;
using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class ADB_List_task : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;
    public Image img_btn_play;
    public Text txt_btn_play;
    private List<string> list_task;
    private int index_cur_task=0;
    private bool is_play=false;

    public void On_Load(){
        this.panel_btn.SetActive(false);
    }

    public void On_Show(){
        this.panel_btn.SetActive(true);
        this.Update_ui_btn_play();
    }

    public void Close_task_list(){
        this.panel_btn.SetActive(false);
        this.On_Stop();
    }

    public void Open_file_tastk_app(){
        this.app.cr.play_sound_click();
        this.app.file.Set_filter(Carrot_File_Data.TextDocument);
        this.index_cur_task=0;
        this.app.file.Open_file(paths=>{
            this.app.Clear_contain(this.app.tr_all_item);
            string s_path=paths[0];
            string fileContent = FileBrowserHelpers.ReadTextFromFile(s_path);
            string[] lines = fileContent.Split('\n');
            int count_line=0;
            this.list_task=new List<string>();
            foreach (string line in lines)
            {
                var index=count_line;
                this.list_task.Add(line);
                Carrot_Box_Item box_item=this.app.Add_item_main();
                box_item.set_title("App "+count_line);
                box_item.txt_name.color=Color.white;
                box_item.set_tip(line);
                box_item.set_icon_white(this.app.cr.icon_carrot_app);
                box_item.set_act(()=>{
                    this.index_cur_task=index;
                    this.app.txt_status_app.text="Select app index:"+index;
                });
                if(count_line%2==0) box_item.GetComponent<Image>().color=this.app.color_colum_a;
                count_line++;
            }
        });
    }

    public void On_Play(){
        if(this.is_play){
            this.is_play=false;
            this.app.adb.On_Stop();
        }else{
            this.is_play=true;
            this.Play_task_by_index(this.index_cur_task);
        }
        this.Update_ui_btn_play();
    }

    public void On_Stop(){
        this.is_play=false;
        this.app.adb.On_Stop();
        this.Update_ui_btn_play();
    }

    private void On_Next_task(){
        this.index_cur_task++;
        if(this.index_cur_task<this.list_task.Count){
            this.Play_task_by_index(index_cur_task);
        }else{
            this.index_cur_task=0;
            this.app.cr.Show_msg("Done all Task!","List Task",Msg_Icon.Success);
            this.On_Stop();
        }
    }

    private void Play_task_by_index(int index){
        Debug.Log("Play task : "+this.list_task[index]);
        this.app.adb.On_Open_App(this.list_task[index]);
        this.Update_list_ui();
        this.app.txt_status_app.text="Play task:"+index+" "+this.list_task[index];
        this.app.adb.On_Play(this.app.adb_editor.Get_list_Command(),()=>{
            this.app.adb.On_Stop_App(this.list_task[index]);
            this.On_Next_task();
        });
    }

    private void Update_list_ui(){
        if(this.list_task.Count>0){
            Carrot_Box_Item item_box_cur=this.app.tr_all_item.GetChild(this.index_cur_task).GetComponent<Carrot_Box_Item>();
            item_box_cur.img_icon.color=Color.yellow;
            item_box_cur.txt_name.color=Color.yellow;
        }
    }

    private void Update_ui_btn_play(){
        if(this.is_play){
            this.img_btn_play.sprite=this.app.sp_icon_stop;
            this.txt_btn_play.text="Stop";
        }else{
            this.img_btn_play.sprite=this.app.sp_icon_start;
            this.txt_btn_play.text="Start";
        }
    }

    public void Save_File_List_App(){
        this.app.file.Save_file(pasths=>{
            string s_path=pasths[0];
            this.SaveListToFile(s_path);
        });
    }

    private void SaveListToFile(string filePath)
    {
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                foreach (string line in this.list_task)
                {
                    writer.WriteLine(line);
                }
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Error save file: " + e.Message);
        }
    }
}
