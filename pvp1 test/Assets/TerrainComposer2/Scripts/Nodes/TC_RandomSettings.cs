using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    // [CreateAssetMenu(fileName = "TC_GlobalSettings", menuName = "TerrainComposer2/RandomSettings")]
    public class TC_RandomSettings : ScriptableObject
    {
        public Int2 amount = new Int2(10, 20);
        public Vector2 posX, posY, posZ, rotY, scaleX, scaleY, scaleZ;
    }
}