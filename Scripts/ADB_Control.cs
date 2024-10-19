using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADB_Control : MonoBehaviour
{

    public List<string> list_command;
    private int index_comand_cur=0;
    private float timer_step=0;
    private bool is_play=false;

    public void On_Start(){
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
}
