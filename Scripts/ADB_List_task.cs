using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADB_List_task : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;

    public void On_Load(){
        this.panel_btn.SetActive(false);
    }

    public void On_Show(){
        this.panel_btn.SetActive(true);
    }

    public void Close_task_list(){
        this.panel_btn.SetActive(false);
    }

    public void Open_file_tastk_app(){
        this.app.cr.play_sound_click();
        this.app.file.Open_file(paths=>{
            string s_path=paths[0];
        });
    }
}
