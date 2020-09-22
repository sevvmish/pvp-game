using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class players : MonoBehaviour
{    
    private Transform PlayerTransform;    
    private Vector3 PreviousPos;
    private Vector3 PreviousRot;
    private AnimationsForPlayers PlayerAnimator;
    private Vector3 position, rotation;
    private TextMeshProUGUI TempText1;
    public int NumberInSendAndReceive;
    public int previousstate;
    public string previouscondition;
    Vector3 speed = Vector3.zero;
    float ResultDelta, DeltaOnlyForRot;
    Vector3 CorrectionForPosition;
    Vector3 CorrectionForRotation;
    public effects PlayerEffects;

    public ConditionsAnalys Conds = new ConditionsAnalys();
    public PlayerUI OtherPlayerUI;

    float rotAngle, sighAngle;

    // Start is called before the first frame update
    void OnEnable()
    {        
        PlayerTransform = this.GetComponent<Transform>();
        PlayerAnimator = new AnimationsForPlayers(this.GetComponent<Animator>(), this.GetComponent<AudioSource>());
        TempText1 = GameObject.Find("OtherHP").GetComponent<TextMeshProUGUI>();
        PlayerTransform.position = new Vector3(0, 0.2f, 0);
        PlayerEffects = this.GetComponent<effects>();
    }

    public void SyncPosNRot(float DeltaForLerp, float AverageCount)
    {
        
        TempText1.text = SendAndReceive.OtherPlayerData[NumberInSendAndReceive].health_pool.ToString();
        pos_n_rot(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].position, SendAndReceive.OtherPlayerData[NumberInSendAndReceive].rotation, DeltaForLerp, AverageCount);
        PlayerAnimator.RefreshAnimations(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].animation_id);

        if (SendAndReceive.OtherPlayerData[NumberInSendAndReceive].conditions != previouscondition)
        {
            //print("enemy - " + SendAndReceive.OtherPlayerData[NumberInSendAndReceive].conditions);
            previouscondition = SendAndReceive.OtherPlayerData[NumberInSendAndReceive].conditions;
            Conds.Check(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].conditions);
        }
                
        previousstate = SendAndReceive.OtherPlayerData[NumberInSendAndReceive].animation_id;
        previouscondition = SendAndReceive.OtherPlayerData[NumberInSendAndReceive].conditions;
    }

    public void pos_n_rot(Vector3 pos, Vector3 rot, float Delta, float Average)
    {
        CorrectionForPosition = pos - PlayerTransform.position;
        

        if (SendAndReceive.OtherPlayerData[NumberInSendAndReceive].animation_id==0)
        {
            ResultDelta *= 0.7f;
            Delta *= ResultDelta;
        } else
        {
            ResultDelta = 1;
            Delta *= ResultDelta;
        }

        if (Delta == 0) Delta = 1;
        if (Average == 0) Average = 1;
        PlayerTransform.position = Vector3.SmoothDamp(PlayerTransform.position, (PlayerTransform.position + CorrectionForPosition / (Average)), ref speed, 0.02f);

        

        rotAngle = Quaternion.Angle(Quaternion.Euler(rot), PlayerTransform.rotation);

        if ((rot.y - PlayerTransform.rotation.eulerAngles.y) >= 0 && Mathf.Abs(rot.y - PlayerTransform.rotation.eulerAngles.y) < 180)
        {
            sighAngle = 1;
        }
        else if ((rot.y - PlayerTransform.rotation.eulerAngles.y) < 0 && Mathf.Abs(rot.y - PlayerTransform.rotation.eulerAngles.y) < 180)
        {
            sighAngle = -1;
        }
        else if ((rot.y - PlayerTransform.rotation.eulerAngles.y) >= 0 && Mathf.Abs(rot.y - PlayerTransform.rotation.eulerAngles.y) > 180)
        {
            sighAngle = -1;
        }
        else if ((rot.y - PlayerTransform.rotation.eulerAngles.y) < 0 && Mathf.Abs(rot.y - PlayerTransform.rotation.eulerAngles.y) > 180)
        {
            sighAngle = 1;
        }


        PlayerTransform.rotation = Quaternion.AngleAxis((PlayerTransform.rotation.eulerAngles.y + rotAngle * sighAngle / Average), Vector3.up);


        PreviousPos = pos;
        PreviousRot = rot;
        
        //StartCoroutine(MoveTo(PlayerTransform.position, PlayerTransform.rotation, pos,rot, delt));
    }

    /*
    IEnumerator MoveTo(Vector3 OldPos, Quaternion OldRot, Vector3 NewPos, Vector3 NewRot, float delta)
    {
        for (float i=0;i<delta;i++)
        {
            PlayerTransform.position = Vector3.Lerp(OldPos, NewPos, (i / (delta-1)));
            PlayerTransform.rotation = Quaternion.Lerp(OldRot, Quaternion.Euler(NewRot), (i / (delta - 1)));
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator MoveTo(Vector3 OldPos, Vector3 NewPos, float delta)
    {
        for (float i = 0; i < delta; i++)
        {
            PlayerTransform.position = Vector3.Lerp(OldPos, NewPos, (i / (delta - 1)));            
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator MoveTo(Quaternion OldRot, Vector3 NewRot, float delta)
    {
        for (float i = 0; i < delta; i++)
        {            
            PlayerTransform.rotation = Quaternion.Lerp(OldRot, Quaternion.Euler(NewRot), (i / (delta - 1)));
            yield return new WaitForSeconds(0.02f);
        }
    }
    */
}
