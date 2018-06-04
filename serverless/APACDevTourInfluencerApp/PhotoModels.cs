using System;
using System.Collections.Generic;

public class Comment
{
    public string userId { get; set; }
    public string text { get; set; }
    public DateTime date { get; set; }
    public string displayName { get; set; }
}

public class Photo
{
    public string photoUrl { get; set; }
    public object thumbnailUrl { get; set; }
    public string userId { get; set; }
    public List<Comment> comments { get; set; }
    public int upVotes { get; set; }
    public int downVotes { get; set; }
    public DateTime uploadDate { get; set; }
    public List<object> tags { get; set; }
    public string displayName { get; set; }
}