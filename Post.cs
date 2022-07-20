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

    private Reaction[] reactions;

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
            sb.Append("<span class=\"info\">&lt;何かのメディア&gt;</span>");
        }
        if (token["attachments"] != null)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append("<span class=\"info\">&lt;何かのメディア&gt;</span>");
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

        if (token["reactions"] != null)
        {
            var list = new List<Reaction>();

            foreach (var reaction in token["reactions"])
            {
                var type = reaction["name"].ToString();
                foreach (var user in reaction["users"])
                {
                    list.Add(new Reaction(user.ToString(), type));
                }
            }

            reactions = list.ToArray();
        }
        else
        {
            reactions = Array.Empty<Reaction>();
        }
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
    public IReadOnlyList<Reaction> Reactions => reactions;



    public class Reaction
    {
        private string userId;
        private string type;

        internal Reaction(string userId, string type)
        {
            this.userId = userId;
            this.type = type;
        }

        public string UserId => userId;
        public string Type => type;
    }
}
