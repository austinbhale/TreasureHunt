using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreasureHunterInventory : MonoBehaviour
{
    // public List<Collectible> inventoryItems = new List<Collectible>();
    public CollectibleDictionary collectibleInventory;
    Camera viewpointCamera;
    int numCollected = 0;
    float totalValue = 0;
    Dictionary<GameObject, int> prefabs;

    void Start() {
        viewpointCamera = this.gameObject.GetComponent<Camera>();
        prefabs = new Dictionary<GameObject, int>();
        prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/3rd Person Controller.prefab", typeof(GameObject)), 0);
        prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/SoccerBallPf.prefab", typeof(GameObject)), 0);
        prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/ToyT-BlockPf.prefab", typeof(GameObject)), 0);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("space pressed");
            Ray ray = new Ray(viewpointCamera.transform.position, transform.forward);
            Vector3 endPosition;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.gameObject.GetComponent<Collectible>() != null) {
                    int keyIndex = 0;
                    foreach (KeyValuePair<GameObject, int> pair in prefabs) {
                        if (hit.transform.gameObject.GetComponent<Collectible>().name == pair.Key.name) {
                            totalValue += pair.Key.GetComponent<Collectible>().value;
                            if (!collectibleInventory.ContainsKey(pair.Key.GetComponent<Collectible>())) {
                                collectibleInventory[pair.Key.GetComponent<Collectible>()] = 1;
                            } else {
                                collectibleInventory[pair.Key.GetComponent<Collectible>()]++;
                            }
                        }
                    }

                    numCollected++;
                    Destroy(hit.transform.gameObject);
                    GameObject.Find("Score").GetComponent<TextMesh>().text = "Austin Hale"
                        + "\nInventory Count: " + numCollected + "\nTotal Value: " + totalValue;
                }
            }
        }
    }
}