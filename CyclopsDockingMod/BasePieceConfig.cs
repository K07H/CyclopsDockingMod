namespace CyclopsDockingMod;
using UnityEngine;

public class BasePieceConfig
{
    public bool XShape;

    public bool TShape;

    public bool IShape;

    public Quaternion Rotation;

    private BasePieceConfig()
    {
    }

    public BasePieceConfig(bool xShape, bool tShape, bool iShape, Quaternion rotation)
    {
        this.XShape = xShape;
        this.TShape = tShape;
        this.IShape = iShape;
        this.Rotation = rotation;
    }

    public bool IsValid()
    {
        return this.IShape || this.TShape || this.XShape;
    }

    public void SetSquare(Quaternion o)
    {
        this.Rotation = o * o;
    }
}
