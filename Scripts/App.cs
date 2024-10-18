using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class App : MonoBehaviour
{
    public GameObject prefab_item_bill;
    private DatabaseReference databaseRef;
    public Transform tr_all_item;

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
            foreach (var userSnapshot in args.Snapshot.Children)
            {
                string userId = userSnapshot.Key;
                string userName = userSnapshot.Child(userId).Value.ToString();
                Debug.Log("User ID: " + userId+" Monney:"+userName);

                if (userSnapshot.HasChild("children"))
                {
                    DataSnapshot childrenSnapshot = userSnapshot.Child("children");
                    foreach (var childSnapshot in childrenSnapshot.Children)
                    {
                        Debug.Log(childSnapshot.GetRawJsonValue());
                        GameObject obj_bill = Instantiate(this.prefab_item_bill);
                        obj_bill.transform.SetParent(this.tr_all_item);
                        obj_bill.transform.localPosition = Vector3.zero;
                        obj_bill.transform.localScale = new Vector3(1f, 1f, 1f);
                        obj_bill.GetComponent<Bill_Item>().txt_name.text = childSnapshot.GetRawJsonValue();
                    }
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
}
