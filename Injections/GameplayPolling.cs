using ConnectorLib.Inject.AddressChaining;
using CrowdControl.Games.Packs.MCCCursedHaloCE.Utilites.ByteArrayBuilding;
using System;
using System.Linq;
using CcLog = CrowdControl.Common.Log;

namespace CrowdControl.Games.Packs.MCCCursedHaloCE;

public partial class MCCCursedHaloCE
{

    
    // ScriptVarPauseDetection is defined on the script injection, since i'm using a scrip to constantly change a var, since they only run during gameplay.

    private long previousGamplayPollingValue = 305441741;
    private bool currentlyInGameplay = false;

    /// <summary>
    /// Returns true if the game is not closed, paused, or in a menu. Returns true during cutscenes.
    /// </summary>
    private bool IsInGameplayCheck()
    {
        if (scriptVarPauseDetection_ch == null)
        {
            CcLog.Message("Gameplay polling pointer is null");
            return false;
        }

        if(!TryGetIndirectByteArray(scriptVarPauseDetection_ch, offset: 0, byteAmount: 4, out byte[] counterRawBytes))
        {
            CcLog.Message("Could not retrieve the gameplay polling variable.");
        }

        int value = BitConverter.ToInt32(counterRawBytes, 0);

        if (value == previousGamplayPollingValue)
        {
            CcLog.Debug("Gameplay polling pointer is unchanged, currently " + value);
            return false;
        }

        // If the current value is 0, this is most likely the scrip has not even loaded yet.
        if (value == 0)
        {
            CcLog.Debug("Gameplay polling value is 0");
            return false;
        }

        previousGamplayPollingValue = value;
        CcLog.Debug("Gameplay polling pointer changed to " + value);

        return true;
    }    
}