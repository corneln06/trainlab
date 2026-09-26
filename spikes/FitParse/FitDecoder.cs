using Dynastream.Fit;

namespace FitParse;

public class FitDecoder
{
    public ActivityMessages Messages { get; } = new();

    public bool Decode(Stream stream)
    {
        var decoder = new Decode();

        if (!decoder.IsFIT(stream)) return false;
        if (!decoder.CheckIntegrity(stream)) Console.WriteLine("Warning: Integrity check failed.");
        stream.Position = 0;

        var broadcaster = new MesgBroadcaster();
        decoder.MesgEvent += broadcaster.OnMesg;
        decoder.MesgDefinitionEvent += broadcaster.OnMesgDefinition;

        broadcaster.FileIdMesgEvent += OnFileIdMesg ;
        broadcaster.SessionMesgEvent += OnSessionMesg ;
        broadcaster.RecordMesgEvent += OnRecordMessage;

        decoder.Read(stream);
        return true;
    }

    private void OnFileIdMesg(object sender, MesgEventArgs e)
    {
        Messages.FileId = (FileIdMesg)e.mesg;
    }

    private void OnSessionMesg(object sender, MesgEventArgs e)
    {
        Messages.Sessions.Add((SessionMesg)e.mesg);
    }

    private void OnRecordMessage(object sender, MesgEventArgs e)
    {
        Messages.Records.Add(e.mesg as RecordMesg);
    }
  
}