using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_GroupBehaviour : TC_ItemBehaviour
    {
        public int firstActive, lastActive, totalActive;
        
        public void Clear(bool undo)
        {
            int index = 0;

            int length = t.childCount;

            for (int i = 0; i < length; i++)
            {
                Transform child = t.GetChild(index);
                TC_ItemBehaviour item = child.GetComponent<TC_ItemBehaviour>();
                if (item != null) item.DestroyMe(undo); 
                else index++;
            }
        }
    }
}