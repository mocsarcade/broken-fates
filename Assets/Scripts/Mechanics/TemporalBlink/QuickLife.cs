using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;

public class QuickLife : PlayerMechanics
{

    public const int RING_SIZE=75;
    public const int STAMINA_COST = 10;

    //In case we decide there will be more than one save
    public int saves_used;
    public List<Memento> savedObjects = new List<Memento>();
    public const int MAXSAVES = 1;

  	 //This instance will be used so if both slots are over the same power, they will reference the same power
  	 private static QuickLife instance;

    // Use this for initialization
    void Awake()
    {
        saves_used = 0;
    }

   // Use this for initialization
   public override void Initialize (GameObject user) {
     base.Initialize(user);
     if(instance == null)
       instance = this;
   }

    public override bool Activate() {
        base.Activate();
        active = false;
        if(saves_used == 0) {
          Save();
          return true;
        }
        else {
          bool status = Load();
          return status;
        }
    }

  	//When power is swapped prematurely before power ends, this method is called to clean up the power
  	public override void Deactivate(bool setMobility)
  	{
      base.Deactivate(setMobility);
      //No clean-up required... for now.
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
    		Vibration.Vibrator().MakeTimeVibration(RING_SIZE, (Vector2) powerScript.GetPosition(), gameObject);
        //Save every object in your inventory
        Inventory.instance.SaveInventory(this);
        //Save Game Statistics such as handIndex or HP
        GameManager.instance.SaveGameStatistics(this);

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
      //Debug.Log("Saved Object: " + touchedObj);
      Memento tempMemento = touchedObj.GetComponent<Material>().CreateMemento();
      //Check that this object hasn't already been saved
      foreach(Memento includedMem in savedObjects) {
        if(includedMem.GetParent() == tempMemento.GetParent() && includedMem.GetParent() != null) {
          Debug.LogException(new Exception("Trying to save an object that's already been saved!"), this);
          return;
        }
      }
      //If this object hasn't been saved yet, it can be added to the list
      savedObjects.Add(tempMemento);
      //NOTE: There is no option to check if the touched object is already in the list. If the same object is saved twice,
      //The Load method will revert both and the second entry will be the final action. The extra computation does not
      //seem to warrant checking every previous of an entry of the list for each new object.
    }

    public void SaveMemento(Memento savedMemento) {
      //Debug.Log("Saved Memento: " + savedMemento);
      //Check that this object hasn't already been saved
      foreach(Memento includedMem in savedObjects) {
        if(includedMem.GetParent() == savedMemento.GetParent() && includedMem.GetParent() != null) {
          Debug.LogException(new Exception("Trying to save an object that's already been saved!"), this);
          return;
        }
      }
      //If this object hasn't been saved yet, it can be added to the list
      savedObjects.Add(savedMemento);
      //NOTE: There is no option to check if the touched object is already in the list. If the same object is saved twice,
      //The Load method will revert both and the second entry will be the final action. The extra computation does not
      //seem to warrant checking every previous of an entry of the list for each new object.
    }

    public bool Load()
    {
      bool status = GameManager.instance.DrainCap(STAMINA_COST);
      if(status == true) {
        //Go through savedObject list and apply to each object
        foreach(Memento toRevert in savedObjects) {
          toRevert.Revert();
          //Destroy mementos
          if(toRevert) {
            Destroy(toRevert.gameObject);
          }
        }
        savedObjects.Clear();
        //Decrease saves_used
        saves_used--;
      }

      return status;

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

    //When this object is destroyed (by player moving to a different mechanic as a choice)
    //Destroy all current mementos
    void OnDestroy() {
        //Go through savedObject list and apply to each object
        foreach(Memento toRevert in savedObjects) {
          //Destroy mementos
          if(toRevert) {
            Destroy(toRevert.gameObject);
          }
        }
        savedObjects.Clear();
        //Decrease saves_used
        saves_used--;
    }

  	public override PlayerMechanics GetInstance() {
  		return instance;
  	}
}

/*
[Serializable]
class PlayerData
{
    public int saves_used;
    public int stamina;
}*/
