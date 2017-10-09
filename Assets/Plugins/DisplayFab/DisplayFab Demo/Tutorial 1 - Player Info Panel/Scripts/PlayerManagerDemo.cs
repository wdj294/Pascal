using UnityEngine;
using System.Collections;
using System;
using Techooka.DisplayFab;


public class PlayerManagerDemo : MonoBehaviour {
    [SerializeField]
    private string _playerName="Test Player";
    [SerializeField]
    private float _playerHealth=45;
    [SerializeField]
    private int _playerHighScore=400;

    

    public string playerName{ get { return _playerName; } set { _playerName = value; } }
    public float playerHealth { get { return _playerHealth; }
                        set {  _playerHealth = value;  SendMessage("DSFNotifyGroup", "UpdateSlidersDisplay",SendMessageOptions.DontRequireReceiver); /*This is the Group dispatch notification. You can either dispatch by group name or by groupUID*/
                            } }
    [SerializeField]
    public int playerHighScore { get { return _playerHighScore; } set { _playerHighScore = value; } }
 

    [ContextMenu("Generate Random Health")]
    public void GenerateRandomHealth()
    {

        playerHealth = UnityEngine.Random.Range(1, 90);

    }


 

}
