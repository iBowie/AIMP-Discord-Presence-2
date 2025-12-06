using AIMP.SDK;
using AIMP.SDK.MessageDispatcher;
using System;

namespace AIMP_Discord_Presence_2.Hooks
{
	public sealed class TrackChangedHook : IAimpMessageHook
	{
		private readonly RPCPlugin _plugin;

		public TrackChangedHook(RPCPlugin plugin)
		{
			_plugin = plugin;
		}

		public AimpActionResult CoreMessage(AimpCoreMessageType message, int param1, IntPtr param2)
		{
			if (message == AimpCoreMessageType.EventPlayerUpdatePosition)
			{
				_plugin.UpdateTrackInfo();
			}

			if (message == AimpCoreMessageType.EventPlayingFileInfo)
			{
				_plugin.UpdateTrackInfo();
			}

			if (message == AimpCoreMessageType.EventStreamStart)
			{
				_plugin.UpdateTrackInfo();
			}

			if (message == AimpCoreMessageType.EventPlayerState)
			{
				_plugin.UpdateTrackInfo();
			}

			return new AimpActionResult(ActionResultType.OK);
		}
	}
}
