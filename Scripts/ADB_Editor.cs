using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class ADB_Editor : MonoBehaviour
{
    public App app;

    public void Show_Editor(){
        this.app.Clear_contain(this.app.tr_all_item);
        this.app.Clear_contain(this.app.tr_all_item_right);
        this.app.cr.play_sound_click();
        this.Load_Menu_Right();
    }

    private void Load_Menu_Right(){
       GameObject obj_item=Instantiate(this.app.item_box_prefab);
       obj_item.transform.SetParent(this.app.tr_all_item_right);
       obj_item.transform.localPosition=new Vector3(1f,1f,1f);
       obj_item.transform.localScale=new Vector3(1f,1f,1f);

       Carrot_Box_Item item_box=obj_item.GetComponent<Carrot_Box_Item>();
       item_box.set_icon_white(this.app.cr.icon_carrot_add);
       item_box.set_title("Add Mouse click");
       item_box.set_tip("Add position x,y click");
       item_box.txt_name.color=Color.white;
       item_box.GetComponent<Image>().color=this.app.color_colum_a;
       item_box.check_type();
       item_box.set_act(()=>{

       });
    }
}
