using System;
using System.IO;

public static class Program
{
    public static void Main(string[] args)
    {
        var log = new SlackLog(
            @"users.json",
            @"general");

        using (var writer = new StreamWriter("general.html"))
        {
            var title = "Slack アーカイブ &#35;" + log.ChannelName + " (2022/07/19)";

            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"UTF-8\">");
            writer.WriteLine("<title>" + title + "</title>");
            writer.WriteLine("<style>");
            writer.WriteLine("body { width: 50em; margin: auto; word-break: break-all; background-color: #212121; color: #E0E0E0; }");
            writer.WriteLine("hr { height: 2px; border: none; background-color: #757575; margin-top: 1em; margin-bottom: 1em; }");
            writer.WriteLine("dd { margin-bottom: 1em; }");
            writer.WriteLine("a { color: #0288D1; }");
            writer.WriteLine(".username { font-weight: bold; color: #03A9F4; }");
            writer.WriteLine(".date { color: #757575; }");
            writer.WriteLine(".reaction { color: #757575; }");
            writer.WriteLine(".info { color: #757575; }");
            writer.WriteLine("</style>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<h1>" + title + "</h1>");

            foreach (var thread in log.EnumerateThreads())
            {
                writer.WriteLine("<hr>");
                writer.WriteLine("<dl>");

                foreach (var post in thread)
                {
                    writer.WriteLine("    <dt><span class=\"username\">" + log.FromUserId(post.UserId).Name + "</span> <span class=\"date\">" + post.Date + "</span></dt>");
                    writer.WriteLine("    <dd>" + log.ToHtml(post.Text) + "</dd>");

                    if (post.Reactions.Count > 0)
                    {
                        writer.Write("    <dd class=\"reaction\">");
                        foreach (var reaction in post.Reactions)
                        {
                            var emoji = log.EmojiNameToUnicode(reaction.Type);
                            writer.Write(emoji + " " + log.FromUserId(reaction.UserId).Name + "<br>");
                        }
                        writer.WriteLine("    </dd>");
                    }
                }

                writer.WriteLine("</dl>");
            }

            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}
