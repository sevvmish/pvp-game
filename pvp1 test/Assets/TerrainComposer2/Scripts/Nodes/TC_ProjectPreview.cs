using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_ProjectPreview : MonoBehaviour
	{
		static public TC_ProjectPreview instance;
		public Material matProjector;

		private void Awake()
		{
			if (instance == null) instance = this;
		}

		private void OnEnable()
		{
			if (instance == null) instance = this;
		}

		private void OnDestroy()
		{
			if (instance == this) instance = null;
		}

		public void SetPreview(TC_ItemBehaviour item)
		{
			matProjector.SetTexture("_MainTex", item.rtDisplay);
		}
	}
}
