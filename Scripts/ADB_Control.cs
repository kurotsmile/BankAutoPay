
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class ADB_Control : MonoBehaviour
{

    [Header("Main Obj")]
    public App app;
    public List<IDictionary> list_command;
    private int index_comand_cur=0;
    private float timer_step=0;
    private bool is_play=false;
    public GameObject item_control_prefab;

    [Header("Asset")]
    public Sprite sp_icon_mouse;

    void ReadFileAndParseData(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                this.app.Clear_contain(this.app.tr_all_item);
                string line;
                int count_col=0;
                this.list_command=new List<IDictionary>();
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                        if (parts.Length == 3)
                        {
                            if (int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                            {
                                Debug.Log("Mouse click x:"+x+",y:"+y);
                                IDictionary MmouseClick=(IDictionary) Carrot.Json.Deserialize("{}");
                                MmouseClick["x"]=x;
                                MmouseClick["y"]=y;

                                GameObject obj_control=Instantiate(this.item_control_prefab);
                                obj_control.transform.SetParent(this.app.tr_all_item);
                                obj_control.transform.localScale=new Vector3(1f,1f,1f);
                                obj_control.transform.localPosition=new Vector3(1f,1f,1f);
                                if(count_col%2==0)
                                    obj_control.GetComponent<Image>().color=this.app.color_colum_a;
                                else
                                    obj_control.GetComponent<Image>().color=this.app.color_colum_b;
                                Carrot.Carrot_Box_Item cr_item=obj_control.GetComponent<Carrot.Carrot_Box_Item>();
                                cr_item.set_title("Mouse Click");
                                cr_item.set_tip("X:"+x+" , Y:"+y);
                                cr_item.check_type();
                                cr_item.set_icon_white(this.sp_icon_mouse);
                                cr_item.txt_name.color=Color.white;
                                cr_item.set_act(()=>{
                                    this.On_Mouse_Click(x,y);
                                });

                                Carrot.Carrot_Box_Btn_Item btn_edit=cr_item.create_item();
                                btn_edit.set_icon_color(Color.white);
                                btn_edit.set_icon(app.cr.icon_carrot_write);
                                btn_edit.set_color(app.cr.color_highlight);
                                btn_edit.set_act(()=>{
                                    
                                });
                                count_col++;
                            }
                        }
                }
            }
        }
        catch (System.Exception e)
        {
            this.app.cr.Show_msg("File could not be read: " + e.Message,"ADB Control",Carrot.Msg_Icon.Alert);
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

    private void On_Mouse_Click(int x,int y){
        this.app.txt_status_app.text="Tap x:"+x+" , y:"+y;
        this.RunCommandWithMemu("adb shell input tap "+x+" "+y);
    }

    public void RunCommandWithMemu(string s_command)
    {
        string command = "/C J:\\Microvirt\\MEmu\\MEmuc.exe -i 0 " + s_command;
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        this.app.txt_status_app.text=output;
    }
}
