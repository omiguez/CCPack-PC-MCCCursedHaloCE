using CrowdControl.Common;
using CrowdControl.Games.Packs.MCCCursedHaloCE.Effects;

namespace CrowdControl.Games.Packs.MCCCursedHaloCE
{
    public partial class MCCCursedHaloCE
    {
        // While on air, multiplies the player current horizontal speed by a factor, making it get out of control quickly.
        public void ActivateUnstableAirtime(EffectRequest request)
        {
            StartTimed(request, () => IsReady(request),
                () =>
                {
                    Connector.SendMessage($"{request.DisplayViewer} aggressively suggest you stay grounded.");
                    return InjectUnstableAirtime();
                },
                EffectMutex.PlayerSpeed)
            .WhenCompleted.Then(_ =>
            {
                Connector.SendMessage($"You can jump safely again.");
                UndoInjection(UnstableAirtimeId);
            });
        }
    }
}