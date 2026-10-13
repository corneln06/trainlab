using System.ComponentModel.DataAnnotations;

namespace TrainLab.Infrastructure;

public class StorageOptions
{
    public const string SectionName = "Storage";

    [Required] public string RootPath { get; set; } = "";

}