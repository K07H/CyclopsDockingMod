namespace CyclopsDockingMod.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Logger = Logger;

public class Route
{
    public int Id { get; set; } = -1;

    public string Name { get; set; } = "Route";

    public float Length { get; set; }

    public int Speed { get; set; } = 2;

    public Vector3? BasePartPosStt { get; set; }

    public Vector3? BasePartPosEnd { get; set; }

    private static bool SimplifyCurve = true;

    private List<Vector3> _simplifiedWayPoints;

    private readonly List<Vector3> _wayPoints = new List<Vector3>();

    public List<Vector3> WayPoints
    {
        get
        {
            if (!SimplifyCurve)
                return this._wayPoints;
            if (this._simplifiedWayPoints == null)
            {
                List<Vector3> list = new List<Vector3>();
                LineUtility.Simplify(this._wayPoints, 0.1f, list);
                this._simplifiedWayPoints = list;
            }
            return this._simplifiedWayPoints;
        }
    }

    public Route(int id, string name, List<Vector3> wayPoints = null, float length = 0f, int speed = 2, Vector3? basePartPosStt = null, Vector3? basePartPosEnd = null)
    {
        if (wayPoints != null)
            foreach (Vector3 vector in wayPoints)
                this._wayPoints.Add(vector);
        this.Id = id;
        this.Name = name;
        this.Length = length;
        this.Speed = speed;
        this.BasePartPosStt = basePartPosStt;
        this.BasePartPosEnd = basePartPosEnd;
    }

    public static Route Deserialize(string serial)
    {
        if (!string.IsNullOrEmpty(serial))
        {
            string[] array = serial.Split(new char[] { '/' }, StringSplitOptions.None);
            if (array != null && array.Length == 11)
            {
                int num;
                float num2;
                int num3;
                if (int.TryParse(array[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out num) && num >= 0 && float.TryParse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out num2) && num2 >= 0f && int.TryParse(array[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out num3) && num3 >= 1 && num3 <= 3)
                {
                    Vector3? vector = null;
                    Vector3? vector2 = null;
                    string text = ((!string.IsNullOrEmpty(array[1])) ? array[1] : string.Format(AutoPilot.Lbl_DefaultRouteName, (num + 1).ToString()));
                    float num4;
                    float num5;
                    float num6;
                    if (array[4] != "?" && float.TryParse(array[4], NumberStyles.Float, CultureInfo.InvariantCulture, out num4) && float.TryParse(array[5], NumberStyles.Float, CultureInfo.InvariantCulture, out num5) && float.TryParse(array[6], NumberStyles.Float, CultureInfo.InvariantCulture, out num6))
                        vector = new Vector3?(new Vector3(num4, num5, num6));
                    float num7;
                    float num8;
                    float num9;
                    if (array[7] != "?" && float.TryParse(array[7], NumberStyles.Float, CultureInfo.InvariantCulture, out num7) && float.TryParse(array[8], NumberStyles.Float, CultureInfo.InvariantCulture, out num8) && float.TryParse(array[9], NumberStyles.Float, CultureInfo.InvariantCulture, out num9))
                        vector2 = new Vector3?(new Vector3(num7, num8, num9));
                    List<Vector3> list = new List<Vector3>();
                    if (!string.IsNullOrEmpty(array[10]))
                    {
                        string[] array2 = array[10].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                        if (array2 != null && array2.Length != 0)
                        {
                            string[] array3 = array2;
                            for (int i = 0; i < array3.Length; i++)
                            {
                                string[] array4 = array3[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                float num10;
                                float num11;
                                float num12;
                                if (array4 != null && array4.Length == 3 && float.TryParse(array4[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num10) && float.TryParse(array4[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num11) && float.TryParse(array4[2], NumberStyles.Float, CultureInfo.InvariantCulture, out num12))
                                    list.Add(new Vector3(num10, num11, num12));
                            }
                        }
                    }
                    return new Route(num, text, list, num2, num3, vector, vector2);
                }
                int num13 = 0;
                if (!string.IsNullOrEmpty(array[10]))
                {
                    string[] array5 = array[10].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    if (array5 != null && array5.Length != 0)
                        num13 = array5.Length;
                }
                if (num13 > 5)
                    Logger.Log($"WARNING: Could not load Cyclops auto-pilot route. Serial=[{array[0]}/{array[1]}/{array[2]}/{array[3]}/{array[4]}/{array[5]}/{array[6]}/{array[7]}/{array[8]}/{array[9]}/...] NbPoints=[{num13}]");
                else
                    Logger.Log($"WARNING: Could not load Cyclops auto-pilot route. Serial=[{array[0]}/{array[1]}/{array[2]}/{array[3]}/{array[4]}/{array[5]}/{array[6]}/{array[7]}/{array[8]}/{array[9]}/{array[10]}]");
            }
        }
        return null;
    }

    public string Serialize()
    {
        string text = "";
        if (this.WayPoints != null)
        {
            foreach (Vector3 vector in this.WayPoints)
            {
                string[] array = new string[7];
                array[0] = text;
                array[1] = ((text.Length > 0) ? "#" : "");
                int num = 2;
                float num2 = vector.x;
                array[num] = num2.ToString(CultureInfo.InvariantCulture);
                array[3] = "|";
                int num3 = 4;
                num2 = vector.y;
                array[num3] = num2.ToString(CultureInfo.InvariantCulture);
                array[5] = "|";
                int num4 = 6;
                num2 = vector.z;
                array[num4] = num2.ToString(CultureInfo.InvariantCulture);
                text = string.Concat(array);
            }
        }
        bool flag = this.BasePartPosStt != null && this.BasePartPosStt != null;
        bool flag2 = this.BasePartPosEnd != null && this.BasePartPosEnd != null;
        IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
        string text2 = "{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}";
        object[] array2 = new object[11];
        array2[0] = this.Id.ToString(CultureInfo.InvariantCulture);
        array2[1] = this.Name;
        array2[2] = this.Length.ToString(CultureInfo.InvariantCulture);
        array2[3] = this.Speed.ToString(CultureInfo.InvariantCulture);
        int num5 = 4;
        object obj;
        if (!flag)
            obj = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosStt.Value;
            obj = vector2.x.ToString(CultureInfo.InvariantCulture);
        }
        array2[num5] = obj;
        int num6 = 5;
        object obj2;
        if (!flag)
            obj2 = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosStt.Value;
            obj2 = vector2.y.ToString(CultureInfo.InvariantCulture);
        }
        array2[num6] = obj2;
        int num7 = 6;
        object obj3;
        if (!flag)
            obj3 = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosStt.Value;
            obj3 = vector2.z.ToString(CultureInfo.InvariantCulture);
        }
        array2[num7] = obj3;
        int num8 = 7;
        object obj4;
        if (!flag2)
            obj4 = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosEnd.Value;
            obj4 = vector2.x.ToString(CultureInfo.InvariantCulture);
        }
        array2[num8] = obj4;
        int num9 = 8;
        object obj5;
        if (!flag2)
            obj5 = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosEnd.Value;
            obj5 = vector2.y.ToString(CultureInfo.InvariantCulture);
        }
        array2[num9] = obj5;
        int num10 = 9;
        object obj6;
        if (!flag2)
            obj6 = "?";
        else
        {
            Vector3 vector2 = this.BasePartPosEnd.Value;
            obj6 = vector2.z.ToString(CultureInfo.InvariantCulture);
        }
        array2[num10] = obj6;
        array2[10] = text;
        return string.Format(invariantCulture, text2, array2);
    }
}
