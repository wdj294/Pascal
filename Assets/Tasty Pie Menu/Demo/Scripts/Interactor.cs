using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityStandardAssets.Characters.FirstPerson;
//using UnityStandardAssets.ImageEffects;
using Xamin;

public class Interactor : MonoBehaviour {

    public Camera fpsCamera;
    public CircleSelector menu;
    public GameObject guiText;

    void Update()
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            print("I'm looking at " + hit.collider.tag);
            if (hit.collider.tag == "CoffeMug")
            {
                currentMug = hit.collider.GetComponent<CoffeMug>();
                guiText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //GetComponent<FirstPersonController>().enabled = false;
                    //fpsCamera.GetComponent<Blur>().enabled = true;
                    Cursor.lockState = CursorLockMode.None;
                    menu.GetButtonWithId("drink").unlocked = currentMug.isFilled;
                    menu.GetButtonWithId("refill").unlocked = !currentMug.isFilled;
                    menu.Open();
                    guiText.SetActive(false);
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    menu.Close();
                    Cursor.lockState = CursorLockMode.Locked;
                    //GetComponent<FirstPersonController>().enabled = true;
                    //fpsCamera.GetComponent<Blur>().enabled = false;
                    guiText.SetActive(false);
                }
            }
            else {
                menu.Close();
                guiText.SetActive(false);
                //fpsCamera.GetComponent<Blur>().enabled = false;
            }
        }
        else
        {
            print("I'm looking at nothing!");
            //fpsCamera.GetComponent<Blur>().enabled = false;
            menu.Close();
        }
    }

    CoffeMug currentMug;

    public void SetFillStateOfCup(bool isFilled)
    {
        currentMug.SetMugFilled(isFilled);
    }
}
