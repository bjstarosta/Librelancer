﻿// MIT License - Copyright (c) Malte Rupprecht, Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibreLancer.Ini;
using LibreLancer.Dll;
    
namespace LibreLancer.Data
{
	public class FreelancerIni : IniFile
	{
		public List<DllFile> Resources { get; private set; }
		public List<string> StartupMovies { get; private set; }

		public string DataPath { get; private set; }
		public string SolarPath { get; private set; }
		public string UniversePath { get; private set; }
		public string HudPath { get; private set; }
        public string XInterfacePath { get; private set; }
        public string DataVersion { get; private set; }

        public List<string> EquipmentPaths { get; private set; }
		public List<string> LoadoutPaths { get; private set; }
		public List<string> ShiparchPaths { get; private set; }
        public List<string> GoodsPaths { get; private set; }
        public List<string> MarketsPaths { get; private set; }
        public List<string> SoundPaths { get; private set; }
		public List<string> GraphPaths { get; private set; }
		public List<string> EffectPaths { get; private set; }
		public List<string> AsteroidPaths { get; private set; }
		public List<string> RichFontPaths { get; private set; }
        public List<string> FontPaths { get; private set;  }
        public List<string> PetalDbPaths { get; private set; }
        public List<string> FusePaths { get; private set;  }
        public List<string> NewCharDBPaths { get; private set;  }
        
        public List<string> VoicePaths { get; private set; }

        public string StarsPath { get; private set; }
		public string BodypartsPath { get; private set; }
		public string CostumesPath { get; private set; }
		public string EffectShapesPath { get; private set; }
		public Tuple<string, string> JsonResources { get; private set; }

        public List<string> NoNavmapSystems { get; private set; }
        static readonly string[] NoNavmaps = {
            "St02c",
            "St03b",
            "St03",
            "St02"
        };
        public List<string> HiddenFactions { get; private set;  }
        static readonly string[] NoShowFactions =  {
            "fc_uk_grp",
            "fc_ouk_grp",
            "fc_q_grp",
            "fc_f_grp",
            "fc_or_grp",
            "fc_n_grp",
            "fc_rn_grp",
            "fc_kn_grp",
            "fc_ln_grp"
        };
        public FreelancerIni(FileSystem vfs) : this("EXE\\freelancer.ini", vfs) { }

        public FreelancerIni (string path, FileSystem vfs)
		{
			EquipmentPaths = new List<string> ();
			LoadoutPaths = new List<string> ();
			ShiparchPaths = new List<string> ();
			SoundPaths = new List<string>();
			GraphPaths = new List<string>();
			EffectPaths = new List<string>();
			AsteroidPaths = new List<string> ();
			RichFontPaths = new List<string>();
            FontPaths = new List<string>();
			PetalDbPaths = new List<string>();
			StartupMovies = new List<string>();
            GoodsPaths = new List<string>();
            MarketsPaths = new List<string>();
            FusePaths = new List<string>();
            NewCharDBPaths = new List<string>();
            VoicePaths = new List<string>();
            bool extNoNavmaps = false;
            bool extHideFac = false;
            NoNavmapSystems = new List<string>(NoNavmaps);
            HiddenFactions = new List<string>(NoShowFactions);

            //For DLL resolving (skip VFS for editor usage)
            var fullPath = vfs == null ? path : vfs.Resolve(path);
            var directory = Path.GetDirectoryName(fullPath);
            var dirFiles = Directory.GetFiles(directory).Select(a => Path.GetFileName(a));
            Func<string, string> resolveFileEXE = (x) =>
            {
                if (File.Exists(Path.Combine(directory, x))) return Path.Combine(directory, x);
                var res = dirFiles.FirstOrDefault(y => y.Equals(x, StringComparison.OrdinalIgnoreCase));
                if (res != null) return Path.Combine(directory, res);
                return null;
            };

            foreach (Section s in ParseFile(fullPath, vfs)) {
				switch (s.Name.ToLowerInvariant ()) {
				case "freelancer":
					foreach (Entry e in s) {
						if (e.Name.ToLowerInvariant () == "data path") {
							if (e.Count != 1)
								throw new Exception ("Invalid number of values in " + s.Name + " Entry " + e.Name + ": " + e.Count);
							if (DataPath != null)
								throw new Exception ("Duplicate " + e.Name + " Entry in " + s.Name);
							DataPath = "EXE\\" + e [0].ToString () + "\\";
						}
					}
					break;
				case "jsonresources":
					JsonResources = new Tuple<string, string>(resolveFileEXE(s[0][0].ToString()), resolveFileEXE(s[0][1].ToString()));
					break;
				case "resources":
                    Resources = new List<DllFile> ();
                    //NOTE: Freelancer hardcodes resources.dll
                    string pathStr;
                    if ((pathStr = resolveFileEXE("resources.dll")) != null)
                        Resources.Add(new DllFile(pathStr, vfs));
                    else
                        FLLog.Warning("Dll", "resources.dll not found");
					foreach (Entry e in s)
					{
						if (e.Name.ToLowerInvariant () != "dll")
							continue;
                        if ((pathStr = resolveFileEXE(e[0].ToString())) != null)
                            Resources.Add(new DllFile(pathStr, vfs));
                        else
                            FLLog.Warning("Dll", e[0].ToString());
					}
					break;
				case "startup":
					foreach (Entry e in s) {
						if (e.Name.ToLowerInvariant () != "movie_file")
							continue;
						StartupMovies.Add (e [0].ToString());
					}
					break;
                case "extended":
                        foreach(Entry e in s) {
                            switch(e.Name.ToLowerInvariant())
                            {
                                case "xinterface":
                                    if (System.IO.Directory.Exists(e[0].ToString()))
                                        XInterfacePath = e[0].ToString();
                                    else
                                        XInterfacePath = DataPath + e[0].ToString();
                                    if (!XInterfacePath.EndsWith("\\",StringComparison.InvariantCulture) && 
                                        !XInterfacePath.EndsWith("/",StringComparison.InvariantCulture))
                                        XInterfacePath += "/";
                                    break;
                                case "dataversion":
                                    DataVersion = e[0].ToString();
                                    break;
                                case "nonavmap":
                                    if (!extNoNavmaps) { NoNavmapSystems = new List<string>(); extNoNavmaps = true; }
                                    NoNavmapSystems.Add(e[0].ToString());
                                    break;
                                case "hidefaction":
                                    if (!extHideFac) { HiddenFactions = new List<string>();  extHideFac = true; };
                                    HiddenFactions.Add(e[0].ToString());
                                    break;
                            }
                        }
                        break;
				case "data":
					foreach (Entry e in s) {
						switch (e.Name.ToLowerInvariant ()) {
						case "solar":
							SolarPath = DataPath + e [0].ToString ();
							break;
						case "universe":
							UniversePath = DataPath + e [0].ToString ();
							break;
						case "equipment":
							EquipmentPaths.Add(DataPath + e [0].ToString ());
							break;
						case "loadouts":
							LoadoutPaths.Add(DataPath + e [0].ToString ());
							break;
						case "stars":
							StarsPath = DataPath + e [0].ToString ();
							break;
						case "bodyparts":
							BodypartsPath = DataPath + e [0].ToString ();
							break;
						case "costumes":
							CostumesPath = DataPath + e [0];
							break;
						case "sounds":
							SoundPaths.Add(DataPath + e[0]);
							break;
						case "ships":
							ShiparchPaths.Add (DataPath + e [0].ToString ());
							break;
						case "rich_fonts":
							RichFontPaths.Add(DataPath + e[0].ToString());
							break;
                        case "fonts":
                            FontPaths.Add(DataPath + e[0].ToString());
                            break;
						case "igraph":
							GraphPaths.Add(DataPath + e[0].ToString());
							break;
						case "effect_shapes":
							EffectShapesPath = DataPath + e[0].ToString();
							break;
						case "effects":
							EffectPaths.Add(DataPath + e[0].ToString());
							break;
						case "asteroids":
							AsteroidPaths.Add (DataPath + e [0].ToString ());
							break;
						case "petaldb":
							PetalDbPaths.Add(DataPath + e[0].ToString());
							break;
						case "hud":
							HudPath = DataPath + e[0].ToString();
							break;
                        case "goods":
                            GoodsPaths.Add(DataPath + e[0].ToString());
                            break;
                        case "markets":
                            MarketsPaths.Add(DataPath + e[0].ToString());
                            break;
                        case "fuses":
                            FusePaths.Add(DataPath + e[0].ToString());
                            break;
                        case "newchardb":
                            NewCharDBPaths.Add(DataPath + e[0].ToString());
                            break;
                        case "voices":
                            VoicePaths.Add(DataPath + e[0].ToString());
                            break;
						}
					}
					break;
				}
			}
		}
	}
}

