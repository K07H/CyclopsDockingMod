namespace CyclopsDockingMod;
using global::CyclopsDockingMod.UI;
using UnityEngine;

public class BasePart
{
    public static readonly Vector3 P_CyclopsDockingHatch = new Vector3(0f, 3.1f, 0f);

    public static readonly Vector3 P_CyclopsDockingHatchC = new Vector3(0f, P_CyclopsDockingHatch.y + 0.4f, 0f);

    public static readonly Vector3 P_CyclopsDockingHatchCF = new Vector3(0f, P_CyclopsDockingHatch.y - 0.3f, 0f);

    public static readonly Vector3 P_CyclopsDockingHatchF = new Vector3(0f, P_CyclopsDockingHatch.y + 2f, 0f);

    public static readonly Color[] Colors = new Color[]
    {
        Color.gray,
        new Color32(211, byte.MaxValue, 253, byte.MaxValue),
        Color.black,
        Color.white,
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        new Color32(153, 50, 204, byte.MaxValue),
        new Color32(byte.MaxValue, 154, 229, byte.MaxValue)
    };

    private Transform _hatch;

    public string id;

    public Int3 cell;

    public int index;

    public Vector3 position;

    public int type;

    public BaseRoot root;

    public string dock;

    public SubRoot sub;

    public bool trapOpened;

    public int signScale;

    public int signColor;

    public bool signBackground;

    public bool[] signElements;

    public GameObject signGo;

    public const int T_CyclopsDockingHatch = 0;

    public BasePart()
    {
        this._hatch = null;
        this.id = null;
        this.cell = Int3.zero;
        this.index = -1;
        this.position = Vector3.zero;
        this.type = -1;
        this.root = null;
        this.dock = null;
        this.sub = null;
        this.signScale = CyclopsDockingModUI.CfgSignTextScale;
        this.signColor = CyclopsDockingModUI.CfgSignTextColorVal;
        this.signBackground = CyclopsDockingModUI.CfgSignBackgroundVisible;
        this.signElements = null;
        this.signGo = null;
        this.trapOpened = false;
    }

    public BasePart(string id, Int3 cell, int index, Vector3 position, int type, string dock, BaseRoot root, SubRoot sub, int signScale, int signColor, bool signBackground, bool[] signElements)
    {
        this._hatch = null;
        this.id = id;
        this.cell = cell;
        this.index = index;
        this.position = position;
        this.type = type;
        this.root = root;
        this.dock = dock;
        this.sub = sub;
        this.signScale = signScale;
        this.signColor = signColor;
        this.signBackground = signBackground;
        this.signElements = signElements;
        this.signGo = null;
        this.trapOpened = false;
    }

    public void Reset()
    {
        this._hatch = null;
    }

    public Transform GetDockingHatch()
    {
        if (this._hatch != null)
            return this._hatch;
        else if (this.root != null)
        {
            Base b = this.root.GetComponent<Base>();
            if (b != null)
            {
                Transform t;
                foreach (Int3 cell in b.AllCells)
                    if ((t = b.GetCellObject(cell)) != null && FastHelper.IsNear(t.position, this.position))
                    {
                        foreach (Transform ch in t)
                            if (ch.name.StartsWith(CyclopsHatchConnector.ModelName))
                            {
                                this._hatch = ch;
                                return this._hatch;
                            }
                        break;
                    }
            }
        }
        return null;
    }

    public void PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim toPlay)
    {
        if (toPlay != CyclopsHatchConnector.CyclopsDockingAnim.NONE)
        {
            Transform dockingHatch = this.GetDockingHatch();
            if (dockingHatch != null)
                CyclopsHatchConnector.PlayDockingAnim(dockingHatch.gameObject, toPlay);
        }
    }

    public bool IsDockingHatch(Vector3 pos)
    {
        return FastHelper.IsNear(pos, this.position - P_CyclopsDockingHatchC);
    }
}
