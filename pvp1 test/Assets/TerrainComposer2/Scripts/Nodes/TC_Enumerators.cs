namespace TerrainComposer2
{
    // GlobalManager
    public enum PresetMode { Default, StampMode }

    // ItemBehaviour
    public enum PositionMode { Transform, Offset, Locked }

    // Node
    public enum InputKind { Terrain = 0, Noise = 1, Shape = 2, File = 3, Current = 4, Portal = 5 };

    public enum InputTerrainHeight { Collision = 5 };
    public enum InputTerrain { Height, Angle, Normal, Convexity, Splatmap, Collision};
    public enum InputNoise { Perlin, Billow, Ridged, Cell, IQ, Swiss, Jordan, Random};
    public enum InputShape { Circle, Gradient, Rectangle, Constant };
    public enum InputFile { Image, RawImage };
    public enum InputCurrent { Blur = 0, Expand = 1, Shrink = 2, EdgeDetect = 4, Distortion = 3};
    public enum InputPortal { Portal };

    public enum NoiseMode { TextureLookup = 0, Normal = 1, Simplex = 2, Value = 3 };
    public enum NoiseMode2 { Normal = 1, Simplex = 2, Value = 3 }
    public enum CellNoiseMode { Fast, Normal };

    public enum Method { Add = 0, Subtract = 1, Lerp = 2, Multiply = 3, Divide = 4, Difference = 5, Average = 6, Max = 7, Min = 8 }
    public enum MethodItem { Overlay = 2, Max = 7, Min = 8 }
    public enum NodeGroupType { Select, Mask };
    public enum CollisionMode { Height, Mask };
    public enum CollisionDirection { Up, Down};
    public enum ConvexityMode { Convex, Concave };

    public enum BlurMode { Normal, Outward, Inward };

    // itemGroup Mix Slider
    public enum MixModeEnum { Group, Single }
    public enum ColorSelectMode { Color, ColorRange };
    
    // GUI Text 
    public enum HorTextAlign { Left, Right, Center }
    public enum VerTextAlign { Top, Center, Bottom }

    // Global
    public enum StretchMode { None, Left, Right, Screen };

    public enum DropPosition { None, Top, Bottom, Left, Right, Center };

    public enum WrapMode { continuous, Clamp };
    public enum ImageWrapMode { Mirror, Repeat, Clamp };

    
}