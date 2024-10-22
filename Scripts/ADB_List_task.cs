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
            this.app.Clear_contain(this.app.tr_all_item);
            string s_path=paths[0];
            string fileContent = FileBrowserHelpers.ReadTextFromFile(s_path);
            string[] lines = fileContent.Split('\n');
            int count_line=0;
            foreach (string line in lines)
            {
                Carrot_Box_Item box_item=this.app.Add_item_main();
                box_item.set_title("App "+(count_line+1));
                box_item.txt_name.color=Color.white;
                box_item.set_tip(line);
                box_item.set_icon_white(this.app.cr.icon_carrot_app);
                if(count_line%2==0) box_item.GetComponent<Image>().color=this.app.color_colum_a;
                count_line++;
            }
        });
    }
}
