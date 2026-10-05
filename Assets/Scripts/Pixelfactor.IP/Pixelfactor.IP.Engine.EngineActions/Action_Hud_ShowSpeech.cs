using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.SaveGame;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Hud_ShowSpeech : EngineAction
	{
		public bool CanSkip = true;

		public bool ForceSpeechModelUpdate = true;

		public bool IsImportant = true;

		public string Message;

		public bool MessageExpires = true;

		public Person Source;

		public override ActionType Type => ActionType.Hud_ShowSpeech;

		public override void Execute()
		{
			base.Execute();
			SpeechModel speechModel = engine.Hud.SpeechModel;
			speechModel.ClearRequests();
			SpeechModel.SpeechRequest speechRequest = speechModel.AddRequest(Source, Message);
			if (speechRequest != null)
			{
				speechRequest.IsImportant = IsImportant;
				speechRequest.Expires = MessageExpires;
				speechRequest.AllowSkip = CanSkip;
			}
			if (ForceSpeechModelUpdate)
			{
				speechModel.Update();
			}
		}

		public override void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
			base.WriteBinary(writer, engine);
			writer.WritePilotId(Source);
		}

		public override void ReadBinary(BinaryReader reader, EngineASX engine)
		{
			base.ReadBinary(reader, engine);
			Source = reader.ReadPilotFromId(engine);
		}
	}
}
