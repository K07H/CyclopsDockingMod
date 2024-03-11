using CyclopsDockingMod.Fixers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CyclopsDockingMod
{
    internal static class JsonWriter
    {
        public static bool IsValidJson(string[] lines)
        {
            bool isValid = false;
            int oBracketCnt = 0;
            int cBracketCnt = 0;
            foreach (string line in lines)
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.Contains("\"baseparts\""))
                        isValid = true;
                    if (line.Contains("["))
                        ++oBracketCnt;
                    if (line.Contains("]"))
                        ++cBracketCnt;
                }
            if (!isValid || oBracketCnt != 1 || cBracketCnt != 1)
            {
                Logger.Error("Could not parse base parts from JSON (unexpected format).");
                return false;
            }
            return true;
        }

        public static string ConcatLines(string[] lines)
        {
            string res = "";
            foreach (string line in lines)
                if (!string.IsNullOrWhiteSpace(line))
                    res += line + " ";
            return res;
        }

        public static string ExtractArrayStr(string[] lines)
        {
            string l = ConcatLines(lines);
            int oBracketPos = l.IndexOf("[", StringComparison.InvariantCulture);
            int cBracketPos = l.IndexOf("]", StringComparison.InvariantCulture);
            if (oBracketPos <= 0 || cBracketPos <= 0 || cBracketPos <= (oBracketPos + 1))
            {
                Logger.Error("Could not parse base parts from JSON (unexpected format).");
                return null;
            }
            return l.Substring(oBracketPos + 1, cBracketPos - (oBracketPos + 1));
        }

        public static List<string> ExtractBaseParts(string elems)
        {
            List<string> res = new List<string>();
            string[] parts = elems.Split(new char[] { '{' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts != null && parts.Length > 0)
                foreach (string part in parts)
                    if (!string.IsNullOrWhiteSpace(part) && part.Contains("}"))
                    {
                        int partEndPos = part.IndexOf("}", StringComparison.InvariantCulture);
                        if (partEndPos > 0)
                            res.Add(part.Substring(0, partEndPos));
                    }
            return res;
        }

        public static string TrimVal(string val)
        {
            int stt = val.IndexOf("\"", StringComparison.InvariantCulture);
            int end = val.LastIndexOf("\"", StringComparison.InvariantCulture);
            if (stt >= 0 && end > 0 && end >= (stt + 1))
                return val.Substring(stt + 1, end - (stt + 1));
            return null;
        }

        public static int? TrimInt(string val)
        {
            string intStr = TrimVal(val);
            if (!string.IsNullOrWhiteSpace(intStr) && int.TryParse(intStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
                return parsed;
            return null;
        }

        public static float? TrimFloat(string val)
        {
            string floatStr = TrimVal(val);
            if (!string.IsNullOrWhiteSpace(floatStr) && float.TryParse(floatStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                return parsed;
            return null;
        }

        public static bool? TrimBool(string val)
        {
            string boolStr = TrimVal(val);
            if (!string.IsNullOrWhiteSpace(boolStr) && bool.TryParse(boolStr, out bool parsed))
                return parsed;
            return null;
        }

        public static BasePartSaveData? ParseEntry(string[] details)
        {
            int parsedValCnt = 0;
            BasePartSaveData bp = new BasePartSaveData();
            foreach (string detail in details)
                if (!string.IsNullOrWhiteSpace(detail))
                {
                    string[] entry = detail.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (entry != null && entry.Length == 2 && !string.IsNullOrWhiteSpace(entry[0]) && !string.IsNullOrWhiteSpace(entry[1]))
                    {
                        if (entry[0].Contains("id"))
                        {
                            string parsed = TrimVal(entry[1]);
                            if (parsed != null)
                            {
                                bp.id = parsed;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("cellX"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.cellX = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("cellY"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.cellY = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("cellZ"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.cellZ = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("index"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.index = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("positionX"))
                        {
                            float? parsed = TrimFloat(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.positionX = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("positionY"))
                        {
                            float? parsed = TrimFloat(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.positionY = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("positionZ"))
                        {
                            float? parsed = TrimFloat(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.positionZ = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("type"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.type = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("dock"))
                        {
                            string parsed = TrimVal(entry[1]);
                            if (parsed != null)
                            {
                                bp.dock = parsed;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("signConfig1"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.signConfig1 = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("signConfig2"))
                        {
                            int? parsed = TrimInt(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.signConfig2 = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("signConfig3"))
                        {
                            bool? parsed = TrimBool(entry[1]);
                            if (parsed != null && parsed.HasValue)
                            {
                                bp.signConfig3 = parsed.Value;
                                ++parsedValCnt;
                            }
                        }
                        else if (entry[0].Contains("signConfig4"))
                        {
                            string parsed = TrimVal(entry[1]);
                            if (parsed != null)
                            {
                                bp.signConfig4 = parsed;
                                ++parsedValCnt;
                            }
                        }
                    }
                }

            if (parsedValCnt == 14)
                return bp;
            else
                Logger.Warning($"Could not parse 14 number of values for saved JSON base part. JSON=[{String.Join(",", details)}]");
            return null;
        }

        public static List<BasePartSaveData> ReadJson(string[] lines)
        {
            if (lines == null || lines.Length <= 0)
                return null;

            if (!IsValidJson(lines))
                return null;

            string elems = ExtractArrayStr(lines);
            if (string.IsNullOrWhiteSpace(elems))
                return null;

            List<string> baseParts = ExtractBaseParts(elems);
            if (baseParts == null || baseParts.Count <= 0)
                return null;

            List<BasePartSaveData> res = new List<BasePartSaveData>();
            foreach (string basePart in baseParts)
                if (!string.IsNullOrWhiteSpace(basePart))
                {
                    string[] details = basePart.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (details != null)
                    {
                        if (details.Length == 14)
                        {
                            BasePartSaveData? bp = ParseEntry(details);
                            if (bp != null && bp.HasValue)
                                res.Add(bp.Value);
                        }
                        else
                            Logger.Warning($"Incorrect number of values for saved JSON base part (JSON base part must have 14 values). JSON=[{basePart}]");
                    }
                }
            return res;
        }

        public static string WriteJson(List<BasePartSaveData> baseParts)
        {
            if (baseParts != null && baseParts.Count > 0)
            {
                bool isFirst = true;
                string res = "{ \"baseparts\": [" + Environment.NewLine;
                foreach (BasePartSaveData bp in baseParts)
                {
                    if (!isFirst)
                        res += ("," + Environment.NewLine);
                    res += ("\t{" + Environment.NewLine);
                    res += ($"\t\t\"id\": \"{bp.id}\"," + Environment.NewLine);
                    res += ($"\t\t\"cellX\": \"{bp.cellX.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"cellY\": \"{bp.cellY.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"cellZ\": \"{bp.cellZ.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"index\": \"{bp.index}\"," + Environment.NewLine);
                    res += ($"\t\t\"positionX\": \"{bp.positionX.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"positionY\": \"{bp.positionY.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"positionZ\": \"{bp.positionZ.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"type\": \"{bp.type.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"dock\": \"{bp.dock}\"," + Environment.NewLine);
                    res += ($"\t\t\"signConfig1\": \"{bp.signConfig1.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"signConfig2\": \"{bp.signConfig2.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"signConfig3\": \"{bp.signConfig3.ToString(CultureInfo.InvariantCulture)}\"," + Environment.NewLine);
                    res += ($"\t\t\"signConfig4\": \"{bp.signConfig4}\"" + Environment.NewLine);
                    res += "\t}";
                    isFirst = false;
                }
                res += (Environment.NewLine + "] }");
                return res;
            }
            return "";
        }
    }
}
