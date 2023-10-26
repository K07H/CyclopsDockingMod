namespace CyclopsDockingMod.Controllers;
using global::CyclopsDockingMod.Fixers;
using UnityEngine;

public class LadderController : HandTarget, IHandTarget
{
    public void OnHandHover(GUIHand hand)
    {
        if (!enabled || hand.player == null)
            return;
        BasePart basePart = this.GetBasePart();
        bool flag = basePart != null && basePart.root != null && (basePart.dock != null || basePart.sub != null);
        HandReticle.main.SetText(HandReticle.TextType.Hand, flag ? ConfigOptions.LblClimbInCyclops : ConfigOptions.LblNoCyclopsDocked, false, GameInput.Button.LeftHand);
        HandReticle.main.SetIcon(flag ? HandReticle.IconType.Hand : HandReticle.IconType.HandDeny, 1f);
    }

    public void OnHandClick(GUIHand hand)
    {
        if (!enabled || hand.player == null)
            return;
        BasePart basePart = this.GetBasePart();
        if (basePart != null && basePart.root != null && (basePart.dock != null || basePart.sub != null))
        {
            if (basePart.sub == null)
                basePart.sub = BaseFixer.GetSubRoot(basePart.dock);
            if (basePart.sub != null)
            {
                hand.player.SetPosition(basePart.position + BasePart.P_CyclopsDockingHatchF);
                hand.player.currentEscapePod = null;
                hand.player.escapePod.Update(false);
                hand.player.SetCurrentSub(basePart.sub, false);
                hand.player.currentWaterPark = null;
                hand.player.precursorOutOfWater = false;
                hand.player.SetDisplaySurfaceWater(true);
            }
        }
    }

    private BasePart GetBasePart()
    {
        Transform transform = base.transform;
        if (((transform != null) ? transform.parent : null) != null)
            foreach (BasePart basePart in BaseFixer.BaseParts)
                if (base.transform.parent.position.x > basePart.position.x - 0.1f && base.transform.parent.position.x < basePart.position.x + 0.1f && base.transform.parent.position.z > basePart.position.z - 0.1f && base.transform.parent.position.z < basePart.position.z + 0.1f && base.transform.parent.position.y > basePart.position.y - (BasePart.P_CyclopsDockingHatchC.y + 0.1f) && base.transform.parent.position.y < basePart.position.y - (BasePart.P_CyclopsDockingHatchC.y - 0.1f))
                    return basePart;
        return null;
    }
}
