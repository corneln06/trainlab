using System.ComponentModel.DataAnnotations;

namespace TrainLab.Modules.Import;

public class UploadOptions
{
    [Range(1, 104857600)]
    public int MaxFileSizeBytes { get; set; }
    [Range(1, 100)]
    public int MaxFilesPerRequest { get; set; }

    public const string SectionName = "Upload";
}