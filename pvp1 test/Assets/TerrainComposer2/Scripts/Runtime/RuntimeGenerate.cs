using UnityEngine;


namespace TerrainComposer2
{
	public class RuntimeGenerate : MonoBehaviour
	{
		public bool generateOnStart = true;
		public bool generateOnUpdate;
		public bool generateHeight = true;
		public bool generateSplat = true;
		public bool generateColor = true;
		public bool generateGrass = true;
		public bool generateTrees = true;
		public bool generateObjects = true;

		void Start()
        {
			if (generateOnStart) Generate();
        }

		void Update()
		{
			if (generateOnUpdate) Generate();
		}

		public void Generate()
        {
			if (generateHeight) TC_Generate.instance.Generate(true, TC.heightOutput);
			if (generateSplat) TC_Generate.instance.Generate(true, TC.splatOutput);
			if (generateColor) TC_Generate.instance.Generate(true, TC.colorOutput);
			if (generateGrass) TC_Generate.instance.Generate(true, TC.grassOutput);
			if (generateTrees) TC_Generate.instance.Generate(true, TC.treeOutput);
			if (generateObjects) TC_Generate.instance.Generate(true, TC.objectOutput);
		}
	}
}
