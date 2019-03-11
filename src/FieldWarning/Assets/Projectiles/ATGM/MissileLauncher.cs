using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MissileLauncher : MonoBehaviour
{

    [Header("Missile settings")]
    
    public GameObject atgmPrefab;
    public float delayBetweenMissile = 1f;
    public int MissileNumber = 8;
    int _remainingMisiles = 8;
    float DelayTillNextMissile;
    RaycastHit hitInfo;
    GameObject target;

    private void Start()
    {
        DelayTillNextMissile = delayBetweenMissile;
        _remainingMisiles = MissileNumber;
    }

    // Update is called once per frame
    void Update () {
        DelayTillNextMissile -= Time.deltaTime;
        if (_remainingMisiles > 0) {
            if (DelayTillNextMissile <= 0 && Input.GetKey(KeyCode.K) && Input.GetMouseButton(0)) {
                if (target != null) {
                    FireMissile();
                } else {
                    Debug.Log("Need a target before firing missile!");
                }

            }
        } else {
            Debug.Log("No missile remaining!");
        }
        
        if (Input.GetKey(KeyCode.O)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo)) {
                target = hitInfo.collider.transform.parent.gameObject;
                Debug.Log("Target set to " + target.name + "...");
            }
        }
        
    }

    void FireMissile()
    {
        Debug.Log("Fired!");
        GameObject atgm = Instantiate(atgmPrefab, transform.position+new Vector3(0,0.2f,0), transform.rotation);
        atgm.GetComponent<Trajectory>().SetTarget(target);
        atgm.GetComponent<Trajectory>().SetParentWeaponSystem(this.gameObject);
        _remainingMisiles--;
        Debug.Log("Remaining missile: " + _remainingMisiles + " out of " + MissileNumber);
        DelayTillNextMissile = delayBetweenMissile;
    }

}
