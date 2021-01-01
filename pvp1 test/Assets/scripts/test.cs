using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{

    TalentsButtonTest talent1;

    public class TalentsButtonTest : MonoBehaviour
    {
        private GameObject WholeButtonImage;
        private Button MainThemeImage;
        private TextMeshProUGUI TalentsNumbers;
        private int MaxTalents;
        private int CurrentTalents;


        public TalentsButtonTest(Sprite MainTheme, int CurrTalents, int MTalents, string TalentName, Vector2 coords)
        {
            WholeButtonImage = Instantiate(Resources.Load<GameObject>("prefabs/point"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            WholeButtonImage.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(coords.x, coords.y, 0);

            MainThemeImage = WholeButtonImage.transform.GetChild(0).GetComponent<Button>();
            MainThemeImage.image.sprite = MainTheme;
            MainThemeImage.name = TalentName;

            CurrentTalents = CurrTalents;
            MaxTalents = MTalents;

            TalentsNumbers = WholeButtonImage.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            GetCurrTalents();


        }

        public void MakeInactive()
        {
            MainThemeImage.interactable = false;
        }

        public void MakeActive()
        {
            MainThemeImage.interactable = true;
        }


        public void AddTalentPoint()
        {
            if (CurrentTalents == MaxTalents)
            {
                CurrentTalents = 0;
                GetCurrTalents();
            }
            else
            {
                CurrentTalents++;
                GetCurrTalents();
            }
        }

        public string GetCurrentTalentPointString()
        {
            return CurrentTalents.ToString();
        }

        private void GetCurrTalents()
        {
            TalentsNumbers.text = CurrentTalents + "/" + MaxTalents;
        }


    }

    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;

        talent1 = new TalentsButtonTest(DB.GetTalentByNumber(1).Talent_icon, 1, 3, "talent1", new Vector2(0,0));

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {


            if (EventSystem.current.currentSelectedGameObject.name != null)
            {
                print(EventSystem.current.currentSelectedGameObject.name);
                if (EventSystem.current.currentSelectedGameObject.name == "talent1")
                {
                    talent1.AddTalentPoint();
                }


            }
            else
            {
                
            }
        }
    }



}
