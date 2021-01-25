using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DnDGUI
{
    public class Core
    {
        public enum AlignmentType
        {
            Lawful,
            Neutral,
            Chaotic,
            Unaligned
        }

        public enum AlignmentExtent
        {
            Good,
            Neutral,
            Evil,
            Unaligned
        }

        public record Entity(
            string Name,
            string Type,
            (AlignmentType, AlignmentExtent) Alignment,
            int ArmorClass,
            (int, string) HitPoints,
            int Speed,
            (int, int)[] Stats,
            List<(string, int)> SavingThrows,
            List<(string, int)> Skills,
            List<(string, int)> Senses,
            string ConditionImmunities,
            string Languages,
            (string, int) Challenge,
            string Abilities,
            string Actions,
            string Vulnerabilities = "N/A",
            string Immunities = "N/A",
            int Swim = 0,
            int Burrow = 0,
            int Climb = 0,
            int Fly = 0);
    }

    public class DataReader
    {
        public static Dictionary<string, Core.Entity> EntityData = new();

        //This will save off to AppData Roaming based on user 
        public static string coreDir =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/tstaszel/DnDcode";

        public static bool CheckDir(string dir) => Directory.Exists(dir) || File.Exists(dir);

        public static void Save()
        {
            var textInput = JsonConvert.SerializeObject(EntityData);
            if (!CheckDir(coreDir)) Directory.CreateDirectory(coreDir);
            //I could have chosen any kid of name
            using var sw = File.CreateText($"{coreDir}/MonsterData.json");
            sw.Write(textInput);
            sw.Close();
        }

        public static void Load()
        {
            if (CheckDir(coreDir) && CheckDir($"{coreDir}/MonsterData.json"))
            {
                using var sr = new StreamReader($"{coreDir}/MonsterData.json");
                EntityData = JsonConvert.DeserializeObject<Dictionary<string, Core.Entity>>(sr.ReadToEnd());
                sr.Close();
            }
        }
    }
}
