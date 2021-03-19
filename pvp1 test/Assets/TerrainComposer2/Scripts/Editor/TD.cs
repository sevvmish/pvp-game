using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace TerrainComposer2
{
	public enum GroupType { LayerGroupResult, NodeGroup, SelectItemGroup };
	static public class TD
	{
		static public List<Rect> rectSelectList = new List<Rect>();
		static public bool showSelectRect;
		static public Vector2 posClickMouseDown;
		static public int mouseDownButton;
		static public TC_ItemBehaviour dropItemReceive;
		static public bool startDrag;
		static public Event eventCurrent;

		static public int countDrawNode;
		static public int countDrawNodeCulled;
		static public int countDrawTextures;

		static public Rect rectWindow;
		static public Vector2 scrollOffset, newScrollOffset;
		static public bool setNewScrollOffset;
		static public float scale = 1;
		static public Vector2 scrollMax;

		static public bool tooltip;
		static public Rect rectTooltip;
		static public float tooltipStartTime;
		static public float tooltipTime;

		static public float labelHeight = 14;
		static public float mouseSensivity = 1;
		static public float mouseScrollWheelSensivity = 1;

		static public float outputBarTopHeight = 40;
		static public float outputBarHeightSpace = outputBarTopHeight + 2;
		static public float outputBarWidth = 5;
		static public float outputBarBottomHeight = 5;

		static public float layerGroupBarTopHeight = 30;
		static public float layerGroupBarWidth = 5;
		static public float layerGroupBarBottomHeight = 5;

		static public float layerBarTopHeight = 20;
		static public float layerBarWidth = 5;
		static public float layerBarBottomHeight = 5;

		static public float layerGroupHeight = 20;
		static public float layerGroupHeightSpace = layerGroupHeight + 2;
		// static public float nodeWidth = 75;
		//static public float nodeHeight = 120;
		static public float nodeWidthSpace = 20 + 3;
		static public float nodeHeightSpace = 20 + 3;
		static public float nodeBorderWidth = 3;
		
		static public Texture buttonLeft;
		static public Texture buttonRight;
		static public Texture buttonDown;
		static public Texture buttonUp;
		static public Texture enumBG_Start;
		static public Texture enumBG_Middle;
		static public Texture enumBG_End;
		static public Texture texFadeBar;
		static public Texture buttonMinus;
		static public Texture buttonPlus;
		static public Texture inActiveTexture, inActiveChildrenTexture, vertLineTexture;
		static public Texture[] outputButtons;
		 
		static public Texture texShelfBackground1, texShelfBackGround2, texConnectionIndicator;
		static public Texture texShelfLinesConnectDown, texShelfLinesConnectUp, texShelfLinesHorizontal, texShelfLinesVertical;
		static public Texture texShelfStart, texShelfStartConnect, texShelfStartOutput, texShelfStartOutputCollapsed;
		static public Texture texShelfLayer, texShelfLayerStart1, texShelfLayerStart2, texShelfLayerCollapsed, texShelfLayerCollapsedStart1, texShelfLayerCollapsedStart2;
		static public Texture texCardHeader, texCardBody, texBracketHeader, texBracketBody, texButton, texInwardButton;

		static public Texture texLineConnectDown, texLineConnectUp, texLineFirstLayer, texLineHorizontal, texLineLayerStart1, texLineLayerStart2, texLineLayerCollapsedStart1, texLineVertical;

		static public Texture texBracketRight, texBracketLeft, texCardCounter, texAddFirstCard;
		static public Texture texOperandBG, texOperandAdd, texOperandSubtract, texOperandLerp, texOperandMultiply, texOperandDivide, texOperandDiff, texOperandAverage, texOperandMin, texOperandMax, texOperandEqual;

		static public Texture texSeparatorLeft, texSeparatorCenter, texSeparatorRight;
		static public Texture texEye, texEyeClosed, texFoldout, texLocked, texUnlocked, texPortal;
		static public Texture texDragIconVertical, texDragIconHorizontal, texSelectCard, texSelectCardHeader;
		
		static public TC_Settings settings;
		static public TC_GlobalSettings g;

		static public TC_ItemBehaviour selectedOpacityItem;
		static public TC_ItemBehaviour hoverItem, hoverItemOld;
		static public float editorSkinMulti;

		static GUIContent guiContent = new GUIContent();

		public static GUIContent GetGUIContent(string text, Texture image = null, string tooltip = "")
		{
			guiContent.text = text;
			guiContent.image = image;
			guiContent.tooltip = tooltip;
			return guiContent;
		}

		public static GUIContent GetGUIContent(Texture image = null, string tooltip = "")
		{
			guiContent.text = string.Empty;
			guiContent.image = image;
			guiContent.tooltip = tooltip;
			return guiContent;
		}

		public static GUIContent GetGUIContent(string text, string tooltip)
		{
			guiContent.text = text;
			guiContent.image = null;
			guiContent.tooltip = tooltip;
			return guiContent;
		}

		public static GUIContent GetGUIContent(Texture image)
		{
			guiContent.text = guiContent.tooltip = string.Empty;
			guiContent.image = image;
			return guiContent;
		}

		static public bool Init()
		{
			if (TC_Settings.instance != null)
			{
				settings = TC_Settings.instance;
				g = settings.global;
			}
			else return false;

			if (TC.installPath == "") return false; 
			 
			editorSkinMulti = EditorGUIUtility.isProSkin ? 1 : 0.35f;

			if (texPortal == null)
			{
				buttonMinus = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/Button_Minus.psd", typeof(Texture));
				buttonPlus = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/Button_Plus.psd", typeof(Texture));
				buttonLeft = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/arrow_left.png", typeof(Texture));
				buttonRight = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/arrow_right.png", typeof(Texture));
				buttonDown = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/arrow_bottom.png", typeof(Texture));
				buttonUp = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/arrow_top.png", typeof(Texture));
				enumBG_Start = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/EnumBG_Start.png", typeof(Texture));
				enumBG_Middle = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/EnumBG_Middle.png", typeof(Texture));
				enumBG_End = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/EnumBG_End.png", typeof(Texture));
				texFadeBar = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/FadeBar.psd", typeof(Texture));

				if (outputButtons == null) outputButtons = new Texture[6];
				outputButtons[0] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_heightmap.png", typeof(Texture));
				outputButtons[1] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_colormap.png", typeof(Texture));
				outputButtons[2] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_splatmap.png", typeof(Texture));
				outputButtons[3] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_tree.png", typeof(Texture));
				outputButtons[4] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_grass.png", typeof(Texture));
				outputButtons[5] = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_objects.png", typeof(Texture));

				inActiveTexture = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/InActivePreview.psd", typeof(Texture));
				inActiveChildrenTexture = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/InActiveChildrenPreview.psd", typeof(Texture));
				vertLineTexture = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/GradientLine2.psd", typeof(Texture));

				texCardBody = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Card_Body.png", typeof(Texture));
				texCardHeader = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Card_Header.png", typeof(Texture));
				texBracketHeader = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Bracket_Header.png", typeof(Texture));
				texBracketBody = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Bracket_Body.png", typeof(Texture));

				texBracketLeft = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Bracket_Left.png", typeof(Texture));
				texBracketRight = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Bracket_Right.png", typeof(Texture));

				texButton = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Button.png", typeof(Texture));
				texInwardButton = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/InwardButton.png", typeof(Texture));

				texOperandBG = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandBG.png", typeof(Texture));
				texOperandAdd = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandAdd.png", typeof(Texture));
				texOperandSubtract = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandSubtract.png", typeof(Texture));
				texOperandLerp = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandLerp.png", typeof(Texture));
				texOperandMultiply = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandMultiply.png", typeof(Texture));
				texOperandDivide = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandDivide.png", typeof(Texture));
				texOperandDiff = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandDiff.png", typeof(Texture));
				texOperandAverage = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandAverage.png", typeof(Texture));
				texOperandMin = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandMin.png", typeof(Texture));
				texOperandMax = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandMax.png", typeof(Texture));
				texOperandEqual = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/OperandEqual.png", typeof(Texture));

				texEye = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Eye.png", typeof(Texture));
				texEyeClosed = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/EyeClosed.png", typeof(Texture));
				texFoldout = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Foldout.png", typeof(Texture));
				texLocked = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Lock_Closed.png", typeof(Texture));
				texUnlocked = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Lock_Open.png", typeof(Texture));
				texPortal = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Portal.png", typeof(Texture));

				string shelfFolder = "Fixed/";
				// string shelfFolder = "";

				texShelfBackground1 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Background1.png", typeof(Texture));
				texShelfBackGround2 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Background2.png", typeof(Texture));
				texConnectionIndicator = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/ConnectionIndicator.png", typeof(Texture));

				texSeparatorLeft = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Separator_Left.png", typeof(Texture));
				texSeparatorCenter = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Separator_Center.png", typeof(Texture));
				texSeparatorRight = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Separator_Right.png", typeof(Texture));

				texCardCounter = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/CardCounter.png", typeof(Texture));
				texAddFirstCard = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/AddFirstCard.png", typeof(Texture));

				texShelfStart = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_TreeStart/" + shelfFolder + "Shelf_Start.png", typeof(Texture));
				texShelfStartConnect = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_TreeStart/" + shelfFolder + "Shelf_Start_Connect.png", typeof(Texture));
				texShelfStartOutput = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_TreeStart/" + shelfFolder + "Shelf_Start_FirstLayer.png", typeof(Texture));
				texShelfStartOutputCollapsed = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_TreeStart/" + shelfFolder + "Shelf_Start_FirstLayer_Collapsed.png", typeof(Texture));

				texShelfLayer = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_Layer.png", typeof(Texture));
				texShelfLayerStart1 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_Layer_Start1.png", typeof(Texture));
				texShelfLayerStart2 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_Layer_Start2.png", typeof(Texture));
				texShelfLayerCollapsed = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_LayerCollapsed.png", typeof(Texture));
				texShelfLayerCollapsedStart1 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_LayerCollapsed_Start1.png", typeof(Texture));
				texShelfLayerCollapsedStart2 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Layer/" + shelfFolder + "Shelf_LayerCollapsed_Start2.png", typeof(Texture));

				texShelfLinesConnectDown = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Lines/" + shelfFolder + "Shelf_Lines_ConnectDown.png", typeof(Texture));
				texShelfLinesConnectUp = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Lines/" + shelfFolder + "Shelf_Lines_ConnectUp.png", typeof(Texture));
				texShelfLinesHorizontal = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Lines/" + shelfFolder + "Shelf_Lines_Horizontal.png", typeof(Texture));
				texShelfLinesVertical = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Shelf/Shelf_Lines/" + shelfFolder + "Shelf_Lines_Vertical.png", typeof(Texture));

				texLineConnectDown = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_ConnectDown.png", typeof(Texture));
				texLineConnectUp = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_ConnectUp.png", typeof(Texture));
				texLineFirstLayer = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_FirstLayer.png", typeof(Texture));
				texLineHorizontal = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_Horizontal.png", typeof(Texture));
				texLineLayerStart1 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_Layer_Start1.png", typeof(Texture));
				texLineLayerStart2 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_Layer_Start2.png", typeof(Texture));
				texLineLayerCollapsedStart1 = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_LayerCollapsed_Start1.png", typeof(Texture));
				texLineVertical = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Lines/Line_Vertical.png", typeof(Texture));

				texDragIconVertical = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/DragIconVertical.png", typeof(Texture));
				texDragIconHorizontal = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/DragIconHorizontal.png", typeof(Texture));

				texSelectCard = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Select_Card.png", typeof(Texture));
				texSelectCardHeader = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Cards/Select_Card_Header.png", typeof(Texture));

				if (texCardHeader == null || texCardBody == null || g == null) return false;

				cardHeight = texCardHeader.height + texCardBody.height;
				nodeWidthHSpace = texCardBody.width + g.nodeHSpace;
				shelfOffsetY = (texShelfLayer.height - cardHeight) / 2;
			}
			return true;
		}

		static public float cardHeight;
		static public float shelfOffsetY;
		static public float nodeWidthHSpace;

		static public bool SelectionContainsItemBehaviour()
		{
			for (int i = 0; i < Selection.transforms.Length; i++) if (Selection.transforms[i].GetComponent<TC_ItemBehaviour>() != null) return true;
			return false;
		}

		static public float GetPosX(float offset)
		{
			float center = TC_NodeWindow.window.position.width / 2;
			offset += scrollOffset.x;
			return (scale * (offset - center)) + center;
		}

		static public Vector2 GetPositionScaled(Vector2 offset)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			offset += scrollOffset;
			return (scale * (offset - center)) + center;
		}

		static public Rect GetRectScaled(Rect rect)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			rect.x += scrollOffset.x;
			rect.y += scrollOffset.y;

			rect.x = (scale * (rect.x - center.x)) + center.x;
			rect.y = (scale * (rect.y - center.y)) + center.y;
			rect.width *= scale;
			rect.height *= scale;
			return rect;
		}

		static public Rect GetRectScaled(float x, float y, float width, float height)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			x += scrollOffset.x;
			y += scrollOffset.y;

			x = (scale * (x - center.x)) + center.x;
			y = (scale * (y - center.y)) + center.y;
			width *= scale;
			height *= scale;
			return new Rect(x, y, width, height);
		}

		static public Rect GetRectScaled(float x, float y, Texture tex, bool invert = false)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			x += scrollOffset.x;
			y += scrollOffset.y;

			x = (scale * (x - center.x)) + center.x;
			y = (scale * (y - center.y)) + center.y;
			float width = tex.width * scale;
			float height = tex.height * scale;
			if (!invert) return new Rect(x, y, width, height);
			else return new Rect(x + width, y, -width, height);
		}

		static public void DrawTexture(Rect rect, Texture tex, string tooltip, Color color)
		{
			GUI.color = color;
			GUI.DrawTexture(rect, tex);
			GUI.Label(rect, GetGUIContent(string.Empty, tooltip));
			GUI.color = Color.white;
		}

		static public Rect DrawTextureScaled(Rect rect, Texture tex, string tooltip, Color color)
		{
			rect = GetRectScaled(rect);
			GUI.color = color;
			GUI.DrawTexture(rect, tex);
			GUI.Label(rect, GetGUIContent(string.Empty, tooltip));
			GUI.color = Color.white;
			return rect;
		}

		static public Rect DrawTextureScaled(float x, float y, Texture tex, Color color, bool invert = false, StretchMode stretchHorizontal = StretchMode.None, StretchMode stretchVertical = StretchMode.None)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			float width, height;

			if (stretchHorizontal == StretchMode.Screen) { x = 0; width = TC_NodeWindow.window.position.width; }
			else
			{
				x += scrollOffset.x;
				x = (scale * (x - center.x)) + center.x;
				width = tex.width * scale;

				if (stretchHorizontal == StretchMode.Left) { width += x; x = 0; }
				else if (stretchHorizontal == StretchMode.Right) { width = TC_NodeWindow.window.position.width - x; }
			}
			
			if (stretchVertical == StretchMode.Screen) { y = 0; height = TC_NodeWindow.window.position.height; }
			else
			{
				y += scrollOffset.y;
				y = (scale * (y - center.y)) + center.y;
				height = tex.height * scale;

				if (stretchVertical == StretchMode.Left) { height += y; y = 0; }
				else if (stretchVertical == StretchMode.Right) { height = TC_NodeWindow.window.position.height - y; }
			}
			Rect rect;
			if (!invert) rect = new Rect(x, y, width, height); 
			else rect = new Rect(x + width, y, -width, height);
			// if (tex == null) return rect;

			GUI.color = color;
			GUI.DrawTexture(rect, tex);
			GUI.color = Color.white;

			return rect;
		}

		static public Rect DrawTextureScaled(float x, float y, float width, Texture tex, Color color, bool invert = false, StretchMode stretchVertical = StretchMode.None)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			float height;
			
			x += scrollOffset.x;
			x = (scale * (x - center.x)) + center.x;
			width *= scale;
			
			if (stretchVertical == StretchMode.Screen) { y = 0; height = TC_NodeWindow.window.position.height; }
			else
			{
				y += scrollOffset.y;
				y = (scale * (y - center.y)) + center.y;
				height = tex.height * scale;

				if (stretchVertical == StretchMode.Left) { height += y; y = 0; }
				else if (stretchVertical == StretchMode.Right) { height = TC_NodeWindow.window.position.height - y; }
			}
			Rect rect;
			if (!invert) rect = new Rect(x, y, width, height);
			else rect = new Rect(x + width, y, -width, height);

			GUI.color = color;
			GUI.DrawTexture(rect, tex);
			GUI.color = Color.white;

			return rect;
		}

		static public Rect DrawTextureScaledV(float x, float y, float height, Texture tex, Color color, bool invert = false, StretchMode stretchHorizontal = StretchMode.None)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			float width;

			if (stretchHorizontal == StretchMode.Screen) { x = 0; width = TC_NodeWindow.window.position.width; }
			else
			{
				x += scrollOffset.x;
				x = (scale * (x - center.x)) + center.x;
				width = tex.width * scale;

				if (stretchHorizontal == StretchMode.Left) { width += x; x = 0; }
				else if (stretchHorizontal == StretchMode.Right) { width = TC_NodeWindow.window.position.width - x; }
			}
			
			y += scrollOffset.y;
			y = (scale * (y - center.y)) + center.y;
			height *= scale;
			
			Rect rect;
			if (!invert) rect = new Rect(x, y, width, height);
			else rect = new Rect(x + width, y, -width, height);

			GUI.color = color;
			GUI.DrawTexture(rect, tex);
			GUI.color = Color.white;

			return rect;
		}

		static public Rect GetLeftRect(Rect rect)
		{
			Vector2 center = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);
			// rect.width += scrollOffset.x;
			rect.y += scrollOffset.y;

			rect.width = (scale * ((rect.width + scrollOffset.x) - center.x)) + center.x;
			rect.y = (scale * (rect.y - center.y)) + center.y;
			// rect.width *= scale;
			rect.height *= scale;
			return rect;
		}

		static public bool CullRect(Rect window, Rect rect)
		{
			if (window.Contains(new Vector2(rect.xMin, rect.yMin)) || window.Contains(new Vector2(rect.xMin, rect.yMax)) || window.Contains(new Vector2(rect.xMax, rect.yMin)) || window.Contains(new Vector2(rect.xMax, rect.yMax))) return false; else return true;
		}

		static public void DrawCenter(Color color, int length, int width)
		{
			GUI.color = color;
			GUI.DrawTexture(new Rect((TC_NodeWindow.window.position.width - length) / 2, (TC_NodeWindow.window.position.height - width) / 2, length, width), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect((TC_NodeWindow.window.position.width - width) / 2, (TC_NodeWindow.window.position.height - length) / 2, width, length), Texture2D.whiteTexture);
			GUI.color = Color.white;
		}

		static public Rect DrawRect(Rect rect, Color color, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign verTextAlign = VerTextAlign.Top)
		{
			Color colOld = GUI.color;
			GUI.color = color;
			rect = GetRectScaled(rect);
			AlignPosition(ref rect, horTextAlign, verTextAlign);
			// if (CullRect (windowRect,rect)) return;
			GUI.DrawTexture(rect, Texture2D.whiteTexture);
			GUI.color = colOld;
			return rect;
		}

		static public Rect DrawRect(Rect rect, Texture tex, Color color)
		{
			Color colOld = GUI.color;
			GUI.color = color;
			rect = GetRectScaled(rect);
			// if (CullRect (windowRect,rect)) return;
			GUI.DrawTexture(rect, tex);
			GUI.color = colOld;
			return rect;
		}

		static public void DrawLeftRect(Rect rect, Texture tex, Color color)
		{
			// rect = GetRect (new Rect(0,0,400,50));
			GUI.color = color;
			rect = GetLeftRect(rect);
			if (rect.width > TC_NodeWindow.window.position.width) rect.width = TC_NodeWindow.window.position.width;
			GUI.DrawTexture(rect, tex); 
		}
		
		static public void DragDropNode(TC_ItemBehaviour item, Rect rectStartDrag, Rect rectDrop, Vector2 pos, bool isDragable = true, bool checkForPosDrop = true)
		{
			if (selectedOpacityItem != null) return;
			
			Vector2 mousePos = eventCurrent.mousePosition;

			if (item.GetType() == typeof(TC_LayerGroupResult)) isDragable = false;

			if (isDragable) {
				if (rectStartDrag.Contains(mousePos) && eventCurrent.button == 0)
				{
					if (eventCurrent.type == EventType.MouseDrag)
					{
						startDrag = true;
						//if (DragAndDrop.objectReferences.Length == 1)
						//{
						//    if (DragAndDrop.objectReferences[0] == item.gameObject)
						//    {
						//        startDrag = false;
						//    }
						//}

						// if (DragAndDrop.objectReferences.Length == 0)
						{
							// Debug.Log("Drag " + item.name);
							if (Selection.activeGameObject == item.gameObject)
							{
								DragAndDrop.PrepareStartDrag();
								// DragAndDrop.activeControlID = item.gameObject.GetInstanceID();
								// DragAndDrop.SetGenericData("Node", item.gameObject);
								DragAndDrop.objectReferences = new Object[1] { item.gameObject };// { null };
								// DragAndDrop.SetGenericData("currentValue", item.gameObject);
								
								DragAndDrop.StartDrag(item.name); 
								eventCurrent.Use();
							}
						}
					}
				}
			}
			
			if (rectDrop.Contains(mousePos) && eventCurrent.button == 0)
			{
				bool drop = eventCurrent.type == EventType.DragPerform;

				if (drop || eventCurrent.type == EventType.DragUpdated)
				{
					// Debug.Log("Accept Drag " + item.name);
					DragAndDrop.AcceptDrag();
					Object[] objs = DragAndDrop.objectReferences;

					for (int i = 0; i < objs.Length; i++)
					{
						Object obj = objs[i];
						if (obj.GetType() == typeof(Texture2D)) DropTexture(item, (Texture2D)obj);
						else if (obj.GetType() == typeof(GameObject))
						{
							TC_ItemBehaviour itemToDrop = ((GameObject)obj).GetComponent<TC_ItemBehaviour>();
							if (itemToDrop != null)
							{
								dropItemReceive = item;

								bool dropAccept = DropItem(item, itemToDrop, rectDrop, drop);

								if (dropAccept && !drop)
								{
									if (GetItemDropPosition(item, itemToDrop, rectDrop))
									{
										if (eventCurrent.alt) DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
										else DragAndDrop.visualMode = DragAndDropVisualMode.Move;
									}
								}
							}
							else DropObject(item, (GameObject)obj);
						}
					}
				}
			}
			else
			{
				if (checkForPosDrop) item.dropPosition = DropPosition.None;
			}

			if (item.dropPosition != DropPosition.None && checkForPosDrop) DrawItemDropPosition(item.dropPosition, pos);
		}

		static public void DrawItemDropPosition(DropPosition posDrop, Vector2 pos)
		{
			Color color;

			if (eventCurrent.shift) color = new Color(0, 1, 0, 0.75f);
			else color = new Color(1, 1, 1, 0.75f);
			
			if (posDrop == DropPosition.Top)
			   DrawTexture(GetRectScaled(new Rect(pos.x + g.rect7.x, pos.y + g.rect7.y, texDragIconVertical.width / 2f, texDragIconVertical.height / 2f)), texDragIconVertical, string.Empty, color);
			else if (posDrop == DropPosition.Bottom)
				DrawCommand.Add(new Rect(pos.x + g.rect8.x, pos.y + g.rect8.y, texDragIconVertical.width / 2f, -texDragIconVertical.height / 2f), texDragIconVertical, string.Empty, color, 2);
			else if (posDrop == DropPosition.Right)
				DrawCommand.Add(new Rect(pos.x + g.rect5.x, pos.y + g.rect5.y, texDragIconHorizontal.width / 2f, texDragIconHorizontal.height / 2f), texDragIconHorizontal, string.Empty, color, 2);
			else if (posDrop == DropPosition.Left)
				DrawCommand.Add(new Rect(pos.x + g.rect6.x, pos.y + g.rect5.y, -texDragIconHorizontal.width / 2f, texDragIconHorizontal.height / 2f), texDragIconHorizontal, string.Empty, color, 2);
		}

		static public TC_ItemBehaviour GetPrevious(TC_ItemBehaviour item, bool isPrefab)
		{
			if (isPrefab) return null;
			if (item.parentItem == null) return null;
			int siblingIndex = item.t.GetSiblingIndex();
			if (siblingIndex <= 0) return null;
			// Debug.Log(siblingIndex + " " + (siblingIndex - 1)+ " "+item.parentItem.t.childCount);

			if (siblingIndex > item.parentItem.t.childCount) return null;
			

			return item.parentItem.t.GetChild(siblingIndex - 1).GetComponent<TC_ItemBehaviour>();
		}

		static public TC_ItemBehaviour GetNext(TC_ItemBehaviour item, bool isPrefab)
		{
			if (isPrefab) return null;
			if (item.transform.parent == null) return null;
			int siblingIndex = item.t.GetSiblingIndex();
			if (siblingIndex == item.parentItem.t.childCount - 1) return null;
			// Debug.Log(siblingIndex + " " + (siblingIndex + 1)+ " "+item.parentItem.t.childCount);
			return item.parentItem.t.GetChild(siblingIndex + 1).GetComponent<TC_ItemBehaviour>();
		}

        static public bool IsPrefab(UnityEngine.Object obj)
        {
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                return (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab);
            #else
                return (PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab);
            #endif
        }

        static public void CreatePrefab(string path, GameObject go)
        {
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                PrefabUtility.CreatePrefab(path, go);
            #else
                PrefabUtility.SaveAsPrefabAsset(go, path);
            #endif
        }

		static public bool GetItemDropPosition(TC_ItemBehaviour itemReceive, TC_ItemBehaviour itemToDrop, Rect rectNode)
		{
			Vector2 mousePos = eventCurrent.mousePosition;
			bool clone = eventCurrent.alt;
            bool isPrefab = IsPrefab(itemToDrop);

			if (!itemReceive.nodeFoldout)
			{
				itemReceive.dropPosition = DropPosition.None;
				return false;
			}

			if (itemReceive.GetType() == typeof(TC_LayerGroupResult))
			{
				if (itemToDrop.GetType() == typeof(TC_LayerGroup) || itemToDrop.GetType() == typeof(TC_Layer))
				{
					itemReceive.dropPosition = DropPosition.Bottom;
					return true;
				}
				return false;
			}

			if (itemReceive.GetType() == typeof(TC_LayerGroup) || itemReceive.GetType() == typeof(TC_Layer))
			{
				TC_LayerGroup layerGroup = itemReceive as TC_LayerGroup;
				if (layerGroup != null)
				{
					if (layerGroup.level == 0) return false;
				}
				// Debug.Log(GetPrevious(itemToDrop).name + " " + itemReceive.name);
				if (mousePos.y < rectNode.y + ((cardHeight / 2) * scale))
				{
					// Debug.Log(GetPrevious(itemToDrop).name + " " + itemReceive.name);
					if (GetNext(itemToDrop, isPrefab) == itemReceive && !clone) { itemReceive.dropPosition = DropPosition.Bottom; }
					else itemReceive.dropPosition = DropPosition.Top;
				}
				else
				{
					if (GetPrevious(itemToDrop, isPrefab) == itemReceive && !clone) { itemReceive.dropPosition = DropPosition.Top; }
					else itemReceive.dropPosition = DropPosition.Bottom;
				}
			}
			else
			{
				bool nodeCondition = ((itemToDrop.GetType() == typeof(TC_Node) || itemToDrop.GetType() == typeof(TC_NodeGroup))  && itemReceive.GetType() == typeof(TC_NodeGroup));
				bool selectItemCondition = ((itemToDrop.GetType() == typeof(TC_SelectItem) || itemToDrop.GetType() == typeof(TC_SelectItemGroup)) && itemReceive.GetType() == typeof(TC_SelectItemGroup));

				if (nodeCondition || selectItemCondition)
				{
					if (itemReceive.t.childCount > 0 && !clone)
					{
						if (itemToDrop == itemReceive.t.GetChild(0).GetComponent<TC_ItemBehaviour>()) return false;
					}
					if (itemReceive.t.childCount == 0) return true;
					
					itemReceive.dropPosition = DropPosition.Left; return true;
				}

				if (mousePos.x < rectNode.x + ((texCardBody.width / 2) * scale))
				{
					if (GetPrevious(itemToDrop, isPrefab) == itemReceive && !clone) { itemReceive.dropPosition = DropPosition.Right; }
					else itemReceive.dropPosition = DropPosition.Left;
				}
				else
				{
					if (GetNext(itemToDrop, isPrefab) == itemReceive && !clone) { itemReceive.dropPosition = DropPosition.Left; }
					else itemReceive.dropPosition = DropPosition.Right;
				}
			}

			return true;
		}

		static public bool DropItem(TC_ItemBehaviour itemReceive, TC_ItemBehaviour itemToDrop, Rect rectDrop, bool drop)
		{
			if (itemReceive == itemToDrop) return false;

			bool isPrefab = IsPrefab(itemToDrop);

			if (itemToDrop.level == 0 && !isPrefab) return false;
			
			bool clone = eventCurrent.alt;
			
			if (itemToDrop.GetType() == typeof(TC_LayerGroup))
			{
				if (itemReceive.t.IsChildOf(itemToDrop.t)) return false;

				if (itemReceive.GetType() == typeof(TC_Layer))
				{
					if (drop) DropItemSameLevel(itemReceive, itemToDrop, clone);
					return true;
				}
				if (itemReceive.GetType() == typeof(TC_LayerGroup))
				{
					if (drop) DropItemSameLevel(itemReceive, itemToDrop, clone);
					return true;
				}
				if (itemReceive.GetType() == typeof(TC_LayerGroupResult))
				{
					if (drop) DropItemAsChild(itemReceive, itemToDrop, 0, clone);
					return true;
				}
			}

			else if (itemToDrop.GetType() == typeof(TC_Layer))
			{
				if (itemReceive.GetType() == typeof(TC_Layer) || itemReceive.GetType() == typeof(TC_LayerGroup) && itemReceive.level != 0)
				{
					if (drop) DropItemSameLevel(itemReceive, itemToDrop, clone);
					return true;
				}
				if (itemReceive.GetType() == typeof(TC_LayerGroupResult))
				{
					if (drop) DropItemAsChild(itemReceive, itemToDrop, 0, clone);
					return true;
				}
			}

			else if (itemToDrop.GetType() == typeof(TC_NodeGroup))
			{
				if (itemReceive.GetType() == typeof(TC_NodeGroup) || itemReceive.GetType() == typeof(TC_Node))
				{
					if (drop) DropGroup(itemReceive, itemToDrop, clone);
					return true;
				}
				else return false;
			}

			else if (itemToDrop.GetType() == typeof(TC_Node))
			{
				if (itemReceive.GetType() == typeof(TC_Node))
				{
					if (drop) DropItemSameLevel(itemReceive, itemToDrop, clone);
					return true;
				}
				if (itemReceive.GetType() == typeof(TC_NodeGroup))
				{
					if (drop) DropItemAsChild(itemReceive, itemToDrop, 0, clone);
					return true;
				}
			}

			else if (itemToDrop.GetType() == typeof(TC_SelectItemGroup))
			{
				if (itemReceive.GetType() == typeof(TC_SelectItemGroup) || itemReceive.GetType() == typeof(TC_SelectItem))
				{
					if (drop) DropGroup(itemReceive, itemToDrop, clone);
					return true;
				}
				else return false;
			}
			
			else if (itemToDrop.GetType() == typeof(TC_SelectItem))
			{
				if (itemReceive.GetType() == typeof(TC_SelectItem))
				{
					if (drop) DropItemSameLevel(itemReceive, itemToDrop, clone);
					return true;
				}
				if (itemReceive.GetType() == typeof(TC_SelectItemGroup))
				{
					if (drop) DropItemAsChild(itemReceive, itemToDrop, 0, clone);
					return true;
				}
			}
			return false;
		}

		static public void DropGroup(TC_ItemBehaviour itemReceive, TC_ItemBehaviour groupToDrop, bool clone)
		{
			TC_SelectItemGroup selectItemGroupReceive = itemReceive as TC_SelectItemGroup;
			TC_NodeGroup nodeGroupReceive = itemReceive as TC_NodeGroup;

			int startIndex;
			int indexOffset = 0;

			if (itemReceive.dropPosition == DropPosition.Right) { indexOffset = -1; }

			if (selectItemGroupReceive != null || nodeGroupReceive != null) startIndex = 0;
			else
			{
				startIndex = itemReceive.t.GetSiblingIndex() + 1;
				itemReceive = itemReceive.parentItem;
			}

			startIndex += indexOffset;
			
			int childCount = groupToDrop.t.childCount;

			for (int i = 0; i < childCount; i++)
			{
				Transform child;
				if (clone) child = groupToDrop.t.GetChild(childCount - i - 1);
				else child = groupToDrop.t.GetChild(groupToDrop.t.childCount - 1);

				TC_ItemBehaviour itemToDrop = child.GetComponent<TC_ItemBehaviour>();

				if (clone) itemToDrop = itemToDrop.Duplicate(itemReceive.transform);
				else itemToDrop.transform.parent = itemReceive.transform;

				itemToDrop.transform.SetSiblingIndex(startIndex);
			}
		}

		static public TC_ItemBehaviour DropItemSameLevel(TC_ItemBehaviour itemReceive, TC_ItemBehaviour itemToDrop, bool clone, bool checkForPrefab = true)
		{
			if (!clone && checkForPrefab)
			{
				if (IsPrefab(itemToDrop)) clone = true;
			}
			
			TC_SelectItemGroup selectItemGroupReceive = itemReceive as TC_SelectItemGroup;
			TC_SelectItem selectItemReceive = itemReceive as TC_SelectItem;

			// TC_SelectItemGroup selectItemGroupToDrop = itemToDrop as TC_SelectItemGroup;
			TC_SelectItem selectItemToDrop = itemToDrop as TC_SelectItem;

			bool restoreRanges = false;
			Vector2[] ranges = null;
			
			if (clone) itemToDrop = itemToDrop.Duplicate(itemReceive.t.parent);
			else
			{
				if (itemReceive.parentItem == itemToDrop.parentItem && selectItemReceive != null && selectItemToDrop != null)
				{
					ranges = selectItemReceive.parentItem.GetRanges();
					restoreRanges = true;
				}
			}

			Undo.SetTransformParent(itemToDrop.transform, itemReceive.transform.parent, "Drag and Drop "+itemToDrop.name);
			itemToDrop.t.SetAsLastSibling();

			int siblingIndex = itemReceive.t.GetSiblingIndex();
			if (itemReceive.dropPosition == DropPosition.Bottom) siblingIndex++;
			else if (itemReceive.dropPosition == DropPosition.Left) siblingIndex++;
			
			itemToDrop.t.SetSiblingIndex(siblingIndex);

			//if (!restoreRanges)
			//{
			//    if (selectItemToDrop != null)
			//    {
			//        selectItemToDrop.parentItem.GetItems();
			//        selectItemToDrop.range = selectItemToDrop.parentItem.GetInbetweenRange(itemToDrop.t.GetSiblingIndex());
			//    }
			//}

			if (restoreRanges)
			{
				// selectItemReceive.parentItem.GetItems(false);
				TC_Area2D.instance.terrainLayer.GetItem(selectItemReceive.outputId, true, false);
				selectItemReceive.parentItem.SetRanges(ranges);
			}
			else if (selectItemToDrop != null)
			{
				if (selectItemToDrop.parentItem != null) selectItemToDrop.parentItem.refreshRanges = true;
			}

			if (selectItemGroupReceive != null) selectItemGroupReceive.refreshRanges = true;
			if (selectItemReceive != null) selectItemReceive.parentItem.refreshRanges = true;

			TC.RefreshOutputReferences((itemReceive.outputId == itemToDrop.outputId) ? itemReceive.outputId : TC.allOutput);
			CheckDropInDifferentLevel(itemReceive, itemToDrop);
			TC.AutoGenerate();

			return itemToDrop;
		}

		static public void DropItemAsChild(TC_ItemBehaviour itemReceive, TC_ItemBehaviour itemToDrop, int startIndex, bool clone)
		{
            if (IsPrefab(itemToDrop)) clone = true;
			if (clone) itemToDrop = itemToDrop.Duplicate(null);

			Undo.SetTransformParent(itemToDrop.transform, itemReceive.transform, "Move " + itemToDrop.name);
			itemToDrop.transform.SetSiblingIndex(startIndex);

			TC.RefreshOutputReferences((itemReceive.outputId == itemToDrop.outputId) ? itemReceive.outputId : TC.allOutput);
			CheckDropInDifferentLevel(itemReceive, itemToDrop);
			TC.AutoGenerate();
		}

		static public void CheckDropInDifferentLevel(TC_ItemBehaviour itemReceive, TC_ItemBehaviour dropItem)
		{
			if (itemReceive.outputId != dropItem.outputId && itemReceive.outputId != TC.heightOutput)
			{
				TC_LayerGroup layerGroup = dropItem as TC_LayerGroup;
				TC_Layer layer = dropItem as TC_Layer;

				if (layerGroup != null) { layerGroup.outputId = itemReceive.outputId; TC_Area2D.instance.terrainLayer.GetItem(layerGroup.outputId, true, false); }
				else if (layer != null) { layer.outputId = itemReceive.outputId; TC_Area2D.instance.terrainLayer.GetItem(layer.outputId, true, false); } 
			}
		}

		static public void DropTexture(TC_ItemBehaviour itemReceive, Texture2D texToDrop)
		{
			// Debug.Log("Drop Texture");
			TC_Node node = itemReceive as TC_Node;

			if (node != null)
			{
				if (eventCurrent.type == EventType.DragPerform)
				{
					node.inputKind = InputKind.File;
					node.inputFile = InputFile.RawImage;
					node.clamp = true;
					node.DropTextureEditor(texToDrop);
					Selection.activeTransform = node.t;
				}
				else DragAndDrop.visualMode = DragAndDropVisualMode.Move;
			}
		}

		static public void DropObject(TC_ItemBehaviour itemReceive, GameObject spawnGO)
		{
			TC_SelectItem selectItem = itemReceive as TC_SelectItem;

			if (selectItem != null && itemReceive.outputId == TC.objectOutput)
			{
				if (eventCurrent.type == EventType.DragPerform)
				{
					selectItem.spawnObject.go = spawnGO;
					selectItem.Refresh();
					Selection.activeTransform = selectItem.t;
				}
				else DragAndDrop.visualMode = DragAndDropVisualMode.Move;
			}
		}

		static public bool InScreenSpace(Rect rect)
		{
			// rect = GetRectScaled(rect);

			if (rect.xMax >= 0 && rect.yMax >= 0 && rect.xMin < TC_NodeWindow.window.position.width && rect.yMin < TC_NodeWindow.window.position.height) return true; else return false;
		}

		static public Rect DrawNode(TC_ItemBehaviour item, Vector2 pos, Color color, Color previewColor, ref bool isCulled, float activeMulti, bool nodeFoldout, bool drawMethod, bool resultNode = false, bool drawPreview = true)
		{
			TC_LayerGroup layerGroup = item as TC_LayerGroup;
			TC_LayerGroupResult groupResult = item as TC_LayerGroupResult;
			TC_Layer layer = item as TC_Layer;
			TC_NodeGroup nodeGroup = item as TC_NodeGroup;
			TC_Node node = item as TC_Node;
			TC_SelectItemGroup selectItemGroup = item as TC_SelectItemGroup;
			TC_SelectItem selectItem = item as TC_SelectItem;
			
			int mouseButton;

			if (!item.active) activeMulti *= 0.75f;

			Rect rectNode = GetRectScaled(pos.x, pos.y, texCardBody.width, nodeFoldout ? cardHeight : 32);

			if (!InScreenSpace(rectNode))
			{
				++countDrawNodeCulled;
				isCulled = true; return rectNode;
			}

			++countDrawNode;

			if (rectNode.Contains(eventCurrent.mousePosition)) hoverItem = item;

			if (Mathw.ArrayContains(Selection.transforms, item.t)) item.Repaint();

			// Rect nodeHeaderRect = GetRect(rect);
			// DrawRect(new Rect(rect.x, rect.y, rect.width, nodeHeaderHeight), new Color32(56, 56, 56, 255));

			DrawTextureScaled(pos.x, pos.y, texCardHeader, color * activeMulti);

			if (GetPositionScaled(pos).y > 0)
			{
				string text = item.t.name;
				if (selectItemGroup != null) text = TC.outputNames[item.outputId] + " Group";
				if (node != null)
				{
					if (node.inputKind == InputKind.File)
					{
						if (node.inputFile == InputFile.RawImage)
						{
							if (text == "Node" && node.stampTex != null) text = node.stampTex.name;
						}
					}
				}
				DrawText(new Vector2(pos.x + 2, pos.y + 3), Mathw.CutString(text, TC.nodeLabelLength), string.Empty, 20, Color.white * activeMulti, FontStyle.Normal, HorTextAlign.Left); // TODO: Optimize by caching name
			}
			
			// Rect previewRect = new Rect(rect.x + nodeBorderWidth, rect.y + previewOffsetY, rect.width - (nodeBorderWidth * 2), nodeWidth - (nodeBorderWidth * 2));

			Rect rectEye;
			
			if (layer != null || layerGroup != null) rectEye = new Rect(pos.x + 216, pos.y + 3, 24, 24);
			else rectEye = new Rect(pos.x + 239.2f, pos.y + 3, 24, 24);
			Rect rectEyeScaled = GetRectScaled(rectEye);

			bool drawEye = true;

			if (selectItemGroup != null || groupResult != null) drawEye = false;

			if (nodeGroup != null)
			{
				if (nodeGroup.type == NodeGroupType.Select) drawEye = false;
			}

			//if (node != null)
			//{
			//    if (node.nodeType == NodeGroupType.Select)
			//    {
			//        TC_NodeGroup nodeGroupParent = (TC_NodeGroup)node.parentItem;
			//        if (nodeGroupParent.firstActive != -1)
			//        {
			//            if (nodeGroupParent.totalActive <= 1 && nodeGroupParent.itemList[nodeGroupParent.firstActive].node == node) drawEye = false;
			//        }
			//    }
			//}

			//if (Selection.activeTransform == item.t)
			//{
			//    newScrollOffset = pos;
			//    setNewScrollOffset = true;
			//}
			
			if (drawEye)
			{
				mouseButton = Button(rectEye, item.visible ? texEye : texEyeClosed, string.Empty, true, item.visible && item.active ? new Color(1, 1, 1, 0.25f) : new Color(1, 0, 0, 0.65f), item.visible && item.active ? Color.white : Color.red, Color.white, true, false);
				if (mouseButton == 0)
				{
					item.visible = !item.visible;
					EditorUtility.SetDirty(item);
					if (selectItem != null) selectItem.parentItem.refreshRangeItem = selectItem;

					TC.RefreshOutputReferences(item.outputId, true);
					eventCurrent.Use();
					//Debug.Log("tes "+item.active); 
				}
			}

			Rect rectExclude = rectEyeScaled;

			if (item.isPortalCount > 0)
			{
				rectEye.x -= 28;
				mouseButton = Button(rectEye, texPortal, string.Empty, true, item.isPortalCount > 1 ? Color.white : new Color(0.5f, 1, 0.6f), Color.white, Color.black, item.isPortalCount > 1 ? false : true);
				if (mouseButton == 0)
				{
					Selection.activeTransform = item.usedAsPortalList[0].t;
					Event.current.Use();
				}
			}

			if (item.portalNode != null)
			{
				rectEye.x -= 28;
				mouseButton = Button(rectEye, texPortal, string.Empty, true, new Color(1, 0.3f, 0.3f, 1), Color.white, Color.black, true);
				if (mouseButton == 0)
				{
					Selection.activeTransform = item.portalNode.t;
					Event.current.Use();
				}
			}
			
			if (selectItemGroup == null && selectItem == null && (item.lockTransform || item.lockPosParent || item.lockPosChildren))
			{
				Rect rectLocked = new Rect(rectEye.x - 23, rectEye.y + 1.7f, texLocked.width / 3.5f, texLocked.height / 3.5f);
				
				if (groupResult != null) rectLocked.x += 8;
				if (nodeGroup != null)
				{
					if (nodeGroup.type == NodeGroupType.Select) rectLocked.x += 30;
				}
				
				if (node != null)
				{
					if (item.lockTransform || item.lockPosParent) mouseButton = Button(rectLocked, texLocked, string.Empty, item.lockTransform, new Color(1, 0, 0, 0.5f), item.lockTransform ? new Color(1, 0, 0, 1) : Color.white, new Color(1, 1, 1, 0.25f));
					else mouseButton = -1;
				}
				else mouseButton = Button(rectLocked, item.lockPosParent || item.lockPosChildren ? texLocked : texUnlocked, string.Empty, item.lockPosChildren, new Color(1, 0, 0, 0.5f), item.lockPosChildren ? new Color(1, 0, 0, 1) : Color.white, new Color(1, 1, 1, 0.25f));

				if (mouseButton == 0)
				{
					if (node != null) item.lockTransform = !item.lockTransform;
					else
					{
						item.lockPosChildren = !item.lockPosChildren;
						item.SetLockChildrenPosition(false);
					}
					eventCurrent.Use();
				}
			}

			if (layerGroup != null || groupResult != null || layer != null) rectExclude = GetRectScaled(new Rect(pos.x + 245.1f, pos.y + 6.5f, 20, 20));

			mouseButton = ClickRect(rectNode, rectExclude);
			if (mouseButton == 0) 
			{
				//if (eventCurrent.control && eventCurrent.clickCount == 2)
				//{
				//    item.visible = item.active = !item.visible;
				//    TC.AutoGenerate();
				//} 
				//else
				//{
				if (eventCurrent.control)
				{
					if (Mathw.ArrayContains(Selection.transforms, item.t)) Selection.objects = Mathw.RemoveFromArray(Selection.objects, item.gameObject);
					else Selection.objects = Mathw.AddToArray(Selection.objects, item.gameObject);
				}
				else Selection.objects = new GameObject[] { item.gameObject };

				// TC_ProjectPreview.instance.SetPreview(item);
				//}
			}

			Rect rectPreview = GetRectScaled(pos.x + 7, pos.y + texCardHeader.height + 7, 256, 256);

			if (nodeFoldout)
			{
				DrawTextureScaled(pos.x, pos.y + texCardHeader.height, texCardBody, color * activeMulti);

				// DrawRect(previewRect, Color.white);
				if (drawPreview)
				{
					// Rect previewRect2 = GetRect(new Rect(previewRect.xMin + 0.5f, previewRect.yMin + 0.5f, previewRect.width - 1, previewRect.height - 1));
					GUI.color = previewColor;

					// GUI.DrawTexture(rectPreview, Texture2D.whiteTexture);
					Texture texPreview = null;

					if (item.outputId == TC.colorOutput && selectItem != null)
					{
						if (selectItem.texColor == null || selectItem.parentItem.itemList.Count != 1) texPreview = item.preview.tex; else texPreview = item.rtDisplay;
					}
					else
					{
						if (nodeGroup != null) texPreview = nodeGroup.rtColorPreview;
						else if (node != null)
						{
							TC_NodeGroup parent = (TC_NodeGroup)node.parentItem;
							if (parent.itemList.Count == 1) texPreview = ((TC_NodeGroup)node.parentItem).rtColorPreview;
						}
						if (texPreview == null)
						{
							if (item.rtDisplay != null) texPreview = item.rtDisplay;
							else if (item.rtPreview != null) texPreview = item.rtPreview;
							else if (item.preview.tex != null) texPreview = item.preview.tex;
						}
					}

					if (texPreview == null) texPreview = Texture2D.blackTexture;

                    //if (selectItem != null && item.outputId == TC.grassOutput)
                    //{
                    //    GUI.color = new Color(0, 0.5f, 0.25f, 1);
                    //    GUI.DrawTexture(rectPreview, Texture2D.whiteTexture);
                    //    GUI.color = Color.white;
                    //    GUI.DrawTexture(rectPreview, texPreview);
                    //}
                    //else 
                    if (item is TC_SelectItem && item.outputId == TC.colorOutput) GUI.DrawTexture(rectPreview, texPreview);
                    else EditorGUI.DrawPreviewTexture(rectPreview, texPreview);

                    // PreviewEdit(item);
                }
				GUI.color = Color.white;

				if (item.outputId == TC.treeOutput || item.outputId == TC.objectOutput)
				{
					if (selectItem != null || layer != null || layerGroup != null || selectItemGroup != null)
					{
						Rect rectButton2 = new Rect(pos.x + 7.24f, pos.y + 306f, 254f, 30);
						DrawTextureScaled(rectButton2, texButton, string.Empty, color * (item.active ? 1 : 0.75f));

						int placed = 0;
						if (selectItem != null) placed = selectItem.placed;
						else if (layer != null) placed = layer.placed;
						else if (layerGroup != null) placed = layerGroup.placed;
						else if (selectItemGroup != null) placed = selectItemGroup.placed;

						DrawText(pos + new Vector2(12.24f, 308f), "Placed: " + placed, string.Empty, 21, Color.white * (item.active ? 1 : 0.75f));
					}
				}

				Rect rectSlider = new Rect(rectNode.x + 5, rectNode.y + texCardBody.width + 6, 37 * scale, 10 * scale);

				rectSlider.y += 12 * scale;
				rectSlider.height = 7f * scale;
				rectSlider.x += 23 * scale;
				rectSlider.width += 6 * scale;

				if (drawMethod)
				{
					if (layerGroup != null)
					{
						if (layerGroup.parentItem != null)
						{
							if (layerGroup.listIndex == ((TC_LayerGroupResult)layerGroup.parentItem).firstActive || !layerGroup.active) activeMulti *= 0.5f;
						}
					}
					else if (layer != null)
					{
						if (layer.parentItem != null)
						{
							if (layer.listIndex == ((TC_LayerGroupResult)layer.parentItem).firstActive || !layer.active) activeMulti *= 0.5f;
						}
					}
					else drawMethod = false;

					if (drawMethod) DrawMethod(item, pos + new Vector2(310, 187), false, color, activeMulti);
				}

				Rect rectButton = new Rect(pos.x + 7.24f, pos.y + 306f + 35, 254f, 30);
				if (GetRectScaled(rectButton).y > 0)
				{
					bool drawOpacity = true;

					if (layerGroup != null)
					{
						if (layerGroup.level == 0) drawOpacity = false;
						else if (layerGroup.method != Method.Lerp || layerGroup.listIndex == ((TC_LayerGroupResult)layerGroup.parentItem).firstActive) drawOpacity = false;
					}
					else if (layer != null)
					{
						if (layer.method != Method.Lerp) drawOpacity = false; // || layer.listIndex == ((TC_LayerGroupResult)layer.parentItem).firstActive) drawOpacity = false;
					}
					else if (groupResult != null) drawOpacity = false;
					else if (nodeGroup != null) drawOpacity = false;
					else if (selectItem != null || selectItemGroup != null)
					{
						if (item.outputId == TC.splatOutput || item.outputId == TC.grassOutput) drawOpacity = false;
						if (item.outputId == TC.colorOutput)
						{
							if (selectItemGroup != null || selectItem != null) drawOpacity = false;
						}
					}

					if (drawOpacity)
					{
						if (DrawOpacity(item, rectButton, color, activeMulti))
						{
							if (selectItem != null) selectItem.parentItem.CreateMixBuffer();
							else if (selectItemGroup != null) selectItemGroup.CreateMixBuffer();
							TC.repaintNodeWindow = true;
							EditorUtility.SetDirty(item);
							TC.AutoGenerate();
						}
					}
				}
				if (selectItem != null && item.outputId == TC.treeOutput) rectPreview.height -= 35 * scale;
				DragDropNode(item, rectPreview, rectNode, pos);
			}
			
			if (Mathw.ArrayContains(Selection.transforms, item.t))
			{
				// Color colLerp = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 0, 0, 1), Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 1.5f)));
				if (nodeFoldout) DrawCommand.Add(new Rect(pos.x - 4f, pos.y - 4f, texSelectCard.width - 2, texSelectCard.height - 2), texSelectCard, string.Empty, Color.white, 1);
				else DrawCommand.Add(new Rect(pos.x - 4f, pos.y - 4f, texSelectCardHeader.width - 2, texSelectCardHeader.height - 2), texSelectCardHeader, string.Empty, Color.white, 1);
			}
			
			return rectPreview;
		}

		static public void DrawMethod(TC_ItemBehaviour item, Vector2 pos, bool equal, Color backgroundColor, float activeMulti)
		{
			DrawCommand.Add(pos, texOperandBG, string.Empty, backgroundColor * activeMulti, 2);

			Texture texOperand = null;

			if (item.GetType() == typeof(TC_LayerGroup) || item.GetType() == typeof(TC_Layer))
			{
				if (item.outputId == TC.treeOutput || item.outputId == TC.objectOutput)
				{
					if (item.method != Method.Lerp && item.method != Method.Max && item.method != Method.Min) item.method = Method.Lerp;
				}
			}

			if (equal) texOperand = texOperandEqual;
			else switch (item.method) 
			{
				case Method.Add:        texOperand = texOperandAdd;         break;
				case Method.Subtract:   texOperand = texOperandSubtract;    break;
				case Method.Lerp:       texOperand = texOperandLerp;        break;
				case Method.Multiply:   texOperand = texOperandMultiply;    break;
				case Method.Divide:     texOperand = texOperandDivide;      break;
				case Method.Difference: texOperand = texOperandDiff;        break;
				case Method.Average:    texOperand = texOperandAverage;     break;
				case Method.Min:        texOperand = texOperandMin;         break;
				case Method.Max:        texOperand = texOperandMax;         break;
			}

			if (texOperand != null) 
			{
				DrawCommand.Add(pos, texOperand, string.Empty, Color.white * Mathf.Clamp01(activeMulti + 0.25f), 2);
				if (!equal) DropDownMethodMenu(item, new Rect(pos.x, pos.y, texOperand.width, texOperand.height));
			}
		}

		static public bool DrawOpacity(TC_ItemBehaviour item, Rect rect, Color color, float activeMulti)
		{
			DrawTextureScaled(rect, texInwardButton, string.Empty, (color + new Color(0.5f, 0.5f, 0.5f)) * activeMulti);

			bool changed = false;

			Vector2 click = ClickRectPercentage(GetRectScaled(rect));
			bool isSelected = selectedOpacityItem == item;
			if (selectedOpacityItem != null && !isSelected) click.x = -1;

			if (mouseDownButton == 0)
			{
				if (Mathw.ArrayContains(Selection.transforms, item.t))
				{
					if (click.x == 0) selectedOpacityItem = item;

					if (isSelected)
					{
						if (item.opacity != click.y) changed = true;
						item.opacity = click.y;
					}
				}
			}
			else selectedOpacityItem = null;

			rect.width *= item.opacity;

			DrawTextureScaled(rect, texInwardButton, string.Empty, color * activeMulti);

			Color fontColor = click.x == -2 || isSelected ? Color.white : new Color(1, 1, 1, 0.5f);

			DrawText(new Vector2(rect.x + 5.02f, rect.y + 14.54f), "Opacity:", string.Empty, 21, Color.white, FontStyle.Normal, HorTextAlign.Left, VerTextAlign.Center);
			DrawText(new Vector2(rect.x + 5.02f + 96.9f, rect.y + 14.54f), "◄►", string.Empty, 14, fontColor, FontStyle.Normal, HorTextAlign.Left, VerTextAlign.Center);
			DrawText(new Vector2(rect.x + 5.02f + 241.97f, rect.y + 14.54f), (item.opacity * 100).ToString("F0") + "%", string.Empty, 21, Color.white, FontStyle.Normal, HorTextAlign.Right, VerTextAlign.Center);

			return changed;
		}

		static public void DropDownMethodMenu(TC_ItemBehaviour item, Rect rect)
		{
			rect = GetRectScaled(rect);
			if (ClickRect(rect) != 0) return;

			GenericMenu menu = new GenericMenu();

			string instanceID = item.GetInstanceID().ToString();

			string methodName;

			if ((item.outputId == TC.treeOutput || item.outputId == TC.objectOutput) && (item.GetType() == typeof(TC_LayerGroup) || item.GetType() == typeof(TC_Layer)))
			{
				menu.AddItem(new GUIContent("Lerp"), false, LeftClickMethodMenu, instanceID + ":" + "Lerp");
				menu.AddItem(new GUIContent("Max"), false, LeftClickMethodMenu, instanceID + ":" + "Max");
				menu.AddItem(new GUIContent("Min"), false, LeftClickMethodMenu, instanceID + ":" + "Min");
			}
			else
			{
				for (int i = 0; i < 9; i++)
				{
					methodName = ((Method)i).ToString();
					menu.AddItem(new GUIContent(methodName), false, LeftClickMethodMenu, instanceID + ":" + methodName);
				}
			}
			
			menu.ShowAsContext();
			eventCurrent.Use();
		}

		static void LeftClickMethodMenu(object obj)
		{
			int instanceID;
			string command = ObjectToCommandAndInstanceID(obj, out instanceID);

			TC_ItemBehaviour item = EditorUtility.InstanceIDToObject(instanceID) as TC_ItemBehaviour;
			Method oldMethod = item.method;
			item.method = (Method)System.Enum.Parse(typeof(Method), command);
			if (item.method != oldMethod)
			{
				EditorUtility.SetDirty(item);
				TC.AutoGenerate();
			}
		}

		static public string ObjectToCommandAndInstanceID(object obj, out int instanceID)
		{
			string command = obj.ToString();
			int index = command.IndexOf(":");
			int.TryParse(command.Substring(0, index), out instanceID);
			return command.Substring(index + 1);
		}

		static public void DrawBracket(ref Vector2 pos, bool nodeFoldout, bool isRightBracket, Color color, ref int foldout, bool drawFoldoutButton, bool clickAbleFoldout)
		{
			TC_GlobalSettings g = TC_Settings.instance.global;
			
			pos.x -= isRightBracket ? texBracketBody.width + g.nodeHSpace : texBracketBody.width + g.bracketHSpace;
			DrawTextureScaled(pos.x, pos.y, texBracketHeader, color, !isRightBracket);
			
			Rect rect;
			
			if (isRightBracket) rect = new Rect(pos.x + 29.1f, pos.y + 3.52f, texBracketLeft.width - 5, texBracketLeft.height - 5);
			else rect = new Rect(pos.x + 7.1f, pos.y + 3.52f, texBracketLeft.width - 5, texBracketRight.height - 5);

			int mouseClick = Button(rect, (foldout < 2 ? !isRightBracket : isRightBracket) ? texBracketLeft : texBracketRight, string.Empty, foldout == 2, new Color(1, 1, 1, 0.25f), Color.white, new Color(1, 1, 1, 0.25f), clickAbleFoldout);
			
			if (mouseClick == 0) {
				if (foldout == 2) foldout = 0; else foldout = 2;
			}
			else if (mouseClick == 1) {
				if (foldout == 2) foldout = 1; else foldout = 2;
			}
				
			if (nodeFoldout) DrawTextureScaled(pos.x, pos.y + texBracketHeader.height, texBracketBody, color, !isRightBracket);
			
			pos.x -= isRightBracket ? g.bracketHSpace : 0;

			// Debug.Log(pos.y);
			if (-pos.x > scrollMax.x) scrollMax.x = -pos.x;
			if (pos.y > scrollMax.y) scrollMax.y = -pos.y; 
		}

		static public int Button(Rect rect, Texture tex, string tooltip, bool active, Color colNormal, Color colHover, Color colDisabled, bool clickAble = true, bool drawTextureCommand = false)
		{
			Rect rectScaled = GetRectScaled(rect);
			if (!clickAble)
			{
				if (drawTextureCommand) DrawCommand.Add(rect, tex, tooltip, colNormal, 2);
				else DrawTexture(rectScaled, tex, tooltip, colNormal);
				return -1;
			}
			int mouseState = ClickRect(rectScaled);
			Color color = mouseState == -2 ? colHover : active ? colNormal : colDisabled;
			if (drawTextureCommand) DrawCommand.Add(rect, tex, tooltip, color, 2);
			else DrawTexture(rectScaled, tex, tooltip, color);
			return mouseState;
		}

		static public int DrawNodeCount(TC_ItemBehaviour item, GroupType groupType, ref Vector2 pos, int itemCount, bool nodeFoldout, ref int foldout, Color color, float scale = 1)
		{
			if (foldout == 2 && itemCount > 0) return -1;
			
			pos.x -= foldout == 1 ? 10 : 20;
			float posY = 0;

			if (nodeFoldout) posY += (cardHeight / 2) - (texBracketHeader.height / 2);
			else pos.x -= 20;

			Rect rect = new Rect(pos.x - 38 , pos.y + posY - 32, texCardCounter.width * scale, texCardCounter.height * scale);
			Rect rectScaled = GetRectScaled(rect);

			int mouseClick = ClickRect(rectScaled);

			string tooltip;

			if (groupType == GroupType.NodeGroup)
            {
				if (((TC_NodeGroup)item).type == NodeGroupType.Select) tooltip = "Add a new Select Node"; else tooltip = "Add a new Mask Node";
			}
			else if (groupType == GroupType.LayerGroupResult)
            {
				tooltip = "Add a new Layer";
            }
			else
            {
				tooltip = string.Empty;
            }
			
			DrawCommand.Add(rect, texCardCounter, string.Empty, color, 2);

			if (foldout == 1) pos.x += 10;

			if (itemCount == 0)
			{
				DrawCommand.Add(rect, texAddFirstCard, tooltip, mouseClick == -2 ? Color.white : new Color(1, 1, 1, 0.25f), 2);
				DragDropNode(item, rectScaled, rectScaled, pos, false, false);
			}
			else 
			{
				DrawCommand.Add(new Vector2(rect.x + (50 * scale), rect.y + (50 * scale)), itemCount.ToString(), string.Empty, Mathf.RoundToInt(0.48f * rect.width), mouseClick == -2 ? Color.white : new Color(1, 1, 1, 0.35f), FontStyle.Normal, HorTextAlign.Center, VerTextAlign.Center);
			}

			if (mouseClick == 0) foldout = 2;

			return mouseClick;
		}

		static public bool DrawButton(Rect rect, string text, int fontSize, bool bold, Color col, Color colBackground)
		{
			int oldFontSize = GUI.skin.button.fontSize;
			FontStyle oldBold = GUI.skin.button.fontStyle;
			Color colTextOld = GUI.skin.button.normal.textColor;

			rect = GetRectScaled(rect);

			if (bold) GUI.skin.button.fontStyle = FontStyle.Bold;
			GUI.skin.button.fontSize = (int)(fontSize * scale);
			GUI.skin.button.normal.textColor = col;
			GUI.backgroundColor = colBackground;
			
			bool clicked = GUI.Button(rect, text);
			GUI.backgroundColor = Color.white;

			GUI.skin.button.fontSize = oldFontSize;
			GUI.skin.button.fontStyle = oldBold;
			GUI.skin.button.normal.textColor = colTextOld;

			return clicked;
		}

		static public Vector2 GetLabelSize(string label)
		{
			return GUI.skin.GetStyle("Label").CalcSize(new GUIContent(label));
		}

		static public void DrawText(Vector2 pos, string text, string tooltip, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign verTextAlign = VerTextAlign.Top)
		{
			Color colorOld = GUI.color;
			GUI.color = color;
			int old_fontSize = GUI.skin.label.fontSize;
			FontStyle old_fontStyle = GUI.skin.label.fontStyle;
			Color colTextOld = GUI.skin.label.normal.textColor;
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.fontStyle = fontStyle;
			GUI.skin.label.normal.textColor = Color.white;

			Vector2 size = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));
            size.x += 7;
			AlignPosition(ref pos, size, horTextAlign, verTextAlign);

			pos = GetPositionScaled(pos);
			Matrix4x4 oldMatrix = GUI.matrix;
			GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), new Vector2(pos.x, pos.y));
			
			GUI.Label(new Rect(pos.x, pos.y, size.x, size.y), TD.GetGUIContent(text, tooltip));

			GUI.matrix = oldMatrix;
			
			GUI.skin.label.fontSize = old_fontSize;
			GUI.skin.label.fontStyle = old_fontStyle;
			GUI.color = colorOld;
			GUI.skin.label.normal.textColor = colTextOld;
		}

		static public float DrawTextX(Vector2 pos, float rightMargin, float leftPos, string text, int fontSize, FontStyle fontStyle, Color color, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign verTextAlign = VerTextAlign.Top)
		{
			Color colorOld = GUI.color;
			GUI.color = color;
			int old_fontSize = GUI.skin.label.fontSize;
			FontStyle old_fontStyle = GUI.skin.label.fontStyle;
			GUI.skin.label.fontSize = Mathf.RoundToInt(fontSize * scale);
			GUI.skin.label.fontStyle = fontStyle;

			Vector2 size = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(text));

			AlignPosition(ref pos, size, horTextAlign, verTextAlign);

			Vector2 sPos = GetPositionScaled(new Vector2(pos.x, pos.y));
			Rect sRect = new Rect(sPos.x, sPos.y, size.x, size.y);
			rightMargin *= scale;
			
			if (sRect.x + size.x + rightMargin > TC_NodeWindow.window.position.width) sRect.x = TC_NodeWindow.window.position.width - size.x - rightMargin;
			float delta = sRect.x - GetPosX(leftPos);
			if (delta < 0) sRect.x -= delta;

			GUI.Label(sRect, text);

			GUI.skin.label.fontSize = old_fontSize;
			GUI.skin.label.fontStyle = old_fontStyle;
			GUI.color = colorOld;

			return (size.x / scale);
		}

		static public void DrawSlider(Vector2 startOffset, ref float value, float min, float max)
		{
			Vector2 sliderPos = GetPositionScaled(new Vector2(startOffset.x + 6, (startOffset.y + 84)));
			GUIUtility.ScaleAroundPivot(new Vector2(scale / 1.5f, scale / 1.5f), new Vector2(sliderPos.x, sliderPos.y));

			GUI.changed = false;
			value = GUI.HorizontalSlider(new Rect(sliderPos.x, sliderPos.y, 93, 10), value, 0, max);

			GUI.matrix = Matrix4x4.Scale(new Vector3(1, 1, 1));
		}

		static public void AlignPosition(ref Vector2 pos, Vector2 size, HorTextAlign horTextAlign, VerTextAlign verTextAlign)
		{
			if (horTextAlign == HorTextAlign.Center) pos.x -= size.x / 2;
			else if (horTextAlign == HorTextAlign.Right) pos.x -= size.x;
			if (verTextAlign == VerTextAlign.Center) pos.y -= size.y / 2;
			else if (verTextAlign == VerTextAlign.Bottom) pos.y -= size.y;
		}

		static public void AlignPosition(ref Rect rect, HorTextAlign horTextAlign, VerTextAlign verTextAlign)
		{
			if (horTextAlign == HorTextAlign.Center) rect.x -= rect.width / 2;
			else if (horTextAlign == HorTextAlign.Right) rect.x -= rect.width;
			if (verTextAlign == VerTextAlign.Center) rect.y -= rect.height / 2;
			else if (verTextAlign == VerTextAlign.Bottom) rect.y -= rect.height;
		}

		static public void DrawPopup(Rect rect, ref System.Enum popup, int fontSize, FontStyle fontStyle, Color color)
		{
			int fontSizeOld = EditorStyles.popup.fontSize;
			FontStyle fontStyleOld = EditorStyles.popup.fontStyle;
			// float fixedHeightOld = EditorStyles.popup.fixedHeight;

			EditorStyles.label.fontSize = Mathf.RoundToInt(fontSize * scale);
			EditorStyles.label.fontStyle = fontStyle;

			// EditorStyles.popup.fixedHeight = rect.height;
			// EditorStyles.popup.stretchHeight = true;
			// EditorStyles.popup.CalcScreenSize(new Vector2(rect.width,rect.height));

			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.x, rect.y, 4, rect.height), enumBG_Start);
			GUI.DrawTexture(new Rect(rect.x + 4, rect.y, rect.width - 15, rect.height), enumBG_Middle);
			GUI.DrawTexture(new Rect(rect.x + (rect.width - 11), rect.y, 11, rect.height), enumBG_End);

			rect.x += 3 * scale;
			rect.width -= 3 * scale;
			GUI.changed = false;
			popup = (Method)EditorGUI.EnumPopup(rect, "", popup, EditorStyles.label);
			if (GUI.changed) TC.AutoGenerate();
			//		GUI.color = Color.red;
			//		GUI.DrawTexture (rect,tex);

			EditorStyles.label.fontSize = fontSizeOld;
			EditorStyles.label.fontStyle = fontStyleOld;
			// EditorStyles.popup.fixedHeight = fixedHeightOld;
		}

		static public bool DrawButton(Rect rect, Texture texture, int mouseButton, Color color)
		{
			Rect sRect = GetRectScaled(rect);
			Color colorOld = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(sRect, texture);
			GUI.color = colorOld;
			return ClickRect(sRect, mouseButton);
		}

		static public bool DrawButtonX(Rect rect, float rightMargin, float leftPos, Texture texture, int mouseButton, Color color, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign verTextAlign = VerTextAlign.Top)
		{
			// Rect sRect = GetRect (rect);
			// sRect = new Rect(rect.x*scale,sRect.y,sRect.width,sRect.height);
			AlignPosition(ref rect, horTextAlign, verTextAlign);
			Rect sRect = GetRectScaled(rect);

			rightMargin *= scale;
			if (sRect.x + rect.width + rightMargin > TC_NodeWindow.window.position.width) sRect.x = TC_NodeWindow.window.position.width - rect.width - rightMargin;
			float delta = sRect.x - GetPosX(leftPos);
			if (delta < 0) sRect.x -= delta;

			Color colorOld = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(sRect, texture);
			GUI.color = colorOld;
			return ClickRect(sRect, mouseButton);
		}

		static public void DrawBar(Rect topBar, Rect bottomBar, float barWidth, Color color)
		{
			// Header Bar
			TD.DrawRect(topBar, TD.texFadeBar, color);
			// Vertical Bar
			TD.DrawRect(new Rect(topBar.xMax, topBar.y, barWidth, bottomBar.yMax - topBar.y), Texture2D.whiteTexture, color);
			TD.DrawRect(bottomBar, TD.texFadeBar, color);
			// Bottom Bar
		}

		static public bool ClickRect(Rect rect, int mouseButton)
		{
			// rect = GetRect (rect);
			if (eventCurrent.type == EventType.MouseDown)
			{
				if (rect.Contains(eventCurrent.mousePosition) && eventCurrent.button == mouseButton) return true;
			}
			return false;
		}

		static public int ClickRect(Rect rect)
		{
			// rect = GetRect (rect);
			if (rect.Contains(eventCurrent.mousePosition))
			{
				if (eventCurrent.type == EventType.MouseDown)
				{
					return eventCurrent.button;
				}
				return -2; // hover
			}
			return -1;
		}

		static public int ClickRect(Rect rect, Rect rectExclude)
		{
			// rect = GetRect (rect);
			Vector2 mousePosition = eventCurrent.mousePosition;

			if (rect.Contains(mousePosition) && !rectExclude.Contains(mousePosition))
			{
				if (eventCurrent.type == EventType.MouseDown)
				{
					return eventCurrent.button;
				}
				return -2; // hover
			}
			return -1;
		}

		static public Vector2 ClickRectPercentage(Rect rect)
		{
			if (rect.Contains(posClickMouseDown))
			{
					TC.repaintNodeWindow = true;
					return new Vector2(eventCurrent.button, Mathf.Clamp01((eventCurrent.mousePosition.x - rect.x) / rect.width));
			}
			else if (rect.Contains(eventCurrent.mousePosition))
			{
				return new Vector2(-2, Mathf.Clamp01((eventCurrent.mousePosition.x - rect.x) / rect.width)); // hover
			}
			return new Vector2(-1, Mathf.Clamp01((eventCurrent.mousePosition.x - rect.x) / rect.width));
		}

		static public void DrawLabel(string label, int fontSize)
		{
			int fontSizeOld = EditorStyles.label.fontSize;
			EditorStyles.boldLabel.fontSize = fontSize;
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(fontSize + 6));
			EditorStyles.boldLabel.fontSize = fontSizeOld;
		}

		static public void DrawLabelWidthUnderline(string label, int fontSize, bool boldLabel = true)
		{
			int fontSizeOld = EditorStyles.label.fontSize;
			EditorStyles.boldLabel.fontSize = fontSize;
			EditorGUILayout.LabelField(label, boldLabel ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Height(fontSize + 6));
			EditorStyles.boldLabel.fontSize = fontSizeOld;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = Color.grey;
			GUI.DrawTexture(new Rect(rect.x, rect.yMax, rect.width, 1), Texture2D.whiteTexture);
			GUI.color = Color.white;
			GUILayout.Space(5);
		}

		static public void PreviewEdit(TC_ItemBehaviour item)
		{
			Event key = eventCurrent;

			if (key.type == EventType.MouseDrag)
			{
				if (key.button == 0)
				{
					Vector3 delta = Quaternion.Inverse(item.t.rotation) * new Vector3(key.delta.x * (1000 / 250), 0, -key.delta.y * (1000 / 250));
					if (delta.x != 0 || delta.y != 0 || delta.z != 0)
					{
						item.t.position += item.t.TransformDirection(new Vector3(delta.x / item.t.lossyScale.x, 0, delta.z / item.t.lossyScale.z));
						TC.AutoGenerate();
					}
				}
				if (key.button == 1) item.t.Rotate(0, key.delta.x, 0);
				// item.ResetOffset();
			}

			if (key.type == EventType.ScrollWheel)
			{
				Vector3 scale = item.t.localScale;

				if (key.delta.y > 0) scale /= 1 + ((key.delta.y / 3) * 0.1f);
				else scale *= 1 + ((-key.delta.y / 3) * 0.1f);

				// if (scale.x == 0) scale.x = 0.000001f;
				// if (scale.z == 0) scale.z = 0.000001f;

				item.t.localScale = scale;
				// item.ResetOffset();

				TC_Reporter.Log("" + scale);
			}
		}

		static public void DrawProperty(SerializedProperty property, GUIContent guiContent = null, float width = -1)
		{
			GUI.changed = false;
			if (width == -1)
			{
				if (guiContent == null) EditorGUILayout.PropertyField(property);
				else EditorGUILayout.PropertyField(property, guiContent);
			}
			else
			{
				if (guiContent == null) EditorGUILayout.PropertyField(property, GUILayout.Width(width));
				else EditorGUILayout.PropertyField(property, guiContent, GUILayout.Width(width));
			}
			if (GUI.changed) TC.AutoGenerate();
		}

		static public void DrawPropertyArray(SerializedProperty property)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(0);
			Rect rect = GUILayoutUtility.GetLastRect();
			property.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y + 3, 25, 18), property.isExpanded, "");
			if (property.isExpanded) DrawLabelWidthUnderline(property.displayName, 12); else DrawLabel(property.displayName, 12);
			EditorGUILayout.EndHorizontal();

			if (property.isExpanded)
			{
				GUILayout.Space(5);
				EditorGUILayout.BeginVertical("Box");
				EditorGUI.indentLevel++;
				property.arraySize = EditorGUILayout.IntField("Size", property.arraySize);
				EditorGUI.indentLevel--;

				EditorGUI.indentLevel++;

				for (int i = 0; i < property.arraySize; i++)
				{
					SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

					DrawProperty(elementProperty);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
			}
		}

		static public SerializedProperty DrawArrayProperty(SerializedProperty arrayElement, string name)
		{
			SerializedProperty element = arrayElement.FindPropertyRelative(name);
			if (element != null) EditorGUILayout.PropertyField(element);
			return element;
		}

		static public void DrawSpacer(float space = 5)
		{
			GUILayout.Space(space);
			EditorGUILayout.BeginHorizontal();
			GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);
			GUILayout.Button("", GUILayout.Height(5));
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(space);

			GUI.color = Color.white;
		}

		static public void ClickOutputButton(TC_LayerGroup layerGroup)
		{
			if (eventCurrent.button == 0)
			{
				layerGroup.visible = !layerGroup.visible;
				EditorUtility.SetDirty(layerGroup);
				TC.RefreshOutputReferences(layerGroup.outputId, layerGroup.visible);
			}
			else
			{
				if (TC_Area2D.instance == null) return;
				bool visibleOld = layerGroup.visible;
				layerGroup.visible = true;
				// TC_Area2D.current.terrainLayer.GetItem(layerGroup.outputId);
				TC_Generate.instance.Generate(false, layerGroup.outputId);
				
				layerGroup.visible = visibleOld;
			}
		}
		
		static public class MinMaxSlider
		{
			static public Texture sliderBaseMiddle;
			static public Texture sliderBaseLeft;
			static public Texture sliderBaseRight;
			static public Texture sliderMiddle;
			static public Texture sliderLeft;
			static public Texture sliderRight;

			static public bool changed = false;
			static public bool mouseDown = false;
			static public Vector2 mousePosOld;
			static public Vector2 delta;
			static public Vector2 vOld;

			static public bool leftDown = false;
			static public bool rightDown = false;
			static public bool middleDown = false;

			static public Vector2 Draw(Rect rect, Vector2 v, float min, float max, Vector2 clickOffset)
			{
				if (eventCurrent == null) return v;
				changed = false;
				rect.width -= 16.0f;

				if (sliderBaseMiddle == null) LoadTextures();

				// Debug.Log(key.mousePosition+", "+rect);

				float range = max - min;
				float scale = range / rect.width;
				float vRange = v.y - v.x;
				// Debug.Log(rect);
				// Debug.Log(scale);

				GUI.DrawTexture(new Rect(rect.x + 2, rect.y + (rect.height / 2), rect.width + 12, 5.0f), sliderBaseMiddle);
				GUI.DrawTexture(new Rect(rect.x, rect.y + (rect.height / 2), 2.0f, 5.0f), sliderBaseLeft);
				GUI.DrawTexture(new Rect((rect.x + rect.width) + 14, rect.y + (rect.height / 2), 2.0f, 5.0f), sliderBaseRight);
				
				float startLeft = (((v.x - min) / range) * rect.width);
				float startRight = (((v.y - min) / range) * rect.width);
				Rect leftRect = new Rect(rect.x + startLeft, rect.y + (rect.height / 2) - 3, 8.0f, 11.0f);
				Rect rightRect = new Rect(rect.x + startRight + 8, rect.y + (rect.height / 2) - 3, 8.0f, 11.0f);
				Rect middleRect = new Rect(rect.x + startLeft + 8, rect.y + (rect.height / 2), (startRight - startLeft), 5.0f);

				GUI.DrawTexture(middleRect, sliderMiddle);
				GUI.DrawTexture(leftRect, sliderLeft);
				GUI.DrawTexture(rightRect, sliderRight);

				middleRect.y -= 5.0f;
				middleRect.height += 10.0f;

				if (eventCurrent.type == EventType.MouseDown)
				{
					if (leftRect.Contains(eventCurrent.mousePosition))
					{
						leftDown = true;
						mousePosOld = eventCurrent.mousePosition;
						vOld = v;
						mouseDown = true;
					}
					else if (rightRect.Contains(eventCurrent.mousePosition))
					{
						rightDown = true;
						mousePosOld = eventCurrent.mousePosition;
						vOld = v;
						mouseDown = true;
					}
					else if (middleRect.Contains(eventCurrent.mousePosition))
					{
						middleDown = true;
						mousePosOld = eventCurrent.mousePosition;
						vOld = v;
						mouseDown = true;
					}
				}
				if (eventCurrent.type == EventType.MouseUp)
				{
					mouseDown = false;
					leftDown = false;
					rightDown = false;
					middleDown = false;
				}

				delta = eventCurrent.mousePosition - mousePosOld;

				if (mouseDown)
				{
					if (leftDown)
					{
						leftRect.x -= clickOffset.x;
						leftRect.width += clickOffset.x * 2;
						leftRect.y -= clickOffset.y;
						leftRect.height += clickOffset.y * 2;
						if (leftRect.Contains(eventCurrent.mousePosition))
						{
							v.x = vOld.x + (delta.x * scale);
							changed = true;
						}
					}
					else if (rightDown)
					{
						rightRect.x -= clickOffset.x;
						rightRect.width += clickOffset.x * 2;
						rightRect.y -= clickOffset.y;
						rightRect.height += clickOffset.y * 2;

						if (rightRect.Contains(eventCurrent.mousePosition))
						{
							v.y = vOld.y + (delta.x * scale);
							if (v.y < v.x) v.y = v.x;
							changed = true;
						}
					}
					if (middleDown)
					{
						middleRect.x -= clickOffset.x;
						middleRect.width += clickOffset.x * 2;
						middleRect.y -= clickOffset.y;
						middleRect.height += clickOffset.y * 2;

						if (middleRect.Contains(eventCurrent.mousePosition))
						{
							v = new Vector2(vOld.x + (delta.x * scale), vOld.y + (delta.x * scale));
							if (v.x < min) { v.x = min; v.y = min + vRange; }
							if (v.y > max) { v.y = max; v.x = max - vRange; }

							changed = true;
						}
					}
				}
				v.y = Mathf.Clamp(v.y, min, max);
				v.x = Mathf.Clamp(v.x, min, v.y);

				return v;
			}

			static public void LoadTextures()
			{
				sliderBaseMiddle = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderBaseMiddle.psd", typeof(Texture));
				sliderBaseLeft = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderBaseLeft.psd", typeof(Texture));
				sliderBaseRight = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderBaseRight.psd", typeof(Texture));
				sliderMiddle = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderMiddle.psd", typeof(Texture));
				sliderLeft = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderLeft.psd", typeof(Texture));
				sliderRight = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/SliderRight.psd", typeof(Texture)); 
			}
		}
	}

	[System.Serializable]
	public class NodeDraw
	{
		// public Vector2 offset = new Vector2(0,0);
		public bool selected = false;
	}

	[System.Serializable]
	public class GroupNodeDraw
	{
		// public Vector2 offset = new Vector2(0,0);
		public bool foldout = true;
		public bool selected = false;
	}
}