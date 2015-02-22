//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
   public enum NPCDialogAction : byte
    {
        Text = 1,
        Link,
        Edit,
        Pic,
        ListLine,
        Popup,
        Create = 100,
        Answer,
        TaskId,
        PatchNotes = 112
    }
}
