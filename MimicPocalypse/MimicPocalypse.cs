using System.Collections.Generic;
using ItemChanger;
using Modding;

namespace MimicPocalypse
{
    public class MimicPocalypse : Mod
    {
        internal static MimicPocalypse instance;
        
        public MimicPocalypse() : base("Mimicpocalypse")
        {
            instance = this;
        }
        
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            ItemChanger.Events.BeforeStartNewGame += Events_BeforeStartNewGame;
        }

        private void Events_BeforeStartNewGame()
        {
            Dictionary<string, AbstractPlacement> placements = ItemChanger.Internal.Ref.Settings.Placements;
            List<AbstractPlacement> newPlacements = new List<AbstractPlacement>();

            foreach (var kvp in placements)
            {
                TryAddMimicTo(kvp.Value, newPlacements);
            }

            ItemChangerMod.AddPlacements(newPlacements, conflictResolution: PlacementConflictResolution.Replace);
        }

        private void TryAddMimicTo(AbstractPlacement placement, List<AbstractPlacement> newPlacements)
        {
            if (placement.Items.Count != 1)
            {
                LogDebug($"{placement.Name} : not 1 item"); return;
            }
            AbstractItem item = placement.Items[0];

            if (item.GetPreferredContainer() != Container.Unknown)
            {
                LogDebug($"{placement.Name} : Biased item {item.name}"); return;
            }

            if (placement is ItemChanger.Placements.IPrimaryLocationPlacement plpmt)
            {
                AbstractLocation loc = plpmt.Location;

                if (loc is ItemChanger.Locations.ExistingContainerLocation eloc && eloc.nonreplaceable)
                {
                    LogDebug($"{placement.Name} : Nonreplaceable location"); return;
                }
                else if (loc is ItemChanger.Locations.ContainerLocation cloc && cloc.forceShiny)
                {
                    LogDebug($"{placement.Name} : ForceShiny location"); return;
                }
                else if (!(loc is ItemChanger.Locations.ContainerLocation) && !(loc is ItemChanger.Locations.ExistingContainerLocation))
                {
                    LogDebug($"{placement.Name} : NonContainer Location"); return;
                }
                else if (placement is ItemChanger.Placements.ISingleCostPlacement scpmt && scpmt.Cost != null)
                {
                    LogDebug($"{placement.Name} : Costed Location"); return;
                }

                LogDebug($"{placement.Name} : Adding Mimics...");


                newPlacements.Add(placement.Add(Finder.GetItem(ItemNames.Mimic_Grub)));
            }
        }
    }
}