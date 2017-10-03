using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetListEntry
{

    //  Constructor
    public PetListEntry(string name, string mapId)
    {
        this.petId = name;
        this.petId = mapId;
    }

    // Unique database identifier.
    public string petId;
    // Plaintext string name.
    public string name = StringConstants.DefaultPetName;
}
 [System.Serializable]
public class UserData  {

    // Database ID
    public string id = "<<ID>>";
    // Plaintext name
    public string nameUser = "<<USER NAME>>";
    public string nameMyPet = "";
    public string nameFriendPet = "";
    // List of all maps owned by this player.
    public List<PetListEntry> maps = new List<PetListEntry>();
}
