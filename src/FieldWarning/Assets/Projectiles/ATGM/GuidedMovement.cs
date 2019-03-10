/**
 * Copyright (c) 2017-present, PFW Contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is
 * distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See
 * the License for the specific language governing permissions and limitations under the License.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PFW.Weapons
{
    public class GuidedMovement : MonoBehaviour
    {
        
        //The mesh of the ATGM
        public Transform body;
        //this needs to be able to be set without a Target, for ATGMS targetet at certain Positions instead of Objects
        public Vector3 TargetPos { get; set; }
        [SerializeField] private int Speed = 10;
        //How intense the random side movement is
        [Range(0, 1)]
        [SerializeField] private float Volatility = 1;
        //How hard the ATGM can Turn
        [SerializeField] private int TurnAbility = 10;
        //Would be set by the Shooter of the ATGM
        public Transform Target;
        private Transform _ATGM;
        private bool _alive = true;
        [SerializeField] private float _hitDistance = 0.01f;
        //Used for the Random side-movement
        private float _time = 0;
        void Start()
        {
            _ATGM = this.transform;
        }
        //there probably should be a hit option which works with colliders and not position,also there is yet no case where the rocked goes completely off-course
        void Update()
        {
            _time += Time.deltaTime*360;
            if (Vector3.SqrMagnitude(_ATGM.position-TargetPos) > _hitDistance && _alive)
            {
                //For moving Targets
                if (Target != null)
                {
                    TargetPos = Target.position;
                }
                _ATGM.position += Speed * _ATGM.forward * Time.deltaTime * 0.1f;



                //Random side-movement based on a sin curve
                /*
                _ATGM.Rotate(Vector3.up * Mathf.Sin(_time) * Time.deltaTime*50);
                _ATGM.Rotate(Vector3.right * Mathf.Sin(_time) * Time.deltaTime * 50);*/
                //Random side-movement based on circular movement
                //get angle between 0 and 360
                float angle = _time % 360;
                //get a 2D Point on a Circle based on that angle
                Vector3 pos = new Vector3((0 + Volatility * Mathf.Cos(Mathf.Deg2Rad * angle)), (0 + Volatility * Mathf.Sin(Mathf.Deg2Rad * angle)), body.position.z);
                //Displace the Mesh
                Vector3 newpos = body.up * pos.x + body.right * pos.y;
                body.localPosition = newpos/5;

                //turn normal towards target
                Vector3 newDir = Vector3.RotateTowards(_ATGM.forward, Target.position - _ATGM.position, TurnAbility * Time.deltaTime * 0.08f, 0.0f);
                Debug.Log(Quaternion.LookRotation(newDir));
                _ATGM.rotation = Quaternion.LookRotation(newDir);
                body.rotation = _ATGM.rotation;
            }
            else
            {
                //hit do explody stuff
                _alive = false;
            }

        }
    }
}
