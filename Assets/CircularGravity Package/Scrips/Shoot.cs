/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 04/21/14
 *  
 *  Description: 
 *  
 *      Shoots gameobject.
 * 
 *  Inputs:
 * 
 *      inputControl: Control to shoot gameobject.
 *      
 *      bullet: Gameobject to shoot.
 *      
 *      bulletPower: Force power to shoot the gameobject.
 *      
 *      bulletLife: Life time of shot gameobject.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class Shoot : MonoBehaviour
    {
        #region Properties

        //Input to shoot
        public string inputControl = "Jump";

        //GameObject to shoot
        public GameObject bullet;

        //Bullet Speed
        public float bulletPower = 1000f;

        //Bullet life
        public float bulletLife = 10f;

        #endregion

        #region Unity Functions

        //Use this for initialization
        void Start()
        {
        }

        //Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            if (Input.GetButtonDown(inputControl))
            {
                GameObject instance = Instantiate(bullet, transform.position, transform.rotation) as GameObject;

                Rigidbody[] ArrayRigs = instance.GetComponentsInChildren<Rigidbody>();
                foreach (var item in ArrayRigs)
                {
                    item.GetComponent<Rigidbody>().AddForce(this.transform.forward * bulletPower);
                }

                DestroyObject(instance, bulletLife);
            }
        }

        #endregion
    }
}