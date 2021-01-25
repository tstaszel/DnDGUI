using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Windows.Media.Brushes;

namespace DnDGUI
{
    /// <summary>
    ///     Interaction logic for MonsterStats.xaml
    /// </summary>
    public partial class MonsterStats : Window
    {
        public List<Run> ColorWrite(string text)
        {
            var alignColors = new[] {Green, White, Red};

            var posNegColors = new[] {Red, Green};

            //Regex will see [#blue] 
            //this will be case sensitive 
            List<(string, Brush)> stringCache = new()
                {(text.Replace("\r", "").Replace("[#w]", "[#white]"), White)};
            while (stringCache.Any(sc => Regex.IsMatch(sc.Item1, @"\[#(.+?)\]")))
            {
                var outliers = stringCache.Where(sc => Regex.IsMatch(sc.Item1, @"\[#(.+?)\]")).ToArray();
                for (int i = 0; i < outliers.Length; i++)
                {
                    var (split, color) = outliers[i];
                    var startIndex = stringCache.IndexOf((split, color));
                    stringCache.Remove((split, color));
                    var splitColorRaw = Regex.Match(split, @"\[#(.+?)\]").Groups[1].Value;
                    var splitColor = (Brush) new BrushConverter().ConvertFromString(splitColorRaw);
                    var continueSplit = split.Split($"[#{splitColorRaw}]");
                    stringCache.Insert(startIndex, (continueSplit[0], color));
                    stringCache.InsertRange(startIndex + 1, continueSplit[1..].Select(s => (s, splitColor)));
                }
            }

            List<Run> runs = new();
            foreach (var (subtext, color) in stringCache)
            {
                runs.Add(new Run(subtext) {Foreground = color});
            }

            return runs;
        }


        public List<Run> PrintEntity(Core.Entity entity)
        {
            var alignColors = new[] {Green, White, Red};

            var posNegColors = new[] {Red, Green};
            StringBuilder sb = new StringBuilder($@"[#darkmagenta]Entity Name[#w]:
[#darkred]{entity.Name}[#w]
[#darkmagenta]Entity Type[#w]: 
[#yellow]{entity.Type}[#w]
");

            //This is to filter out items in a list
            void ExpandList(IEnumerable<(string, int)> list, string name)
            {
                sb.Append($"[#darkmagenta]{name}[#w]: \n");
                var valueTuples = list as (string, int)[] ?? list.ToArray();

                if (!valueTuples.Any()) sb.Append($"[#yellow]N/A \n");

                else
                    foreach (var objects in valueTuples)
                        sb.Append(
                            $"{objects.Item1} ([#{posNegColors[objects.Item2 >= 0 ? 1 : 0]}]{objects.Item2}[#w]) \n");
            }

            sb.Append($"[#darkmagenta]Entity Alignment[#w]: \n");
            if (entity.Alignment.Item1 != Core.AlignmentType.Unaligned)
                sb.Append($"[#{alignColors[(int) entity.Alignment.Item1]}]{entity.Alignment.Item1}" +
                          $" [#{alignColors[(int) entity.Alignment.Item2]}]{entity.Alignment.Item2}[#w] \n");
            else sb.Append($"[#gray]{entity.Alignment.Item1} \n");

            //Coloring Armor Class
            sb.Append($"[#darkmagenta]Armor Class[#w]: \n" +
                      $"{entity.ArmorClass} \n");

            //Coloring Hit Points
            sb.Append($"[#darkmagenta]Hit Points[#w]: \n" +
                      $"[#darkgreen]{entity.HitPoints.Item1}[#w] " +
                      $"([#darkgreen]{entity.HitPoints.Item2}[#w]) \n");

            //Coloring Speed and Movement
            sb.Append($"[#darkmagenta]Movement Speeds[#w]: \n{entity.Speed}ft");
            if (entity.Climb != 0) sb.Append($" | {entity.Climb}ft");
            if (entity.Fly != 0) sb.Append($" | {entity.Fly}ft");
            if (entity.Swim != 0) sb.Append($" | {entity.Swim}ft");
            if (entity.Burrow != 0) sb.Append($" | {entity.Burrow}ft");
            sb.Append("\n");


            //Coloring Stats
            sb.Append($"[#darkmagenta]Stats[#w]: \n");
            //Dictionary changes location from nth to string, changing key from int to something else
            //Lets us skip like 1, 4, 6, 9 
            Dictionary<string, Brush> statsColor = new()
                {{"Str", Red}, {"Dex", Green}, {"Con", Yellow}, {"Int", Blue}, {"Wis", Cyan}, {"Cha", Magenta}};
            for (int i = 0; i < statsColor.Count; i++)
            {
                var stat = statsColor.Keys.ToArray()[i];
                sb.Append($"[#{statsColor[stat]}]{stat}[#w]:" + $" {entity.Stats[i].Item1} " +
                          $"([#{posNegColors[entity.Stats[i].Item2 >= 0 ? 1 : 0]}]{entity.Stats[i].Item2}[#w]) \n");
            }


            //Expand Saving Throws List + Color
            sb.Append($"[#darkmagenta]Saving Throws[#w]: \n");
            if (!entity.SavingThrows.Any()) sb.Append("[#yellow]N/A \n");
            else
            {
                foreach (var objects in entity.SavingThrows)
                {
                    var stat = objects.Item1;
                    sb.Append($"[#{statsColor[stat]}]{objects.Item1}[#w]: " +
                              $"([#{posNegColors[objects.Item2 >= 0 ? 1 : 0]}]{objects.Item2}[#w]) \n");
                }
            }

            //Expand Skills List
            ExpandList(entity.Skills, "Skills");

            //Coloring Vulnerabilities
            sb.Append($"[#darkmagenta]Vulnerabilities[#w]: \n");
            sb.Append(entity.Vulnerabilities.Contains("N/A")
                ? $"[#yellow]{entity.Vulnerabilities}\n"
                : $"{entity.Vulnerabilities}\n");

            //Coloring Immunities
            sb.Append($"[#darkmagenta]Immunities[#w]: \n");
            sb.Append(entity.Immunities.Contains("N/A") ? $"[#yellow]{entity.Immunities}\n" : $"{entity.Immunities}\n");

            //Coloring Condition Immunities
            sb.Append($"[#darkmagenta]Condition Immunities[#w]:  \n");
            sb.Append(entity.ConditionImmunities.Contains("N/A")
                ? $"[#yellow]{entity.ConditionImmunities} \n"
                : $"{entity.ConditionImmunities} \n");

            //Coloring Senses
            ExpandList(entity.Senses, "Senses");

            //Coloring Languages
            sb.Append($"[#darkmagenta]Languages[#w]: \n");
            sb.Append(entity.Languages.Contains("N/A") ? $"[#yellow]{entity.Languages} \n" : $"{entity.Languages}\n");

            //Coloring Challenge
            sb.Append($"[#darkmagenta]Challenge[#w]: \n" +
                      $"[#darkred]{entity.Challenge.Item1}[#w]" +
                      $"([#darkred]{entity.Challenge.Item2}[#w]) \n");

            //Coloring Abilities
            sb.Append($"[#darkmagenta]Abilities[#w]: \n{entity.Abilities} \n");

            //Coloring Actions
            sb.Append($"[#darkmagenta]Actions[#w]: \n{entity.Actions.Replace(". ", ". \n")}");

            ColorWrite(sb.ToString());
            return ColorWrite(sb.ToString());
        }


        public MonsterStats(Core.Entity entity)
        {
            InitializeComponent();
            MonStat.FontSize = 14;
            MonStat.Inlines.AddRange(PrintEntity(entity));
            Show();
        }
    }
}