using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

public class Post
{
    private string userId;
    private string text;
    private string threadId;
    private DateTime date;

    private Post(JToken token)
    {
        if (token["user"] != null)
        {
            userId = token["user"].ToString();
        }
        else
        {
            userId = "BOT";
        }

        var sb = new StringBuilder();
        sb.Append(token["text"].ToString());
        if (token["files"] != null)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append("<何かのメディア>");
        }
        if (token["attachments"] != null)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append("<何かのメディア>");
        }
        text = sb.ToString();

        if (token["thread_ts"] != null)
        {
            threadId = token["thread_ts"].ToString();
        }
        else
        {
            threadId = token["ts"].ToString();
        }

        date = DateTime.UnixEpoch + TimeSpan.FromSeconds(long.Parse(token["ts"].ToString().Split('.')[0])) + TimeSpan.FromHours(9);
    }

    private static IEnumerable<Post> GetPostsFromSingleJson(string path)
    {
        foreach (var token in JArray.Parse(File.ReadAllText(path, Encoding.UTF8)))
        {
            yield return new Post(token);
        }
    }

    public static IEnumerable<Post> GetPosts(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            foreach (var post in GetPostsFromSingleJson(file))
            {
                yield return post;
            }
        }
    }

    public string UserId => userId;
    public string Text => text;
    public string ThreadId => threadId;
    public DateTime Date => date;
}
