using System;
using System.Collections.Generic;
using System.Drawing.Text;

namespace TerrainGenerator.ThreeDFile
{
    /*
    ; FBX 7.1.0 project file
    ; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.
    ; All rights reserved.
    ; ----------------------------------------------------
    FBXHeaderExtension: {
        ; header information: global file information.
    FBXHeaderVersion: 1003
    FBXVersion: 7100
    CreationTimeStamp:  {
    Version: 1000
    Year: 2010
    Month: 1
    Day: 19
    Hour: 16
    Minute: 30
    Second: 28
    Millisecond: 884
    }
    Creator: "FBX SDK/FBX Plugins version 2011.2"
    SceneInfo: "SceneInfo::GlobalInfo", "UserData" {
    ...
    }
    GlobalSettings: {
    Version: 1000
    Properties70:  {
    P: "UpAxis", "int", "Integer", "",1
    P: "UpAxisSign", "int", "Integer", "",1
    P: "FrontAxis", "int", "Integer", "",2
    P: "FrontAxisSign", "int", "Integer", "",1
    P: "CoordAxis", "int", "Integer", "",0
    P: "CoordAxisSign", "int", "Integer", "",1
    P: "OriginalUpAxis", "int", "Integer", "",-1
    P: "OriginalUpAxisSign", "int", "Integer", "",1
    P: "UnitScaleFactor", "double", "Number", "",1
    P: "OriginalUnitScaleFactor", "double", "Number", "",1
    P: "AmbientColor", "ColorRGB", "Color", "",0,0,0
    P: "DefaultCamera", "KString", "", "", "Producer Perspective"
    P: "TimeMode", "enum", "", "",6
    P: "TimeSpanStart", "KTime", "Time", "",0
    P: "TimeSpanStop", "KTime", "Time", "",46186158000
    }
    }
    ...
    ; Object definitions
    ;------------------------------------------------------------------
    Definitions: {
    Version: 100
    Count: 2251
    ObjectType: "GlobalSettings" {
    Count: 1
    }
    ObjectType: "Model" {
    Count: 86
    PropertyTemplate: "KFbxNode" {
    Properties70:  {
    P: "QuaternionInterpolate", "bool", "", "",0
    P: "RotationOffset", "Vector3D", "Vector", "",0,0,0
    P: "RotationPivot", "Vector3D", "Vector", "",0,0,0
    P: "ScalingOffset", "Vector3D", "Vector", "",0,0,0
    P: "ScalingPivot", "Vector3D", "Vector", "",0,0,0
    ...}
    ObjectType: "Material" {
    Count: 1
    PropertyTemplate: "KFbxSurfacePhong" {
    Properties70:  {
    P: "ShadingModel", "KString", "", "", "Phong"
    P: "MultiLayer", "bool", "", "",0
    P: "EmissiveColor", "ColorRGB", "Color", "",0,0,0
    P: "EmissiveFactor", "double", "Number", "",1
    P: "AmbientColor", "ColorRGB", "Color", "",0.2,0.2,0.2
    ...}
    Model: 21883936, "Model::Humanoid:Hips", "LimbNode" {
    Version: 232
    Properties70:  {
    P: "ScalingMin", "Vector3D", "Vector", "",1,1,1
    P: "NegativePercentShapeSupport", "bool", "", "",0
    P: "DefaultAttributeIndex", "int", "Integer", "",0
    P: "Lcl Translation", "Lcl Translation", "", "A+",-271.281097412109,-762.185852050781,528.336242675781
    P: "Lcl Rotation", "Lcl Rotation", "", "A+",-1.35128843784332,2.6148145198822,0.42334708571434
    P: "Lcl Scaling", "Lcl Scaling", "", "A+",1,0.99999988079071,1
    ...

    */

    internal class Fbx
    {
        private const string headerComment =
            "; FBX 7.1.0 project file\r\n; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.\r\n; All rights reserved.\r\n; ----------------------------------------------------";

        private class FBXHeaderExtension
        {
            internal int FBXHeaderVersion = 1003;
            internal int FBXVersion = 7100;

            internal class CreationTimeStamp
            {
                private int Version = 1000;
                private int Year = 2010;
                private int Month = 1;
                private int Day = 19;
                private int Hour = 16;
                private int Minute = 30;
                private int Second = 28;
                private int Millisecond = 884;
            }

            internal string Creator = "FBX SDK/FBX Plugins version 2011.2";
            internal string SceneInfo = "\"SceneInfo::GlobalInfo\", \"UserData {\r\n}";

            internal class GlobalSettings
            {
                private int Version = 1000;
                // Tuple list format P: "UpAxisSign", "int", "Integer", "", 1
                internal List<List<object>> Properties70 = new List<List<object>>
                {
                    new List<object> {"UpAxis", "int", "Integer", "", 1},
                    new List<object> {"UpAxisSign", "int", "Integer", "", 1},
                    new List<object> {"FrontAxis", "int", "Integer", "", 2},
                    new List<object> {"FrontAxisSign", "int", "Integer", "", 1},
                    new List<object> {"CoordAxis", "int", "Integer", "", 0},
                    new List<object> {"CoordAxisSign", "int", "Integer", "", 1},
                    new List<object> {"OriginalUpAxis", "int", "Integer", "", -1},
                    new List<object> {"OriginalUpAxisSign", "int", "Integer", "", 1},
                    new List<object> {"UnitScaleFactor", "double", "Number", "", 1},
                    new List<object> {"OriginalUnitScaleFactor", "double", "Number", "", 1},
                    new List<object> {"AmbientColor", "ColorRGB", "Color", "", 0, 0, 0},
                    new List<object> {"DefaultCamera", "KString", "", "", "Producer Perspective"},
                    new List<object> {"TimeMode", "enum", "", "", 6},
                    new List<object> {"TimeSpanStart", "KTime", "Time", "", 0},
                    new List<object> {"TimeSpanStop", "KTime", "Time", "", 46186158000}
                };
            }

            // Object definitions
            // ------------------------------------------------------------------

            private class Definitions
            {
                internal int Version = 100;
                internal int Count = 2251;

                internal class ObjectType
                {
                }

                internal class PropertyTemplate
                {
                }

                internal class GlobalSettings : ObjectType
                {
                    internal int Count = 1;
                }

                class Model : ObjectType
                {
                    internal int Count = 86;

                    internal class KFbxNode : PropertyTemplate
                    {
                        internal List<List<object>> Properties70 = new List<List<object>>
                        {
                            new List<object> {"QuaternionInterpolate", "bool", "", "", 0},
                            new List<object> {"RotationOffset", "Vector3D", "Vector", "", 0, 0, 0 },
                            new List<object> {"RotationPivot", "Vector3D", "Vector", "", 0, 0, 0 },
                            new List<object> {"ScalingOffset", "Vector3D", "Vector", "", 0, 0, 0 },
                            new List<object> {"ScalingPivot", "Vector3D", "Vector", "", 0, 0, 0 }
                        };
                    }
                }
            }
        }
    }
}
