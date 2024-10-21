
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
    private float timer_step_waiting=2;
    private float timer_step_length=2;
    private bool is_play=false;
    private bool is_timer_waiting=false;

    public void On_Start(string filePath){
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
            if(this.timer_step>=this.timer_step_waiting){
                this.timer_step=0;
                if(this.is_timer_waiting){
                    this.timer_step_waiting=this.timer_step_length;
                    this.is_timer_waiting=false;
                }
                IDictionary data_item=(IDictionary) this.list_command[this.index_comand_cur];

                if(data_item["type"].ToString()=="mouse_click") this.On_Mouse_Click(data_item["x"].ToString(),data_item["y"].ToString());
                if(data_item["type"].ToString()=="waiting"){
                    this.timer_step_waiting=int.Parse(data_item["timer"].ToString());
                    this.is_timer_waiting=true;
                }

                if(data_item["type"].ToString()=="send_text") this.On_Send_Text(data_item["text"].ToString());

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

    public void On_Send_Text(string s_text){
        this.app.txt_status_app.text="Send Text:"+s_text;
        this.RunCommandWithMemu("adb shell input text \""+s_text+"\"");
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

     public static string RunADBCommand(string command)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
    
    [ContextMenu("Act_Get_list_Devices")]
    public void Act_Get_list_Devices(){
        this.RunCommandWithMemu("adb devices",s_list=>{
            this.app.cr.Show_msg(s_list);
            Debug.Log(s_list);
        });
    }

    public List<string> ListConnectedDevices()
    {
        string adbDevicesCommand = "adb devices";
        string output = RunADBCommand(adbDevicesCommand);

        Debug.Log(output);

        string[] lines = output.Split('\n');

        List<string> deviceList = new List<string>();
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (!string.IsNullOrEmpty(line) && line.Contains("device"))
            {
                string[] parts = line.Split('\t');
                if (parts.Length > 0)
                {
                    deviceList.Add(parts[0]);
                }
            }
        }
        return deviceList;
    }

}
