using System;
using System.IO;
using System.Linq;
using InfinityScript;

public class gamemodes : BaseScript
{
    private static string[] gametypes = new string[26] {"AR", "AUGfected", "CoD4 Rem FFA", "CoD4 Rem TDM", "exoFFA", "exoTDM", "Fusion", "GTNWS", "IMH", "JuggAttack", "LotD", "LotD2", "LotD3", "LotD4", "Mascot", "MW2 Rem FFA", "MW2 Rem TDM", "PerkRoulette", "Primed", "PropHunt", "Reverse_Inf", "Streakfected", "Thrusters", "TRotater", "WeapRoul", "Uplink" };
    private static string[] gametypes_name = new string[26] {"Arms Race", "AUGfected", "CoD4 Reminission Mode: FFA", "CoD4 Reminission Mode: TDM", "Exo-Suit FFA", "Exo-Suit TDM", "Fusion", "Global Thermo-Nuclear War S", "Infected Man's Hand", "Juggernaut Invasion", "Luck of the Draw", "Luck of the Draw", "Luck of the Draw", "Luck of the Draw", "Mascot", "MW2 Reminission Mode: FFA", "MW2 Reminission Mode: TDM", "Perk Roulette", "Primed", "Prop Hunt", "Reverse Infected", "Streakfected", "Thrusters", "Team Rotater", "Weapon Roulette", "Uplink" };
    private static string[] gametypes_serverName = new string[26] {"Arms Race", "AUGfected", "CoD4 FFA", "CoD4 TDM", "Exo FFA", "Exo TDM", "Fusion", "GTNWS", "IMH", "Jugg Attack", "LotD", "LotD", "LotD", "LotD", "Mascot", "MW2 FFA", "MW2 TDM", "Perk Roulette", "Primed", "Prop Hunt", "Reverse Infect", "Streakfected", "Thrusters", "Team Rotater", "Gun Roulette", "Uplink" };
    //private static Random random = new Random();

    public gamemodes()
    {
        GSCFunctions.SetDvarIfUninitialized("sv_gametypeName", "");
        OnInterval(100, setCurrentGametype);

        //Set high quality voice chat audio
        GSCFunctions.SetDvar("sv_voiceQuality", 9);
        GSCFunctions.SetDvar("maxVoicePacketsPerSec", 2000);
        GSCFunctions.SetDvar("maxVoicePacketsPerSecForServer", 1000);

        PlayerConnected += (player) => player.SetClientDvars("maxVoicePacketsPerSec", 2000, "maxVoicePacketsPerSecForServer", 1000);
    }
    private static bool setCurrentGametype()
    {
        //yield return Wait(.2f);

        int randomNextMap = GSCFunctions.RandomInt(gametypes.Length);
        string nextmap = GSCFunctions.GetDvar("nextmap");
        if (nextmap == "")
            return true;
        string currentMap = GSCFunctions.GetDvar("mapname");
        string currentGamemode = nextmap.Substring(currentMap.Length + 1);
        //Utilities.PrintToConsole(currentGamemode);
        if (!gametypes.Contains(currentGamemode))
            return false;
        string currentGametypeName = gametypes_name[Array.IndexOf(gametypes, currentGamemode)];

        Log.Debug("Current Gamemode is {0}; Current Gamemode Name is {1}", currentGamemode, currentGametypeName);

        //setup current gametype elements
        if (currentGamemode != "AIZ" && currentGamemode != "PropHunt")
        {
            HudElem info = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, .7f);
            info.SetPoint("TOP CENTER", "top center", 0, 1);
            info.HideWhenInMenu = true;
            info.HideWhenInDemo = false;
            info.HideIn3rdPerson = false;
            info.HideWhenDead = false;
            info.Archived = true;
            info.SetText("Gamemode: ^2" + currentGametypeName);
        }

        Log.Debug("Server gamemode is {0}", gametypes_serverName[Array.IndexOf(gametypes_name, currentGametypeName)]);
        GSCFunctions.SetDvar("sv_gametypeName", gametypes_serverName[Array.IndexOf(gametypes_name, currentGametypeName)]);

        setNextGametype();

        return false;
    }
    public static void setNextGametype()
    {
        string[] mapList = new string[16]{"mp_alpha", "mp_bootleg", "mp_bravo", "mp_carbon", "mp_dome",
                "mp_exchange", "mp_hardhat", "mp_interchange", "mp_lambeth", "mp_mogadishu", "mp_paris",
                "mp_plaza2","mp_radar", "mp_seatown", "mp_underground", "mp_village"};
        string nextMapname = mapList[GSCFunctions.RandomInt(16)];
        string nextGamemode = gametypes[GSCFunctions.RandomInt(gametypes.Length)];
        Log.Debug("Next Map is {0}; Next Gametype {1}", nextMapname, nextGamemode);
        using (StreamWriter rotation = new StreamWriter("admin\\FGMBuffer.dspl", false))
        {
            rotation.WriteLine(nextMapname + "," + nextGamemode + ",1");
            rotation.Flush();
            rotation.Close();
            rotation.Dispose();
        }
        int count = 0;
        OnInterval(1000, () => {GSCFunctions.SetDvar("nextmap", nextMapname + " " + nextGamemode); count++; if (count == 2) return false; return true; });
    }
}