using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class players : MonoBehaviour
{
    
    public Transform PlayerTransform;    
    private AnimationsForPlayers PlayerAnimator;    
    private TextMeshProUGUI TempText1;
    public int NumberInSendAndReceive;
    public int previousstate;
    public string previouscondition;
    Vector3 speed = Vector3.zero;
    float ResultDelta, DeltaOnlyForRot;
    Vector3 CorrectionForPosition;    
    public effects PlayerEffects;

    public ConditionsAnalys Conds = new ConditionsAnalys();
    public PlayerUI OtherPlayerUI;

    public string OtherPlayerName;
    public bool isUIadded;

    float rotAngle, sighAngle;

    // Start is called before the first frame update
    void OnEnable()
    {        
        PlayerTransform = this.GetComponent<Transform>();
        PlayerAnimator = new AnimationsForPlayers(this.GetComponent<Animator>(), this.GetComponent<AudioSource>());
        TempText1 = GameObject.Find("OtherHP").GetComponent<TextMeshProUGUI>();
        PlayerTransform.position = Vector3.zero;
        PlayerEffects = this.GetComponent<effects>();

        
    }

    public void CreateUI()
    {
        GameObject temp2 = Instantiate(Resources.Load<GameObject>("prefabs/otherplayerUI 2"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("CanvasInterface").transform);
        temp2.name = OtherPlayerName;
        temp2.GetComponent<RectTransform>().anchoredPosition = new Vector2(970, -50 * (NumberInSendAndReceive+1) * 1.5f);
        temp2.SetActive(true);
        OtherPlayerUI = new PlayerUI(temp2, false);
        isUIadded = true;
        
    }

    public void SyncPosNRot(float DeltaForLerp, float AverageCount)
    {
        
        TempText1.text = SendAndReceive.OtherPlayerData[NumberInSendAndReceive].health_pool.ToString();
        if (isUIadded)
        {
            
            OtherPlayerUI.HealthInput(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].health_pool, SendAndReceive.OtherPlayerData[NumberInSendAndReceive].max_health_pool);
            OtherPlayerUI.EnergyInput(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].energy);
        }
        pos_n_rot(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].position, SendAndReceive.OtherPlayerData[NumberInSendAndReceive].rotation, DeltaForLerp, AverageCount);
        PlayerAnimator.RefreshAnimations(SendAndReceive.OtherPlayerData[NumberInSendAndReceive].animation_id);
        //print(NumberInSendAndReceive + " er");

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

        
    }

    
}
