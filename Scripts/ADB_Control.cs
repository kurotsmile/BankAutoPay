
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ADB_Control : MonoBehaviour
{

    [Header("Main Obj")]
    public App app;

    [Header("UI")]
    public Slider slider_process_length;
    private IList list_command;

    private int index_comand_cur=0;
    private float timer_step=0;
    private bool is_play=false;

    void ReadFileAndParseData(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
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
                                this.list_command.Add(MmouseClick);
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

    public void On_Play(IList list_cmd){
        this.list_command=list_cmd;
        this.slider_process_length.maxValue=list_cmd.Count;
        this.slider_process_length.value=0;
        this.index_comand_cur=0;
        if(this.list_command.Count==0){
            this.app.cr.Show_msg("No commands have been created yet!","ADB Control",Msg_Icon.Alert);
        }
        else
        {
            this.is_play=true;    
        }
    }

    void Update()
    {
        if(this.is_play){
            this.timer_step+=1f*Time.deltaTime;
            if(this.timer_step>=2f){
                this.timer_step=0;

                IDictionary data_item=(IDictionary) this.list_command[this.index_comand_cur];
                this.slider_process_length.value=(this.index_comand_cur+1);
                this.index_comand_cur++;
                if(this.index_comand_cur>=this.list_command.Count){
                    this.On_Stop();
                }
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

    public void On_Mouse_Click(string x,string y){
        this.app.txt_status_app.text="Tap x:"+x+" , y:"+y;
        this.RunCommandWithMemu("adb shell input tap "+x+" "+y);
    }

    public void RunCommandWithMemu(string s_command,UnityAction<string> act_done=null)
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
        if(act_done!=null) act_done(output);
    }

}
