using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Main Object")]
    public GameObject prefab_item_bill;
    public Color32 color_nomal;
    public Color32 color_sel;
    public ADB_Control adb;
    public Carrot.Carrot cr;

    [Header("UI")]
    public Transform tr_all_item;
    public Text txt_status_app;
    public Text txt_btn_start;
    public Image img_icon_btn_start;

    [Header("Bank")]
    public Bank_Item[] bank_items;

    [Header("Asset")]
    public Sprite sp_icon_start;
    public Sprite sp_icon_stop;
    private DatabaseReference databaseRef;
    private FirebaseApp customApp;
    private int index_sel_bank;

    void Start()
    {
        this.cr.Load_Carrot();
        this.index_sel_bank=PlayerPrefs.GetInt("index_sel_bank",0);
        this.Update_ui_list_bank();
    }

    public void Load_config_app(){
        AppOptions options = new AppOptions
        {
            ApiKey = "AIzaSyBKQ51navOWhgLHY1flH7eK4hPuj9knOa0", 
            AppId = "14228562704",
            ProjectId = "clbank",
            DatabaseUrl = new Uri("https://clbank-default-rtdb.asia-southeast1.firebasedatabase.app"),
            StorageBucket = "clbank.appspot.com"
        };

        if(customApp==null){
            this.txt_status_app.text="Start software initialization and server connection...";
            customApp = FirebaseApp.Create(options, "customApp_bank");
            databaseRef = FirebaseDatabase.GetInstance(customApp).RootReference;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                Firebase.DependencyStatus status = task.Result;
                if (status == Firebase.DependencyStatus.Available)
                {
                    Debug.Log("Firebase is ready with custom config.");
                    ReadDataFromFirebase();
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
                }
            });
        }
        else{
            Debug.Log("App da duoc tao!");
        } 
    }

    void ReadDataFromFirebase()
    {
        DatabaseReference usersRef = databaseRef.Child("lich_su_nap_rut");
        usersRef.ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Lỗi đọc dữ liệu: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists)
        {
            this.Clear_contain(this.tr_all_item);
            foreach (var dateSnapshot in args.Snapshot.Children)
            {
                string dateKey = dateSnapshot.Key; 
 
                foreach (var moneySnapshot in dateSnapshot.Children)
                {
                    string moneyKey = moneySnapshot.Key;
                    string moneyValue = moneySnapshot.Value.ToString();

                    GameObject obj_bill = Instantiate(this.prefab_item_bill);
                    obj_bill.transform.SetParent(this.tr_all_item);
                    obj_bill.transform.localPosition = Vector3.zero;
                    obj_bill.transform.localScale = new Vector3(1f, 1f, 1f);
                    Bill_Item bill = obj_bill.GetComponent<Bill_Item>();

                    if (moneySnapshot.HasChild("username")) bill.txt_name.text = moneySnapshot.Child("username").Value.ToString();
                    if (moneySnapshot.HasChild("money")) bill.txt_tip.text = moneySnapshot.Child("money").Value.ToString();
                    if (moneySnapshot.HasChild("date")) bill.txt_date.text = moneySnapshot.Child("date").Value.ToString();

                    bill.Set_Act_Click(() =>
                    {
                        this.RunCommandWithMemu("start");
                    });
                }
            }
        }
        else
        {
            Debug.Log("Không có dữ liệu.");
        }
    }

    void OnDisable() {
        if(databaseRef!=null) databaseRef.ValueChanged -= HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GoOffline();
    }

    public void Quit_App()
    {
        this.cr.play_sound_click();
        this.OnDisable();
        this.cr.delay_function(2f,()=>{
            Application.Quit();
        });
    }

    public void Clear_contain(Transform tr)
    {
        foreach(Transform child in tr)
        {
            Destroy(child.gameObject);
        }
    }

    public void Btn_start_Memu(){
        this.txt_status_app.text="Memu emulator launched";
        this.RunCommandWithMemu("start");
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
        this.txt_status_app.text=output;
    }

    private void Update_ui_list_bank(){
        for(int i=0;i<this.bank_items.Length;i++) this.bank_items[i].img_bk.color=this.color_nomal;
        this.bank_items[this.index_sel_bank].img_bk.color=this.color_sel;
    }

    public void Select_bank(int index){
        PlayerPrefs.SetInt("index_sel_bank",index);
        this.index_sel_bank=index;
        this.Update_ui_list_bank();
        this.cr.play_sound_click();
    }

    public void QueryList()
    {
        DatabaseReference usersRef = databaseRef.Child("lich_su_nap_rut");
        Query ageQuery = usersRef.OrderByChild("status").EqualTo("done");

        ageQuery.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi truy vấn: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        Debug.Log("User ID: " + childSnapshot.Key);
                        Debug.Log("User Data: " + childSnapshot.GetRawJsonValue());
                    }
                }
            }
        });
    }

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            FirebaseDatabase.DefaultInstance.GoOffline();
        } else {
            FirebaseDatabase.DefaultInstance.GoOnline();
        }
    }

    public void Btn_start_auto(){
        if(this.adb.get_status()){
            this.adb.On_Stop();
            this.txt_btn_start.text="Start";
            this.img_icon_btn_start.sprite=this.sp_icon_start;
        }else{
            this.adb.On_Start(this.bank_items[this.index_sel_bank].name_file_macro);
            this.txt_btn_start.text="Stop";
            this.img_icon_btn_start.sprite=this.sp_icon_stop;
        }
        this.cr.play_sound_click();
    }
}
