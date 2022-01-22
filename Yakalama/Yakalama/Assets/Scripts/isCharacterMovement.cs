using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class isCharacterMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private int movVer,movHor, xval, yval, order, score;
    PhotonView pv;
    public AudioSource aus;
    public AudioClip clip, c2, c3;
    //[SerializeField]
    //private Text infotxt;
    //[SerializeField]
    //private GameObject bait;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(xval);
            stream.SendNext(yval);
        }
        else
        {
            // Network player, receive data
            this.xval = (int)stream.ReceiveNext();
            this.yval = (int)stream.ReceiveNext();
        }
    }

    void Start()
    {
        order = PhotonNetwork.LocalPlayer.ActorNumber;
        pv = GetComponent<PhotonView>();
        movVer = 27;
        movHor = -30;
        this.transform.position = new Vector3(movHor, movVer, 0);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 24;
        
        /*if (GameObject.FindGameObjectWithTag("axisValueText"))
            infotxt = GameObject.FindGameObjectWithTag("axisValueText").GetComponent<Text>();
        else
            Debug.LogError("Axis Gösterici Bulunamadý");
        
        if (GameObject.FindGameObjectWithTag("bait"))
            bait = GameObject.FindGameObjectWithTag("bait");
        else
            Debug.LogError("Yem Bulunamadý");*/

    }
    void Update()
    {
        if (pv.IsMine)
        {
            pv.RPC("BaitSetPos", RpcTarget.All);
            Movements();
            this.GetComponent<SpriteRenderer>().color = Color.green;
        }
       
    }
    private void Movements() 
    {
        #region Inputs
        if (Input.GetButtonDown("forwardbtn"))
        {
            movVer += 3;
            NormalMovement();
        }
        if (Input.GetButtonDown("backwardbtn"))
        {
            movVer -= 3;
            NormalMovement();
        }
        if (Input.GetButtonDown("leftbtn"))
        {
            movHor -= 3;
            NormalMovement();
        }
        if (Input.GetButtonDown("rightbtn"))
        {
            movHor += 3;
            NormalMovement();
        }
        #endregion
    }
    [PunRPC]
    private void BaitSetPos() 
    {
        #region Bait Set Position
        if (GameObject.FindGameObjectWithTag("bait").transform.position == this.transform.position)
        {   /* set it for x3 speed */
            xval = Random.Range(-250, 250);
            yval = Random.Range(20, 380);
            if (xval % 3 != 0)
            {
                if (xval > 0) // x can be negative. It's -250..250
                    xval += xval % 3 == 1 ? 2 : 1;
                else         //if it's negative
                    xval -= xval % 3 == -1 ? 2 : 1;
            }
            if (yval % 3 != 0)
            {
                yval += yval % 3 == 1 ? 2 : 1;
            }
            int ri = Random.Range(1, 3);
            if(ri==1)
                aus.PlayOneShot(c2, 1f);
            else
                aus.PlayOneShot(c3, 1f);
            score++;
            if (pv.IsMine)
            {
                GameObject.FindGameObjectWithTag("numberone").GetComponent<Text>().text = score.ToString();
            }
            GameObject.FindGameObjectWithTag("bait").transform.position = new Vector3(xval, yval, 0);
        }
        #endregion
    }

    private void NormalMovement() 
    {
        aus.PlayOneShot(clip, 1f);
        this.transform.position = new Vector3(movHor, movVer, 0); // position implement
        //infotxt.text = transform.position.x.ToString() + "-" + transform.position.y.ToString(); //position info board
    }

}
