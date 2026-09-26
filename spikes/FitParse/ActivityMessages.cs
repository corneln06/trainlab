using Dynastream.Fit;

namespace FitParse;

public class ActivityMessages
{
    public FileIdMesg? FileId { get; set; }
    public List<SessionMesg> Sessions { get; set; } = new();
    public List<RecordMesg?> Records { get; }= new();
    
    
}