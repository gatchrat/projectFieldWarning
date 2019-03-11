using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {

    public float speed = 50f * TerrainConstants.MAP_SCALE;
    public float range = 1000f * TerrainConstants.MAP_SCALE;
    public float explosionRadius = 5f;
    public float damage = 25;
    public float _rotLerpSpeed = 1 / 1000f;
    public GameObject TrailEffect;
    public GameObject ExplosionEffect;
    Rigidbody rb;
    GameObject target;
    RaycastHit hitInfo;
    Vector3 estimatedTargetPos;
    Vector3 prevPos;
    private Vector3 _oldDisplacement;
    float estimatedTargetPosCorr;
    private MissileStateEnum MissileState;
    GameObject ParentWeaponSystem;
    private float _time = 0;
    [Range(0, 1)]
    [SerializeField] private float Volatility = 1;
    // Update is called once per frame
    void Update() {

    }

    private void Start()
    {
        _oldDisplacement = new Vector3(0,0,0);
        prevPos = Vector3.zero;
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        _time += Time.deltaTime * 360;
        //Calculate estimated time till missile reach target (distance / speed)
        var distanceRemaining = Vector3.Distance(target.transform.position, transform.position);
        var timeRemaining = distanceRemaining / speed;
        Debug.Log(timeRemaining);
        //Calculate estimated position of the target after the elapsed time
        //if (target == Terrain.activeTerrain.gameObject) {
        if (3>4) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hitInfo);
            estimatedTargetPos = hitInfo.point;
        } else if (target.GetComponent<Rigidbody>() != null) {
            target.transform.Translate(Vector3.forward * speed / 5f * Time.fixedDeltaTime);
            estimatedTargetPosCorr = target.GetComponent<Rigidbody>().velocity.magnitude * timeRemaining;
            estimatedTargetPos = target.transform.position + estimatedTargetPosCorr * target.transform.forward;
        } else {
            target.transform.Translate(Vector3.forward * speed / 5f * Time.fixedDeltaTime);
            estimatedTargetPosCorr = CalTargetVelocity().magnitude * timeRemaining;
            estimatedTargetPos = target.transform.position + estimatedTargetPosCorr * target.transform.forward;
        }
        //Set missile target to the estimated position
        var quat = Quaternion.LookRotation((estimatedTargetPos - transform.position).normalized);
       transform.rotation = Quaternion.Lerp(quat, transform.rotation, 0.001f);
        transform.Translate(transform.forward * speed * Time.fixedDeltaTime);



        float angle = _time % 360;
        //get a 2D Point on a Circle based on that angle
        Vector3 pos = new Vector3((0 + Volatility * Mathf.Cos(Mathf.Deg2Rad * angle)), (0 + Volatility * Mathf.Sin(Mathf.Deg2Rad * angle)), 0);
        //Displace the Mesh
        Vector3 _newDisplacement = transform.up * pos.x + transform.right * pos.y;
        _newDisplacement = _newDisplacement / 5;

        transform.position = transform.position + (_newDisplacement - _oldDisplacement);
        _oldDisplacement = _newDisplacement;
        //TODO: Check if distance to target got bigger. It means we went past the target (We missed)
        //TODO: Also continually ask the weapon system, if it got LOS and if it is still in range to guide the missile (except if missile is FF)
        //TODO: If miss, Stop tracking the target, then activate gravity on missile, give it a random rotation so that it will crash somewhere
        //Then report back the miss to the weapon system
        //TODO: Aim at Center of Target Instead of pure Position
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TODO: Give weapon damage to object hit or send impact confirmation to weapon system and he will apply the damage

        //TODO: Instantiate explosion effect depending of what was hit (target, terrain, water, etc)
        Instantiate(ExplosionEffect, transform.localPosition, transform.localRotation);
        Destroy(gameObject);
    }

    public void SetTarget(GameObject tgr)
    {
        target = tgr;
    }

    public void SetParentWeaponSystem(GameObject parent)
    {
        ParentWeaponSystem = parent;
    }

    public MissileStateEnum GetMissileState()
    {
        //Missile should directly tell it's state to the weapon system because once it explodes we lose the object. So this method might be useless
        return MissileState;
    }

    private Vector3 CalTargetVelocity()
    {
        var currVel = (prevPos - target.transform.position) / Time.fixedDeltaTime;  //Called in fixedUpdate
        prevPos = target.transform.position;
        return currVel;
    }

    public enum MissileStateEnum
    {
        InFlightTowardsTarget,
        TargetMissed,
        TargetHit,
        GroundHit
    }
}
