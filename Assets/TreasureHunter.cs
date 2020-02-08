using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureHunter : MonoBehaviour
{
    OVRCameraRig oVRCameraRig;
    OVRManager oVRManager;
    OVRHeadsetEmulator oVRHeadsetEmulator;
    Camera viewpointCamera;
    // PostProcessLayer postProcessLayer;
    // LocomotionHandler locomotionHandler;
    float currentTotalScore;

    // Start is called before the first frame update
    void Start()
    {
        oVRCameraRig = this.gameObject.GetComponent<OVRCameraRig>();
        oVRManager = this.gameObject.GetComponent<OVRManager>();
        oVRHeadsetEmulator = this.gameObject.GetComponent<OVRHeadsetEmulator>();
        viewpointCamera = this.gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("key1"); 
            if (!this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Contains(GameObject.Find("coin").GetComponent<Collectible>())) {
                this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(GameObject.Find("coin").GetComponent<Collectible>()); 
                calculateScore();         
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("key2");
            if (!this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Contains(GameObject.Find("chest").GetComponent<Collectible>())) {
                this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(GameObject.Find("chest").GetComponent<Collectible>());  
                calculateScore();         
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("key3"); 
            if (!this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Contains(GameObject.Find("diamond").GetComponent<Collectible>())) {
                this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Add(GameObject.Find("diamond").GetComponent<Collectible>());
                calculateScore();          
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            calculateScore();
            GameObject.Find("Score").GetComponent<TextMesh>().text = "Austin Hale"
                + "\nInventory Count: " + this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems.Count
                + "\nScore: " + currentTotalScore;
            Debug.Log("key4");   
        }
    }

    float calculateScore() {
        List<Collectible> collectibles = this.gameObject.GetComponent<TreasureHunterInventory>().inventoryItems;
        float totalScore = 0;
        foreach (Collectible treasure in collectibles) {
            totalScore += treasure.value;
        }

        if (collectibles.Count == 3) {
            GameObject.Find("Win").GetComponent<TextMesh>().text = "You win";
        }
        currentTotalScore = totalScore;
        return totalScore;
    }
}
