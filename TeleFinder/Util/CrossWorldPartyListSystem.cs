using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.ClientState.Party;
using Dalamud.Logging;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace TeleFinder.Util;

public static class CrossWorldPartyListSystem
{
    // Yes, there's already a type in Dalamud for this.
    // TODO? add more if we end up needing it
    public struct CrossWorldMember
    {
        public string Name;
        public int PartyCount;
        public uint Level;
        public uint JobId;
    }
    
    public delegate void CrossWorldJoinDelegate(CrossWorldMember m);
    public delegate void CrossWorldLeaveDelegate(CrossWorldMember m);

    public static event CrossWorldJoinDelegate? OnJoin;
    public static event CrossWorldLeaveDelegate? OnLeave;

    public static void Start()
    {
        PluginLog.Information("Start");
        Service.Framework.Update += Update;
    }

    public static void Stop()
    {
        PluginLog.Information("Stop");
        Service.Framework.Update -= Update;
    }

    private static List<CrossWorldMember> Members = new();
    private static List<CrossWorldMember> OldMembers = new();

    static bool ListContainsMember(List<CrossWorldMember> l, CrossWorldMember m)
    {
        // oh this is incredibly fucking stupid
        foreach (var a in l)
        {
            if (a.Name == m.Name)
                return true;
        }

        return false;
    }

    static unsafe void Update(IFramework framework)
    {
        if (!Service.ClientState.IsLoggedIn)
            return;

        if (!InfoProxyCrossRealm.IsCrossRealmParty())
            return;
        
        Members.Clear();
        var partyCount = InfoProxyCrossRealm.GetPartyMemberCount();
        for (var i = 0u; i < partyCount; i++)
        {
            var addr = InfoProxyCrossRealm.GetGroupMember(i);
            var name = MemoryHelper.ReadStringNullTerminated((nint)addr->Name);
            var mObj = new CrossWorldMember
            {
                Name = name,
                PartyCount = partyCount,
                Level = addr->Level,
                JobId = addr->ClassJobId,
            };
            Members.Add(mObj);
        }
        
        if (Members.Count != OldMembers.Count)
        {
            // a change has been detected
            
            // Check for joins
            foreach (var i in Members)
            {
                if (!ListContainsMember(OldMembers, i))
                {
                    // member joined
                    OnJoin?.Invoke(i);
                }
            }
            
            // Check for leaves
            // Is this what we call 'iterating too much?'
            foreach (var i in OldMembers)
            {
                if (!ListContainsMember(Members, i))
                {
                    // member left
                    OnLeave?.Invoke(i);
                }
            }
        }
        
        // REFERENCE FUNNIES?
        OldMembers = Members.ToList();
    }
}
