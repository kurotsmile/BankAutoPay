using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public GameObject prefab_item_bill;
    private DatabaseReference databaseRef;
    public Transform tr_all_item;
    private FirebaseApp customApp;
    public Text txt_status_app;
    void Start()
    {
        // Tạo cấu hình Firebase tuỳ chỉnh
        AppOptions options = new AppOptions
        {
            ApiKey = "AIzaSyBKQ51navOWhgLHY1flH7eK4hPuj9knOa0", 
            AppId = "14228562704",
            ProjectId = "clbank",
            DatabaseUrl = new Uri("https://clbank-default-rtdb.asia-southeast1.firebasedatabase.app"),
            StorageBucket = "clbank.appspot.com"
        };

        // Khởi tạo ứng dụng Firebase với cấu hình tuỳ chỉnh
        customApp = FirebaseApp.Create(options, "customApp_bank");

        // Khởi tạo Firebase Database với ứng dụng tuỳ chỉnh
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

                    bill.Set_Act_Click(() =>
                    {
                        //this.RunMirFileWithMemu();
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


    public void Quit_App()
    {
        Application.Quit();
    }

    public void Clear_contain(Transform tr)
    {
        foreach(Transform child in tr)
        {
            Destroy(child.gameObject);
        }
    }

    public void RunMirFileWithMemu()
    {
        string memuPath = @"J:\Microvirt\MEmu\MEmuc.exe";  // Đường dẫn đến file MEmu.exe
        string mirFilePath = @"J:\Microvirt\MEmu\scripts\20241018221107.mir"; // Đường dẫn đến file .mir


        // Tạo tiến trình để chạy MEmu với file recorder .mir
        System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo();
        processInfo.FileName = memuPath;                  // Chạy memuc
        processInfo.Arguments = "MEmuc -i 0 adb 'shell input tap 357 405'"; // Tham số: chạy file recorder

        // Khởi chạy tiến trình
        System.Diagnostics.Process.Start(processInfo);
        UnityEngine.Debug.Log("Running recorder file (.mir) with MEmu: " + mirFilePath);
    }

    public void Btn_start_Memu(){
        this.txt_status_app.text="Memu emulator launched";
        this.RunCommandWithMemu("start");
    }

    public void RunCommandWithMemu(string s_command)
    {
        string memuPath = @"J:\Microvirt\MEmu\MEmuc.exe"; 

        System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo();
        processInfo.FileName = memuPath; 
        processInfo.Arguments = "MEmuc -i 0 "+s_command; 

        System.Diagnostics.Process.Start(processInfo);
        UnityEngine.Debug.Log("Running CMD with MEmu: " + s_command);
    }
}
