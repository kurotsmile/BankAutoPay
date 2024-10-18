using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    private DatabaseReference databaseRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                ReadDataFromFirebase();
            }
            else
            {
                Debug.LogError("Không thể khởi tạo Firebase: " + task.Result);
            }
        });
    }

    void ReadDataFromFirebase()
    {
        DatabaseReference usersRef = databaseRef.Child("users");
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
            foreach (var childSnapshot in args.Snapshot.Children)
            {
                Debug.Log("User ID: " + childSnapshot.Key);
                Debug.Log("User Data: " + childSnapshot.GetRawJsonValue());
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
}
