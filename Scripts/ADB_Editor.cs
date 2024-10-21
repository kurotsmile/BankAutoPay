using System.Collections;
using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public enum CONTROL_ADB_TYPE{mouse_click,open_app,send_text}
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
        this.Item_Left("Add Mouse click","Add position x,y click",this.sp_icon_mouse);
        this.Item_Left("Open The App","Open the application with the package name",this.sp_icon_open_app);
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
        item_box.set_act(()=>{
                this.Show_edit_control(-1);
        });
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
                IDictionary MmouseClick=(IDictionary) list_command[i];

                GameObject obj_control=Instantiate(this.app.item_box_prefab);
                obj_control.transform.SetParent(this.app.tr_all_item);
                obj_control.transform.localScale=new Vector3(1f,1f,1f);
                obj_control.transform.localPosition=new Vector3(1f,1f,1f);
                if(i%2==0)
                    obj_control.GetComponent<Image>().color=this.app.color_colum_a;
                else
                    obj_control.GetComponent<Image>().color=this.app.color_colum_b;
                Carrot.Carrot_Box_Item cr_item=obj_control.GetComponent<Carrot.Carrot_Box_Item>();
                cr_item.set_title("Mouse Click");
                cr_item.set_tip("X:"+MmouseClick["x"].ToString()+" , Y:"+MmouseClick["y"].ToString());
                cr_item.check_type();
                cr_item.set_icon_white(this.sp_icon_mouse);
                cr_item.txt_name.color=Color.white;
                cr_item.set_act(()=>{
                    this.app.adb.On_Mouse_Click(MmouseClick["x"].ToString(),MmouseClick["y"].ToString());
                });

                Carrot.Carrot_Box_Btn_Item btn_edit=cr_item.create_item();
                btn_edit.set_icon_color(Color.white);
                btn_edit.set_icon(app.cr.icon_carrot_write);
                btn_edit.set_color(app.cr.color_highlight);
                btn_edit.set_act(()=>{
                    this.Show_edit_control(index);
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

    private void Show_edit_control(int index){
        if(this.box!=null) this.box.close();

        IDictionary data_control=null;
        if(index!=-1){
            data_control=(IDictionary)this.list_command[index];
        }
        else{
            data_control=(IDictionary) Carrot.Json.Deserialize("{}");
            data_control["x"]=0;
            data_control["y"]=0;
            data_control["type"]="mouse_click";
        }
            

        this.box=this.app.cr.Create_Box("Edit Control");
        this.box.set_icon(this.sp_icon_mouse);

        Carrot.Carrot_Box_Item inp_x=this.box.create_item("inp_x");
        inp_x.set_title("Position x");
        inp_x.set_tip("Position x mouse and tap");
        inp_x.set_icon(this.app.cr.icon_carrot_write);
        inp_x.set_type(Carrot.Box_Item_Type.box_value_input);
        inp_x.set_val(data_control["x"].ToString());

        Carrot.Carrot_Box_Item inp_y=this.box.create_item("inp_x");
        inp_y.set_title("Position Y");
        inp_y.set_tip("Position Y mouse and tap");
        inp_y.set_icon(this.app.cr.icon_carrot_write);
        inp_y.set_type(Carrot.Box_Item_Type.box_value_input);
        inp_y.set_val(data_control["y"].ToString());

        Carrot_Box_Btn_Panel btn_Panel=this.box.create_panel_btn();
        Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_cancel");
        btn_done.set_bk_color(this.app.cr.color_highlight);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_icon_white(this.app.cr.icon_carrot_done);
        btn_done.set_act_click(()=>{
            IDictionary data_new=(IDictionary) Carrot.Json.Deserialize("{}");
            data_new["x"]=inp_x.get_val();
            data_new["y"]=inp_y.get_val();
            if(index!=-1)
                this.list_command[index]=data_new;
            else
                this.list_command.Add(data_new);
            this.box.close();
            this.app.cr.play_sound_click();
            this.Update_list_ui();
        });

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
