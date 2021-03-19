using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_Randomizer : MonoBehaviour
    {
        public TC_ItemBehaviour item;
        public TC_RandomSettings r;
        public bool randomize;

        void Awake()
        {
            item = GetComponent<TC_ItemBehaviour>();
        }

        void Update()
        {
            if (randomize)
            {
                randomize = false;
                Randomize();
            }
        }

        void Randomize()
        {
            if (r == null || item == null) return;

            int amount = Random.Range(r.amount.x, r.amount.y);

            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = new Vector3(Random.Range(r.posX.x, r.posX.y), 0, Random.Range(r.posZ.x, r.posZ.y));
                float rotY = Random.Range(r.rotY.x, r.rotY.y);
                float scaleX = Random.Range(r.scaleX.x, r.scaleX.y);
                Vector3 scale = new Vector3(scaleX, Random.Range(r.scaleY.x, r.scaleY.y), scaleX);

                TC_ItemBehaviour newItem = item.Duplicate(item.t.parent);
                newItem.t.position = pos;
                newItem.t.rotation = Quaternion.Euler(0, rotY, 0);
                newItem.t.localScale = scale;
                newItem.method = Method.Max;
            }

            TC.AutoGenerate();
        }
    }
}
