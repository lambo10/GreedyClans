using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArmyMovement : MonoBehaviour
{
    private float minDistance = 200f, maxDistance = 1000f;
    private armyJsonArry previous_armyJsonArry = null;
    Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();
    [SerializeField]public GameObject Griffen, GrandWarden, Golem,Goblin,Ghost,Fairy,Ent,Dragon,BarbarianKing,BabyDragon,ArcherQueen,Archer,Wizard,Barbarian,Peasant,Troll;
    // Start is called before the first frame update
    void Start()
    {
        
        dictionary.Add(25, Griffen);
        dictionary.Add(14, GrandWarden);
        dictionary.Add(22, Golem);
        dictionary.Add(18, Goblin);
        dictionary.Add(24, Ghost);
        dictionary.Add(27, Fairy);
        dictionary.Add(17, Ent);
        dictionary.Add(20, Dragon);
        dictionary.Add(12,BarbarianKing);
        dictionary.Add(21, BabyDragon);
        dictionary.Add(13, ArcherQueen);
        dictionary.Add(16, Archer);
        dictionary.Add(19, Wizard);
        dictionary.Add(15, Barbarian);
        dictionary.Add(26, Peasant);
        dictionary.Add(23, Troll);
        StartCoroutine(get_player_army(Helper.get_player_army(playerDetails.usr_walletAddress, playerDetails.landNo, playerDetails.usr_sessionID, Helper.chainId)));
    }


    IEnumerator get_player_army(string url)
    {
        //spiner.SetActive(true);
        // clear error message
        //errorMessage.text = "";
        // unity web request

        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            Debug.Log(url);

            armyInit army = JsonUtility.FromJson<armyInit>(response);


            if (army.success != null && !army.success)

            {
                checkExpiredSession.checkSessionExpiration(army.msg);
            }
            else
            {
                string newJson = "{\"armyCamp\":" + army.playerArmy + "}";
                armyJsonArry newArmy = JsonUtility.FromJson<armyJsonArry>(newJson);
                int[] nftId = newArmy.armyCamp;
                for (int i = 0;i < nftId.Length; i++)
                {
                    if (dictionary.ContainsKey(nftId[i])){
                        GameObject trainedWarrior = dictionary[nftId[i]];
                        if(trainedWarrior != null)
                        {
                            GameObject instance_ = Instantiate(trainedWarrior, RandolmlyPickWhereToInstanciate(), Quaternion.identity);
                            BoxCollider instance_boxC = instance_.GetComponent<BoxCollider>();
                            instance_boxC.enabled = false;

                            instance_.AddComponent<moveRandomly>();
                        }
                    }
                }
                previous_armyJsonArry = newArmy;
            }
        }
    }





    public void get_player_army_after_train()
    {
        StartCoroutine(get_player_army_after_train_req(Helper.get_player_army(playerDetails.usr_walletAddress, playerDetails.landNo, playerDetails.usr_sessionID, Helper.chainId)));
    }



    IEnumerator get_player_army_after_train_req(string url)
    {
        //spiner.SetActive(true);
        // clear error message
        //errorMessage.text = "";
        // unity web request

        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            Debug.Log(response);

            armyInit army = JsonUtility.FromJson<armyInit>(response);


            if (army.success != null && !army.success)

            {
                checkExpiredSession.checkSessionExpiration(army.msg);
            }
            else
            {
                string newJson = "{\"armyCamp\":" + army.playerArmy + "}";
                armyJsonArry newArmy = JsonUtility.FromJson<armyJsonArry>(newJson);
                int[] nftId = CancelOut(previous_armyJsonArry.armyCamp, newArmy.armyCamp);
                for (int i = 0; i < nftId.Length; i++)
                {
                    if (dictionary.ContainsKey(nftId[i]))
                    {
                        GameObject trainedWarrior = dictionary[nftId[i]];
                        if (trainedWarrior != null)
                        {
                            GameObject instance_ = Instantiate(trainedWarrior, RandolmlyPickWhereToInstanciate(), Quaternion.identity);
                            BoxCollider instance_boxC = instance_.GetComponent<BoxCollider>();
                            instance_boxC.enabled = false;

                            instance_.AddComponent<moveRandomly>();
                        }
                    }
                }
                previous_armyJsonArry = newArmy;
            }
        }
    }



    public static int[] CancelOut(int[] p, int[] n)
    {
        // Create a list to store the values that are left after canceling
        List<int> values = new List<int>();

        // Create a dictionary to store the count of each value in p
        Dictionary<int, int> countsP = new Dictionary<int, int>();
        foreach (int value in p)
        {
            if (countsP.ContainsKey(value))
            {
                countsP[value]++;
            }
            else
            {
                countsP[value] = 1;
            }
        }

        // Create a dictionary to store the count of each value in n
        Dictionary<int, int> countsN = new Dictionary<int, int>();
        foreach (int value in n)
        {
            if (countsN.ContainsKey(value))
            {
                countsN[value]++;
            }
            else
            {
                countsN[value] = 1;
            }
        }

        // Iterate over the keys in the first dictionary
        foreach (int key in countsP.Keys)
        {
            // Get the count of this value in the second dictionary
            int count = countsN.GetValueOrDefault(key);

            // If the count is greater than the count in the first dictionary, add the values from the first dictionary to the list
            if (count > countsP[key])
            {
                for (int i = 0; i < countsP[key]; i++)
                {
                    values.Add(key);
                }
            }
            // If the count is equal to or less than the count in the first dictionary, subtract the count from the first dictionary
            else
            {
                countsP[key] -= count;
                for (int i = 0; i < countsP[key]; i++)
                {
                    values.Add(key);
                }
            }
        }

        // Iterate over the keys in the second dictionary
        foreach (int key in countsN.Keys)
        {
            // Skip keys that are already in the first dictionary
            if (countsP.ContainsKey(key))
            {
                continue;
            }

            // Add the count of this value to the list
            for (int i = 0; i < countsN[key]; i++)
            {
                values.Add(key);
            }
        }

        // Return the list as an array
        return values.ToArray();
    }



    private Vector3 RandolmlyPickWhereToInstanciate()
    {
        // Choose a random direction and distance from the game object's current position
        Vector3 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);

        // Calculate the position of the new waypoint
        Vector3 newWaypoint = transform.position + direction * distance;
        //ArrayList waypoints = FindNextFree(newWaypoint);
        //Node wapPnode = (Node)waypoints[0];
        //return wapPnode.position;
        return newWaypoint;
    }




}

