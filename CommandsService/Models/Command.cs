using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models;

public class Command
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string Howto { get; set; }
    [Required] public string CommandLine { get; set; }
    [Required] public int PlatformId { get; set; }
    public Platform Platform { get; set; }
}