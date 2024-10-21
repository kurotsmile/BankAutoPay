using System.Collections;
using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public enum CONTROL_ADB_TYPE{mouse_click,open_app,send_text,waiting}
public class ADB_Editor : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;
    public Image img_icon_play;
    public Text txt_play;

    [Header("Asset")]
    public Sprite sp_icon_mouse;
    public Sprite sp_icon_open_app;
    public Sprite sp_icon_waiting;
    public Sprite sp_icon_send_text;

    private IList list_command;

    private Carrot_Box box=null;

    public void Show_Editor(){
        this.list_command=(IList) Carrot.Json.Deserialize("[]");
        this.app.Clear_contain(this.app.tr_all_item);
        this.app.Clear_contain(this.app.tr_all_item_right);
        this.app.cr.play_sound_click();
        this.Load_Menu_Right();
        this.panel_btn.SetActive(true);
    }

    private void Load_Menu_Right(){
        this.Item_Left("Add Mouse click","Add position x,y click",this.sp_icon_mouse).set_act(()=>{
            this.Show_edit_control(-1,CONTROL_ADB_TYPE.mouse_click);
        });

        this.Item_Left("Open The App","Open the application with the package name",this.sp_icon_open_app).set_act(()=>{
            this.Show_edit_control(-1,CONTROL_ADB_TYPE.open_app);
        });

        this.Item_Left("Waiting","waiting to continue other tasks",this.sp_icon_waiting).set_act(()=>{
            this.Show_edit_control(-1,CONTROL_ADB_TYPE.waiting);
        });

        this.Item_Left("Send Text","Send Text to Device as Clipboard",this.sp_icon_send_text).set_act(()=>{
            this.Show_edit_control(-1,CONTROL_ADB_TYPE.send_text);
        });

        this.Item_Left("All Device","Get list Devices",this.sp_icon_send_text).set_act(()=>{
            this.Show_edit_control(-1,CONTROL_ADB_TYPE.send_text);
        });
    }

    private Carrot_Box_Item Item_Left(string s_title,string s_tip,Sprite s_icon){
        GameObject obj_item=Instantiate(this.app.item_box_prefab);
        obj_item.transform.SetParent(this.app.tr_all_item_right);
        obj_item.transform.localPosition=new Vector3(1f,1f,1f);
        obj_item.transform.localScale=new Vector3(1f,1f,1f);

        Carrot_Box_Item item_box=obj_item.GetComponent<Carrot_Box_Item>();
        item_box.set_icon_white(s_icon);
        item_box.set_title(s_title);
        item_box.set_tip(s_tip);
        item_box.txt_name.color=Color.white;
        item_box.GetComponent<Image>().color=this.app.color_colum_a;
        item_box.check_type();
        return item_box;
    }

    public void Save_data_json_control(){
        this.app.file.Save_file(paths=>{
            string s_path=paths[0];
            FileBrowserHelpers.WriteTextToFile(s_path,Carrot.Json.Serialize(this.list_command));
        });
    }

    public void Update_list_ui(){
        this.app.Clear_contain(this.app.tr_all_item);
        for(int i=0;i<this.list_command.Count;i++){
                var index=i;
                IDictionary control_data=(IDictionary) list_command[i];

                GameObject obj_control=Instantiate(this.app.item_box_prefab);
                obj_control.transform.SetParent(this.app.tr_all_item);
                obj_control.transform.localScale=new Vector3(1f,1f,1f);
                obj_control.transform.localPosition=new Vector3(1f,1f,1f);
                if(i%2==0)
                    obj_control.GetComponent<Image>().color=this.app.color_colum_a;
                else
                    obj_control.GetComponent<Image>().color=this.app.color_colum_b;
                Carrot.Carrot_Box_Item cr_item=obj_control.GetComponent<Carrot.Carrot_Box_Item>();

                if(control_data["type"].ToString()=="mouse_click"){
                    cr_item.set_icon_white(this.sp_icon_mouse);
                    cr_item.set_title("Mouse Click");
                    cr_item.set_tip("X:"+control_data["x"].ToString()+" , Y:"+control_data["y"].ToString());
                }

                if(control_data["type"].ToString()=="open_app"){
                    cr_item.set_icon_white(this.sp_icon_open_app);
                    cr_item.set_title("Open App");
                    cr_item.set_tip("App id:"+control_data["id_app"].ToString());
                }

                if(control_data["type"].ToString()=="waiting"){
                    cr_item.set_icon_white(this.sp_icon_waiting);
                    cr_item.set_title("Waiting");
                    cr_item.set_tip("Timer:"+control_data["timer"].ToString());
                }

                if(control_data["type"].ToString()=="send_text"){
                    cr_item.set_icon_white(this.sp_icon_send_text);
                    cr_item.set_title("Send Text");
                    cr_item.set_tip("Text:"+control_data["text"].ToString());
                }

                cr_item.check_type();
                cr_item.txt_name.color=Color.white;
                cr_item.set_act(()=>{
                    if(control_data["type"].ToString()=="mouse_click") this.app.adb.On_Mouse_Click(control_data["x"].ToString(),control_data["y"].ToString());
                    if(control_data["type"].ToString()=="send_text") this.app.adb.On_Send_Text(control_data["text"].ToString());
                });

                Carrot.Carrot_Box_Btn_Item btn_edit=cr_item.create_item();
                btn_edit.set_icon_color(Color.white);
                btn_edit.set_icon(app.cr.icon_carrot_write);
                btn_edit.set_color(app.cr.color_highlight);
                btn_edit.set_act(()=>{
                    if(control_data["type"].ToString()=="mouse_click") this.Show_edit_control(index,CONTROL_ADB_TYPE.mouse_click);
                    if(control_data["type"].ToString()=="open_app") this.Show_edit_control(index,CONTROL_ADB_TYPE.open_app);
                    if(control_data["type"].ToString()=="waiting") this.Show_edit_control(index,CONTROL_ADB_TYPE.waiting);
                    if(control_data["type"].ToString()=="send_text") this.Show_edit_control(index,CONTROL_ADB_TYPE.send_text);
                });

                Carrot_Box_Btn_Item btn_del=cr_item.create_item();
                btn_del.set_icon_color(Color.white);
                btn_del.set_icon(app.cr.sp_icon_del_data);
                btn_del.set_color(app.cr.color_highlight);
                btn_del.set_act(()=>{
                    this.list_command.RemoveAt(index);
                    this.Update_list_ui();
                });
        }
    }

    private void Show_edit_control(int index,CONTROL_ADB_TYPE type){
        if(this.box!=null) this.box.close();

        IDictionary data_control=null;
        if(index!=-1){
            data_control=(IDictionary)this.list_command[index];
        }
        else{
            data_control=(IDictionary) Carrot.Json.Deserialize("{}");
            data_control["x"]=0;
            data_control["y"]=0;
            data_control["type"]=type.ToString();
        }

        this.box=this.app.cr.Create_Box("box_control_edit");
        Carrot_Box_Btn_Panel btn_Panel=null;
        if(type==CONTROL_ADB_TYPE.mouse_click){
            this.box.set_icon(this.sp_icon_mouse);
            if(index==-1)
                this.box.set_title("Add Mouse Click");
            else
                this.box.set_title("Update Mouse Click");
            Carrot.Carrot_Box_Item inp_x=this.box.create_item("inp_x");
            inp_x.set_title("Position x");
            inp_x.set_tip("Position x mouse and tap");
            inp_x.set_icon(this.app.cr.icon_carrot_write);
            inp_x.set_type(Carrot.Box_Item_Type.box_number_input);
            inp_x.set_val(data_control["x"].ToString());

            Carrot.Carrot_Box_Item inp_y=this.box.create_item("inp_x");
            inp_y.set_title("Position Y");
            inp_y.set_tip("Position Y mouse and tap");
            inp_y.set_icon(this.app.cr.icon_carrot_write);
            inp_y.set_type(Carrot.Box_Item_Type.box_number_input);
            inp_y.set_val(data_control["y"].ToString());

            btn_Panel=this.box.create_panel_btn();

            Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
            btn_done.set_bk_color(this.app.cr.color_highlight);
            btn_done.set_label("Done");
            btn_done.set_label_color(Color.white);
            btn_done.set_icon_white(this.app.cr.icon_carrot_done);
            btn_done.set_act_click(()=>{
                data_control["x"]=inp_x.get_val();
                data_control["y"]=inp_y.get_val();
                data_control["type"]=type.ToString();
                if(index!=-1)
                    this.list_command[index]=data_control;
                else
                    this.list_command.Add(data_control);
                this.box.close();
                this.app.cr.play_sound_click();
                this.Update_list_ui();
            });
        }

        if(type==CONTROL_ADB_TYPE.open_app){
            this.box.set_icon(this.sp_icon_open_app);
            if(index==-1)
                this.box.set_title("Add Open App");
            else
                this.box.set_title("Update Open App");
            Carrot.Carrot_Box_Item inp_id_app=this.box.create_item("id_app");
            inp_id_app.set_title("Application Package Name (Application ID)");
            inp_id_app.set_tip("Enter the application name and main startup function");
            inp_id_app.set_icon(this.app.cr.icon_carrot_write);
            inp_id_app.set_type(Carrot.Box_Item_Type.box_value_input);
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());

            btn_Panel=this.box.create_panel_btn();

            Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
            btn_done.set_bk_color(this.app.cr.color_highlight);
            btn_done.set_label("Done");
            btn_done.set_label_color(Color.white);
            btn_done.set_icon_white(this.app.cr.icon_carrot_done);
            btn_done.set_act_click(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(index!=-1)
                    this.list_command[index]=data_control;
                else
                    this.list_command.Add(data_control);
                this.box.close();
                this.app.cr.play_sound_click();
                this.Update_list_ui();
            });
        }

        if(type==CONTROL_ADB_TYPE.waiting){
            this.box.set_icon(this.sp_icon_waiting);
            if(index==-1)
                this.box.set_title("Add Waiting");
            else
                this.box.set_title("Update Waiting");
            Carrot.Carrot_Box_Item inp_timer=this.box.create_item("timer");
            inp_timer.set_title("Waiting time");
            inp_timer.set_tip("Enter the number of seconds to wait");
            inp_timer.set_icon(this.app.cr.icon_carrot_write);
            inp_timer.set_type(Carrot.Box_Item_Type.box_number_input);
            if(data_control["timer"]!=null) inp_timer.set_val(data_control["timer"].ToString());

            btn_Panel=this.box.create_panel_btn();

            Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
            btn_done.set_bk_color(this.app.cr.color_highlight);
            btn_done.set_label("Done");
            btn_done.set_label_color(Color.white);
            btn_done.set_icon_white(this.app.cr.icon_carrot_done);
            btn_done.set_act_click(()=>{
                data_control["timer"]=inp_timer.get_val();
                if(index!=-1)
                    this.list_command[index]=data_control;
                else
                    this.list_command.Add(data_control);
                this.box.close();
                this.app.cr.play_sound_click();
                this.Update_list_ui();
            });
        }

        if(type==CONTROL_ADB_TYPE.send_text){
            this.box.set_icon(this.sp_icon_send_text);
            if(index==-1)
                this.box.set_title("Add Send Text");
            else
                this.box.set_title("Update Send Text");
            Carrot.Carrot_Box_Item inp_text=this.box.create_item("inp_text");
            inp_text.set_title("Text");
            inp_text.set_tip("Enter the text you want to insert into the device");
            inp_text.set_icon(this.app.cr.icon_carrot_write);
            inp_text.set_type(Carrot.Box_Item_Type.box_value_input);
            if(data_control["text"]!=null) inp_text.set_val(data_control["text"].ToString());

            btn_Panel=this.box.create_panel_btn();

            Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
            btn_done.set_bk_color(this.app.cr.color_highlight);
            btn_done.set_label("Done");
            btn_done.set_label_color(Color.white);
            btn_done.set_icon_white(this.app.cr.icon_carrot_done);
            btn_done.set_act_click(()=>{
                data_control["text"]=inp_text.get_val();
                if(index!=-1)
                    this.list_command[index]=data_control;
                else
                    this.list_command.Add(data_control);
                this.box.close();
                this.app.cr.play_sound_click();
                this.Update_list_ui();
            });
        }

        Carrot_Button_Item btn_cancel=btn_Panel.create_btn("btn_cancel");
        btn_cancel.set_bk_color(this.app.cr.color_highlight);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
        btn_cancel.set_act_click(()=>{
            this.box.close();
            this.app.cr.play_sound_click();
        });
    }

    public void Close_Editor(){
        this.panel_btn.SetActive(false);
        this.app.cr.play_sound_click();
    }

    public void On_Open(){
        this.app.file.Open_file(paths=>{
            string s_path=paths[0];
            this.list_command= (IList) Carrot.Json.Deserialize(FileBrowserHelpers.ReadTextFromFile(s_path));
            this.Update_list_ui();
        });
    }

    public void Play_all_comand(){
        if(this.app.adb.get_status()){
            this.txt_play.text="Stop";
            this.img_icon_play.sprite=this.app.sp_icon_stop;
            this.app.adb.On_Stop();
        }else{
            this.txt_play.text="Play";
            this.img_icon_play.sprite=this.app.sp_icon_start;
            this.app.adb.On_Play(this.list_command);
        }
    }
}
