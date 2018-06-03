using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class QuickLife : PlayerMechanics
{

    //Variables controlled by the eidtor to define the prefab's owner
    public int ringSize=175;

    public const int STAMINA_COST = 250;
    //In case we decide there will be more than one save
    public int saves_used;
    public List<Memento> savedObjects = new List<Memento>();
    public const int MAXSAVES = 1;

    // Use this for initialization
    void Awake()
    {
        saves_used = 0;
    }

    public override bool Activate() {
      Debug.Log("ACTIVATED!");
        if(saves_used == 0) {
            Save();
            return true;
        }
        else {
            bool status = GameManager.instance.DrainStamina(STAMINA_COST);
            if(status) {
              Load();
              return true;
            }
            else {
              return false;
            }
        }
    }

    /*
    private void OnGUI()
    {
        x_coord = gameObject.transform.position.x;
        y_coord = gameObject.transform.position.y;
        string s = String.Format("Coords: {0:F2}, {1:F2} \nSaves: {2} of {3}", x_coord, y_coord, saves_used, MAXSAVES);
        GUI.Label(new Rect(10, 10, 150, 50), s);
    }*/

    public void Save()
    {
        saves_used += 1;
        if (saves_used > MAXSAVES)
        {
            saves_used -= 1;
            return;
        }
        //Make Vibration to add all the materials to the savedObjects list
    		Vibration.Vibrator().MakeTimeVibration(ringSize, (Vector2) powerUser.transform.position, gameObject);
        //Save every object in your inventory
        Inventory.instance.SaveInventory(this);


        /*
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (!File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
        {
            file = File.Create(Application.persistentDataPath + "/playerinfo.dat");
        } else
        {
            file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
        }

        PlayerData data = new PlayerData();
        data.x_coord = gameObject.transform.position.x;
        data.y_coord = gameObject.transform.position.y;
        data.saves_used = saves_used;
        data.stamina = 100;

        bf.Serialize(file, data);
        file.Close();*/
    }

    public void SaveObject(GameObject touchedObj) {
      savedObjects.Add(touchedObj.GetComponent<Material>().CreateMemento());
      //NOTE: There is no option to check if the touched object is already in the list. If the same object is saved twice,
      //The Load method will revert both and the second entry will be the final action. The extra computation does not
      //seem to warrant checking every previous of an entry of the list for each new object.
    }

    public void SaveMemento(Memento savedMemento) {
      savedObjects.Add(savedMemento);
      //NOTE: There is no option to check if the touched object is already in the list. If the same object is saved twice,
      //The Load method will revert both and the second entry will be the final action. The extra computation does not
      //seem to warrant checking every previous of an entry of the list for each new object.
    }

    public void Load()
    {
        //Go through savedObject list and apply to each object
        foreach(Memento toRevert in savedObjects) {
          toRevert.Revert();
          //Destroy mementos
          Destroy(toRevert.gameObject);
        }
        savedObjects.Clear();
        //Decrease saves_used
        saves_used--;

        /*
        try
        {
            if (File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
                PlayerData data = (PlayerData)bf.Deserialize(file);

                saves_used = data.saves_used - 1;
                x_coord = data.x_coord;
                y_coord = data.y_coord;

                gameObject.transform.SetPositionAndRotation(new Vector2(x_coord, y_coord), new Quaternion(0,0,0,0));
            }
        } catch (IOException e)
        {
            Console.WriteLine("file does not exist: " + e);
        }*/
    }
}

/*
[Serializable]
class PlayerData
{
    public int saves_used;
    public int stamina;
}*/
