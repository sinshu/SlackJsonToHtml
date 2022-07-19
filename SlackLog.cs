using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class SlackLog
{
    private User[] users;
    private Post[] posts;

    private Dictionary<string, User> userIdToUser;

    public SlackLog(string userJsonPath, string directory)
    {
        users = User.GetUsers(userJsonPath).ToArray();
        posts = Post.GetPosts(directory).ToArray();

        userIdToUser = new Dictionary<string, User>();
        foreach (var user in users)
        {
            userIdToUser.Add(user.Id, user);
        }
        userIdToUser.Add(User.Bot.Id, User.Bot);
    }

    public IEnumerable<IEnumerable<Post>> EnumerateThreads()
    {
        var threads = posts.GroupBy(post => post.ThreadId);
        foreach (var thread in threads)
        {
            yield return thread.OrderBy(post => post.Date);
        }
    }
}
