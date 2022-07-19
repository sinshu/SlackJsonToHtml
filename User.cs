using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

public class User
{
    public static readonly User Bot = new User("BOT", "BOT");

    private string id;
    private string name;

    private User(string id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public User(JToken token)
    {
        id = token["id"].ToString();
        name = token["real_name"].ToString();
    }

    public static IEnumerable<User> GetUsers(string path)
    {
        foreach (var token in JArray.Parse(File.ReadAllText(path, Encoding.UTF8)))
        {
            yield return new User(token);
        }
    }

    public string Id => id;
    public string Name => name;
}
