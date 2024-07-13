

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;






   
    public class Userstg
{
    public long Id { get; set; }
    public string? username { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }

    public int phone_number { get; set; }
}

public class Chatstg
{
    [Key] 
    [Column("ChatId")]
    public long ChatId { get; set; }
    public string? title { get; set; }
}

public class Userchatrelationship
{
    [Key]
    public int relationshipid { get; set; }
    public long userid { get; set; }
    public long chatidRS { get; set; }
}

public class VideoRecord
{
    public int Id { get; set; }
    public string VideoId { get; set; }
    public int Duration { get; set; }
    public string MimeType { get; set; }
    public long FileSize { get; set; }

    
    public string Title { get; set; }
    public string Description { get; set; }
}


