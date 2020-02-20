using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;

// Parts of this class implement Nick's VRPawn code here https://github.com/nrewkowski/COMP590ClassExampleUnity/blob/master/Assets/NickVRPawn.cs 
public class VRPawn : MonoBehaviour
{
    public enum AttachmentRule{KeepRelative, KeepWorld, SnapToTarget}
    public GameObject grabObject;
    public LayerMask collectiblesMask;
    
    public CollectibleDictionary collectibleInventory;
    Dictionary<GameObject, int> prefabs;
    int numCollected = 0;
    float totalValue = 0;
    
    TextMesh updateText;
    public Camera camera;
    GameObject thingOnCone;
    Collectible thingIGrabbed;

    void Start()
    {
        updateText = GameObject.Find("Score").GetComponent<TextMesh>();
        
        prefabs = new Dictionary<GameObject, int>();
        // prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/BillardBall03Pf.prefab", typeof(GameObject)), 0);
        // prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/SoccerBallPf.prefab", typeof(GameObject)), 0);
        // prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/BillardBall10Pf.prefab", typeof(GameObject)), 0);
        // To build apk
        prefabs.Add((GameObject)Resources.Load("Prefabs/BillardBall03Pf", typeof(GameObject)), 0);
        prefabs.Add((GameObject)Resources.Load("Prefabs/SoccerBallPf", typeof(GameObject)), 0);
        prefabs.Add((GameObject)Resources.Load("Prefabs/BillardBall10Pf", typeof(GameObject)), 0);
        prefabs.Add((GameObject)Resources.Load("Prefabs/OculusWaterBottlePf", typeof(GameObject)), 0);
        updateScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)) {
            
            RaycastHit outHit;
            if (Physics.Raycast(grabObject.transform.position, grabObject.transform.forward, out outHit)) {
                
                Collider[] overlappingObjs = Physics.OverlapSphere(grabObject.transform.position, 1f, collectiblesMask);
                if (overlappingObjs.Length > 0) {
                    // updateText.text = "hit = " + overlappingObjs[0].gameObject;
                    thingIGrabbed=overlappingObjs[0].gameObject.GetComponent<Collectible>();
                    attachGameObjectToChild(overlappingObjs[0].gameObject, grabObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, true);
                }
            }

        } else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)) {
            letGo();
        }
    }

    // Per Nick's VRPawn
    public void attachGameObjectToChild(GameObject GOAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld)
    {
        GOAttach.transform.parent = newParent.transform;
        handleAttachmentRules(GOAttach, locationRule, rotationRule, scaleRule);
        if (weld) {
            simulatePhysics(GOAttach, Vector3.zero, false);
        }
    }

    // Per Nick's VRPawn
    public void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }

    // Per Nick's VRPawn
    public void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        GOToHandle.transform.localPosition = 
            (locationRule==AttachmentRule.KeepRelative) ? GOToHandle.transform.position :
                //technically don't need to change anything but I wanted to compress into ternary
                (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition :
                    new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles =
            (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles :
                //technically don't need to change anything but I wanted to compress into ternary
                (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
                    new Vector3(0,0,0);


        GOToHandle.transform.localScale =
            (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale :
                //technically don't need to change anything but I wanted to compress into ternary
                (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
                    new Vector3(1,1,1);
    }
    
    // Per Nick's VRPawn
    public void simulatePhysics(GameObject target,Vector3 oldParentVelocity,bool simulate){
        Rigidbody rb=target.GetComponent<Rigidbody>();
        if (rb) {
            if (!simulate) {
                Destroy(rb);
            } 
        } else {
            if (simulate){
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB=target.AddComponent<Rigidbody>();
                newRB.velocity=oldParentVelocity;
            }
        }
    }

    void letGo() 
    {
        if (thingIGrabbed){
            Collider[] objectOnCone = Physics.OverlapSphere(grabObject.transform.position,1f,collectiblesMask);
            if (objectOnCone.Length > 0) {
                bool rangeValid = checkRange(objectOnCone[0].gameObject);
                if (rangeValid) {
                    foreach (KeyValuePair<GameObject, int> pair in prefabs) {
                        if (objectOnCone[0].gameObject.GetComponent<Collectible>().name == pair.Key.name) {
                            totalValue += pair.Key.GetComponent<Collectible>().value;
                            if (!collectibleInventory.ContainsKey(pair.Key.GetComponent<Collectible>())) {
                                collectibleInventory[pair.Key.GetComponent<Collectible>()] = 1;
                            } else {
                                collectibleInventory[pair.Key.GetComponent<Collectible>()]++;
                            }
                        }
                    }
                    numCollected++;
                    detachGameObject(objectOnCone[0].gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                    Destroy(objectOnCone[0].gameObject);
                    updateScore();
                } else {
                    detachGameObject(objectOnCone[0].gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                    simulatePhysics(objectOnCone[0].gameObject,Vector3.zero,true);
                }

                // attachGameObjectToChild(objectOnCone[0].gameObject,grabObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
                // thingOnCone=objectOnCone[0].gameObject;
                thingIGrabbed=null;
            }
            // } else {
            //     detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
            //     simulatePhysics(thingIGrabbed.gameObject,(rightPointerObject.gameObject.transform.position-previousPointerPos)/Time.deltaTime,true);
            //     thingIGrabbed=null;
            // }
        }
    }

    bool checkRange(GameObject obj)
    {
        float offsetY = 1f;
        float range = 0.8f;
        Vector3 waistRange = new Vector3(camera.transform.position.x,camera.transform.position.y - offsetY,camera.transform.position.z);
        return ((obj.transform.position - waistRange).magnitude < range);
    }

    void updateScore()
    {
        string addText = "Austin Hale\nItem | Value | Count\n";
        foreach (KeyValuePair<GameObject, int> pair in prefabs) {
            if (collectibleInventory.ContainsKey(pair.Key.GetComponent<Collectible>())) {
                addText += pair.Key.GetComponent<Collectible>().name + " | " +
                    pair.Key.GetComponent<Collectible>().value + " | " +
                    collectibleInventory[pair.Key.GetComponent<Collectible>()] + "\n";
            }
        }
        addText += "Total Value: " + totalValue;
        updateText.text = addText;
    }
}
