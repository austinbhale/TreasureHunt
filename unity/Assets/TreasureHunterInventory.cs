using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreasureHunterInventory : MonoBehaviour
{
    // public List<Collectible> inventoryItems = new List<Collectible>();
    public CollectibleDictionary collectibleInventory;
    Collectible thingIGrabbed;
    Camera viewpointCamera;
    int numCollected = 0;
    float totalValue = 0;
    // Dictionary<GameObject, int> prefabs;

    // private Rigidbody rb;

    void Start() {
        viewpointCamera = this.gameObject.GetComponent<Camera>();
        // rb = GetComponent<Rigidbody>();
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
                    // foreach (KeyValuePair<GameObject, int> pair in prefabs) {

                        if (hit.transform.gameObject.GetComponent<Collectible>()) {
                            totalValue += hit.transform.gameObject.GetComponent<Collectible>().value;
                            Debug.Log(hit.transform.gameObject.GetComponent<Collectible>().name);
                            // GameObject c = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/" + hit.transform.gameObject.GetComponent<Collectible>().name + ".prefab", typeof(GameObject));
                            GameObject c = (GameObject)Resources.Load("Assets/Resources/Prefabs/" + hit.transform.gameObject.GetComponent<Collectible>().name + ".prefab", typeof(GameObject));
                            if (!collectibleInventory.ContainsKey(c.GetComponent<Collectible>())) {
                                // thingIGrabbed = c.GetComponent<Collectible>();
                                // AssetDatabase.LoadAssetAtPath("Prefabs/"+ thingIGrabbed.name + ".prefab");
                                collectibleInventory[c.GetComponent<Collectible>()] = 1;
                            } else {
                                collectibleInventory[c.GetComponent<Collectible>()]++;
                            }
                        }

                        // if (!collectibleInventory.ContainsKey(thingIGrabbed)) {
                            
                        //     collectibleInventory[thingIGrabbed] = 1;
                        // } else {
                        //     collectibleInventory[thingIGrabbed]++;
                        // }
                    // }

                    numCollected++;
                    Destroy(hit.transform.gameObject);
                    GameObject.Find("Score").GetComponent<TextMesh>().text = "Austin Hale"
                        + "\nInventory Count: " + numCollected + "\nTotal Value: " + totalValue;
                }
            }
        }
    }
}