﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlueprintEditor2
{
    class MyDisplayBlueprint
    {
        //public string[] Elements { get; private set; }
        public string Name { get; private set; }
        public string Owner { get; private set; }
        public DateTime CreationTime { get; private set; }
        public string CreationTimeText => CreationTime.ToString("d");
        public DateTime LastEditTime { get; private set; }
        public string LastEditTimeText => LastEditTime.ToString("d");
        public int BlockCount { get; private set; }
        public string BlockCountText => BlockCount.ToString("N0", CultureInfo.InvariantCulture);
        public int GridCount { get; private set; }
        public string GridCountText => GridCount.ToString("N0", CultureInfo.InvariantCulture);

        private MyDisplayBlueprint()
        {

        }
        public void AddXmlData(MyXmlBlueprint blueprint)
        {
            if (blueprint.Owner == MySettings.Current.SteamID)
                Owner = MySettings.Current.UserName;
            else
                Owner = blueprint.DisplayName;
            int blcnt = 0;
            foreach (MyXmlGrid grd in blueprint.Grids)
                blcnt += grd.Blocks.Count;
            BlockCount = blcnt;
            GridCount = blueprint.Grids.Length;
        }
        static public MyDisplayBlueprint FromBlueprint(string Bluepath)
        {
            if (File.Exists(Bluepath + "\\bp.sbc"))
            {
                MyDisplayBlueprint Elem = new MyDisplayBlueprint
                {
                    Name = Bluepath.Split('\\').Last()
                };
                FileInfo Inf = new FileInfo(Bluepath + "\\bp.sbc");
                Elem.LastEditTime = Inf.LastWriteTime;
                Elem.CreationTime = Inf.CreationTime;
                if (!MySettings.Current.DontOpenBlueprintsOnScan)
                {
                    XmlDocument BlueprintXml = new XmlDocument();
                    BlueprintXml.Load($"{Bluepath}\\bp.sbc");
                    foreach(XmlNode x in BlueprintXml.GetElementsByTagName("ProjectedGrids")){
                        x.InnerXml = "";
                    }
                    string SteamID = BlueprintXml.GetElementsByTagName("OwnerSteamId").Item(0).InnerText;
                    if (SteamID == MySettings.Current.SteamID)
                        Elem.Owner = MySettings.Current.UserName;
                    else
                        Elem.Owner = BlueprintXml.GetElementsByTagName("DisplayName").Item(0).InnerText;
                    Elem.BlockCount = BlueprintXml.GetElementsByTagName("MyObjectBuilder_CubeBlock").Count;
                    Elem.GridCount = BlueprintXml.GetElementsByTagName("CubeGrid").Count;
                }
                else
                {
                    Elem.Owner = "";
                    Elem.BlockCount = 0;
                    Elem.GridCount = 0;
                }

                return Elem;
            }
            return null;
        }
    }
}
