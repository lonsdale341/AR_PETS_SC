using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBStruct<T> where T : new()
{


    // Database path to the struct.
    public string dbPathName = "<<UNNAMED STRUCT>>";
    public bool areChangesPending { get; private set; }

    public T data { get; private set; }
    public T newData { get; private set; }

    Firebase.Database.FirebaseDatabase database;
    Firebase.FirebaseApp app;

    DBStruct()
    {
        DiscardRemoteChanges();
    }

    public DBStruct(string name, Firebase.FirebaseApp app)
    {
        this.app = app;
        database = Firebase.Database.FirebaseDatabase.GetInstance(this.app);
        dbPathName = name;
        data = new T();
        newData = new T();
        database.GetReference(dbPathName).ValueChanged += OnDataChanged;
    }

    public void ApplyRemoteChanges()
    {
        if (areChangesPending)
        {
            data = newData;
            DiscardRemoteChanges();
        }
    }

    public void DiscardRemoteChanges()
    {
        areChangesPending = false;
    }

    // Returns a guaranteed unique string, usable as a dictionary key value.
    public string GetUniqueKey()
    {
        return database.RootReference.Child(dbPathName).Push().Key;
    }

    public void Initialize(T value)
    {
        data = value;
        newData = value;
        DiscardRemoteChanges();
        PushData();
    }

    void OnDataChanged(object sender, Firebase.Database.ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Something went wrong - database error on struct [" + dbPathName + "]!\n"
                + args.DatabaseError.ToString());
            return;
        }
        T newValue = JsonUtility.FromJson<T>(args.Snapshot.GetRawJsonValue());
        newData = newValue;

        areChangesPending = true;
    }

    public void PushData()
    {
        UnityEngine.Assertions.Assert.IsNotNull(database, "Database ref is null!");
        string json = JsonUtility.ToJson(data);
        database.RootReference.Child(dbPathName).SetRawJsonValueAsync(json);
    }
}
