using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SlackLog
{
    private Regex reCodeBlock = new Regex("```(.+?)```", RegexOptions.Singleline);
    private Regex reSpecialText = new Regex("<(.+?)>");
    private Regex reEmoji = new Regex(":([a-z_]+?):");

    private string channelName;
    private User[] users;
    private Post[] posts;

    private Dictionary<string, User> userIdToUser;

    public SlackLog(string userJsonPath, string directory)
    {
        channelName = Path.GetFileName(directory);
        users = User.GetUsers(userJsonPath).ToArray();
        posts = Post.GetPosts(directory).ToArray();

        userIdToUser = new Dictionary<string, User>();
        foreach (var user in users)
        {
            userIdToUser.Add(user.Id, user);
        }
    }

    public IEnumerable<IEnumerable<Post>> EnumerateThreads()
    {
        var threads = posts.GroupBy(post => post.ThreadId);
        foreach (var thread in threads.OrderByDescending(thread => thread.First().Date))
        {
            yield return thread.OrderBy(post => post.Date);
        }
    }

    public User FromUserId(string userId)
    {
        if (userIdToUser.ContainsKey(userId))
        {
            return userIdToUser[userId];
        }
        else
        {
            return User.Bot;
        }
    }

    public string ToHtml(string text)
    {
        text = reEmoji.Replace(text, new MatchEvaluator(EmojiMatchAction));
        text = reSpecialText.Replace(text, new MatchEvaluator(SpecialTextMatchAction));
        text = reCodeBlock.Replace(text, new MatchEvaluator(CodeBlockMatchAction));
        text = text.Replace("\r\n", "<br>");
        text = text.Replace("\n", "<br>");
        text = text.Replace(":skin-tone-3:", "");
        //text = text.Replace("<", "&lt;");
        //text = text.Replace(">", "&gt;");
        //text = text.Replace("&", "&amp;");
        //text = text.Replace("\"", "&quot;");
        return text;
    }

    private string SpecialTextMatchAction(Match match)
    {
        var target = match.Groups[1].Value;

        var urlSplit = target.Split('|');
        if (urlSplit.Length == 2)
        {
            var url = urlSplit[1];
            var title = urlSplit[0];
            return "<a href=\"" + url + "\">" + title + "</a>";
        }

        if (target.StartsWith("http"))
        {
            return "<a href=\"" + target + "\">" + target + "</a>";
        }

        if (target.StartsWith('@'))
        {
            if (userIdToUser.ContainsKey(target.Substring(1)))
            {
                return "&lt;" + userIdToUser[target.Substring(1)].Name + "&gt;";
            }
        }

        if (target.StartsWith("span") || target.StartsWith("/span"))
        {
            return match.Value;
        }

        return target;
    }

    private string EmojiMatchAction(Match match)
    {
        var target = match.Groups[1].Value;
        return EmojiNameToUnicode(target);
    }

    public string EmojiNameToUnicode(string name)
    {
        switch (name)
        {
            case "grin":
                return "&#x1f601;";
            case "fearful":
                return "&#x1f628;";
            case "relaxed":
                return "&#x263a;";
            case "rolling_on_the_floor_laughing":
                return "&#x1f923;";
            case "wink":
                return "&#x1f609;";
            case "joy":
                return "&#x1f602;";
            case "smile":
                return "&#x1f604;";
            case "yum":
                return "&#x1f60b;";
            case "cry":
                return "&#x1f622;";
            case "thinking_face":
                return "&#x1f914;";
            case "hot_face":
                return "&#x1f975;";
            case "sunglasses":
                return "&#x1f60e;";
            case "laughing":
                return "&#x1f606;";
            case "asyncparrot":
                return "&#x1f99c;";
            case "heart_eyes":
                return "&#x1f60d;";
            case "blush":
                return "&#x1f60a;";
            case "pray":
                return "&#x1f64f;";
            case "sake":
                return "&#x1f376;";
            case "star":
                return "&#x2b50;";
            case "cold_face":
                return "&#x1f976;";
            case "exploding_head":
                return "&#x1f92f;";
            case "sleepy":
                return "&#x1f62a;";
            case "partying_face":
                return "&#x1f973;";
            case "eyes":
                return "&#x1f440;";
            case "wavy_dash":
                return "&#x3030;";
            case "arrow_down":
                return "&#x2b07;";
            case "shit":
                return "&#x1f4a9;";
            case "black_large_square":
                return "&#x2b1b;";
            case "large_yellow_square":
                return "&#x1f7e8;";
            case "large_green_square":
                return "&#x1f7e9;";
            case "sweat_smile":
                return "&#x1f605;";
            case "snowman":
                return "&#x2603;";
            case "sushi":
                return "&#x1f363;";
            case "dumpling":
                return "&#x1f95f;";
            case "bamboo":
                return "&#x1f38d;";
            case "open_mouth":
                return "&#x1f62e;";
            case "broom":
                return "&#x1f9f9;";
            case "sob":
                return "&#x1f62d;";
            case "tada":
                return "&#x1f389;";
            case "soccer":
                return "&#x26bd;";
            case "arrow_up":
                return "&#x2b06;";
            case "smiley":
                return "&#x1f60a;";
            case "innocent":
                return "&#x1f607;";
            case "exclamation":
                return "&#x2757;";
            case "zany_face":
                return "&#x1f92a;";
            case "triumph":
                return "&#x1f624;";
            case "scream":
                return "&#x1f631;";
            case "sparkles":
                return "&#x2728;";
            case "point_down":
                return "&#x1f447;";
            case "tooth":
                return "&#x1f9b7;";
            case "wave":
                return "&#x1f44b;";
            case "owl":
                return "&#x1f989;";
            case "hamster":
                return "&#x1f439;";
            case "egg":
                return "&#x1f95a;";
            case "sneezing_face":
                return "&#x1f927;";
            case "hushed":
                return "&#x1f62f;";
            case "crab":
                return "&#x1f980;";
            case "fork_and_knife":
                return "&#x1f374;";
            case "beer":
                return "&#x1f37a;";
            case "raised_hands":
                return "&#x1f64c;";
            case "guitar":
                return "&#x1f3b8;";
            case "musical_keyboard":
                return "&#x1f3b9;";
            case "tv":
                return "&#x1f4fa;";
            case "camera":
                return "&#x1f4f7;";
            case "watermelon":
                return "&#x1f349;";
            case "star-struck":
                return "&#x1f929;";
            case "kissing_heart":
                return "&#x1f618;";
            case "+1::skin-tone-3":
                return "&#x1f44d;";
            case "kissing_closed_eyes":
                return "&#x1f619;";
            case "drooling_face":
                return "&#x1f924;";
            case "+1":
                return "&#x1f44d;";
            case "gun":
                return "&#x1f52b;";
            case "coffee":
                return "&#x2615;";
            case "congratulations":
                return "&#x3297;";
            case "窓":
                return "&#x1fa9f;";
            case "ocean":
                return "&#x1f30a;";
            case "deer":
                return "&#x1f98c;";
            case "shell":
                return "&#x1f41a;";
            case "grinning":
                return "&#x1f929;";
            case "slot_machine":
                return "&#x1f3b0;";
            case "kotlin":
                return "&#x1f426;";
            case "poultry_leg":
                return "&#x1f357;";
            case "ok_hand":
                return "&#x1f44c;";
            case "skull":
                return "&#x1f480;";
            case "wrench":
                return "&#x1f527;";
            case "clap::skin-tone-3":
                return "&#x1f44f;";
            case "computer":
                return "&#x1f4bb;";
            case "cookie":
                return "&#x1f36a;";
            case "beers":
                return "&#x1f37b;";
            case "u6708":
                return "&#x1f237;";
            case "keyboard":
                return "&#x2328;";
            case "woman-golfing":
                return "&#x1f3cc;";
            case "astonished":
                return "&#x1f632;";
            case "confetti_ball":
                return "&#x1f38a;";
            case "cherry_blossom":
                return "&#x1f338;";
            case "boom":
                return "&#x1f4a5;";
            case "large_orange_circle":
                return "&#x1f7e0;";
            case "rocket":
                return "&#x1f680;";
            case "dog2":
                return "&#x1f415;";
            case "spider_web":
                return "&#x1f578;";
            case "handshake":
                return "&#x1f91d;";
            case "sun_with_face":
                return "&#x1f31e;";
            case "two":
                return "&#x0032;";
            case "three":
                return "&#x0033;";
            case "clap":
                return "&#x1f44f;";
            case "cake":
                return "&#x1f370;";
            case "snow_capped_mountain":
                return "&#x1f3d4;";
            case "ok":
                return "&#x1f44c;";
            case "dark_sunglasses":
                return "&#x1f576;";
            case "star2":
                return "&#x1f31f;";
            case "atom_symbol":
                return "&#x269b;";
            case "ram":
                return "&#x1f40f;";
            case "fish":
                return "&#x1f41f;";
            case "grimacing":
                return "&#x1f62c;";
            case "money_mouth_face":
                return "&#x1f911;";
            case "skull_and_crossbones":
                return "&#x2620;";
            case "crossed_swords":
                return "&#x2694;";
            case "film_projector":
                return "&#x1f4fd;";
            case "hamburger":
                return "&#x1f354;";
            case "fried_egg":
                return "&#x1f373;";
            case "bat":
                return "&#x1f987;";
            case "upside_down_face":
                return "&#x1f643;";
            case "confused":
                return "&#x1f615;";
            case "no_mouth":
                return "&#x1f636;";
            case "pig2":
                return "&#x1f416;";
            case "large_yellow_circle":
                return "&#x1f7e1;";
            case "squid":
                return "&#x1f991;";
            case "octopus":
                return "&#x1f419;";
            case "face_with_symbols_on_mouth":
                return "&#x1f92c;";
            case "funeral_urn":
                return "&#x26b1;";
            case "hammer":
                return "&#x1f528;";
            case "flushed":
                return "&#x1f633;";
            case "polar_bear":
                return "&#x1f43b;";
            case "girl":
                return "&#x1f467;";
            case "snake":
                return "&#x1f40d;";
            case "desktop_computer":
                return "&#x1f5a5;";
            case "glitch_crab":
                return "&#x1f980;";
            case "headstone":
                return "&#x1faa6;";
            case "arrows_counterclockwise":
                return "&#x1f504;";
            case "spaghetti":
                return "&#x1f35d;";
            case "full_moon_with_face":
                return "&#x1f31d;";
            case "face_with_raised_eyebrow":
                return "&#x1f928;";
            case "bike":
                return "&#x1f6b2;";
            case "face_with_rolling_eyes":
                return "&#x1f644;";
            case "wolf":
                return "&#x1f43a;";
            case "neutral_face":
                return "&#x1f610;";
            case "older_woman":
                return "&#x1f475;";
            case "rainbow":
                return "&#x1f308;";
            case "point_up_2":
                return "&#x1f446;";
            case "penguin":
                return "&#x1f427;";
            case "horse":
                return "&#x1f40e;";
            case "shower":
                return "&#x1f6bf;";
            case "sunrise":
                return "&#x1f305;";
            case "canned_food":
                return "&#x1f96b;";
            case "glass_of_milk":
                return "&#x1f95b;";
            case "cow":
                return "&#x1f404;";
            case "relieved":
                return "&#x1f60c;";
            case "expressionless":
                return "&#x1f611;";
            case "couch_and_lamp":
                return "&#x1f6cb;";
            case "bed":
                return "&#x1f6cf;";
            case "pray::skin-tone-3":
                return "&#x1f64f;";
            case "motor_scooter":
                return "&#x1f6f5;";
            case "confounded":
                return "&#x1f616;";
            case "scales":
                return "&#x2696;";
            case "carrot":
                return "&#x1f955;";
            case "clock3":
                return "&#x1f552;";
            case "apple":
                return "&#x1f34e;";
            case "elephant":
                return "&#x1f418;";
            case "meat_on_bone":
                return "&#x1f356;";
            case "herb":
                return "&#x1f33f;";
            case "bearded_person":
                return "&#x1f9d4;";
            case "strawberry":
                return "&#x1f353;";
            case "kiwifruit":
                return "&#x1f95d;";
            case "bust_in_silhouette":
                return "&#x1f464;";
            case "dollar":
                return "&#x1f4b5;";
            case "notes":
                return "&#x1f3b6;";
            case "japanese_goblin":
                return "&#x1f47a;";
            case "balance":
                return "&#x2696;";
            case "racing_motorcycle":
                return "&#x1f3cd;";
            case "bird":
                return "&#x1f426;";
            case "turtle":
                return "&#x1f422;";
            case "baseball":
                return "&#x26be;";
            case "frog":
                return "&#x1f438;";
            case "tongue":
                return "&#x1f445;";
            case "cut_of_meat":
                return "&#x1f969;";
            case "hankey":
                return "&#x1f4a9;";
            case "one":
                return "&#x0031;";
            case "ringed_planet":
                return "&#x1fa90;";
            case "bowl_with_spoon":
                return "&#x1f963;";
            case "grapes":
                return "&#x1f347;";
            case "cold_sweat":
                return "&#x1f630;";
            case "male_zombie":
                return "&#x1f9df;";
            case "musical_note":
                return "&#x1f3b5;";
            case "bomb":
                return "&#x1f4a3;";
            case "bangbang":
                return "&#x203c;";
            case "face_with_hand_over_mouth":
                return "&#x1f92d;";
            case "face_vomiting":
                return "&#x1f92e;";
            case "hocho":
                return "&#x1f52a;";
            case "tomato":
                return "&#x1f345;";
            case "cat":
                return "&#x1f408;";
            case "alien":
                return "&#x1f47d;";
            case "bread":
                return "&#x1f35e;";
            case "100":
                return "&#x1f4af;";
            case "smirk":
                return "&#x1f60f;";
            case "zap":
                return "&#x26a1;";
            case "man-facepalming":
                return "&#x1f926;";
            case "heart":
                return "&#x2764;";
            case "disappointed":
                return "&#x1f61e;";
            case "bell":
                return "&#x1f514;";
            case "drum_with_drumsticks":
                return "&#x1f941;";
            case "hospital":
                return "&#x1f3e5;";
            case "smiling_imp":
                return "&#x1f608;";
            case "muscle":
                return "&#x1f4aa;";
            case "fox_face":
                return "&#x1f98a;";
            case "camping":
                return "&#x1f3d5;";
            case "ship":
                return "&#x1f6a2;";
            case "passenger_ship":
                return "&#x1f6f3;";
            case "dog":
                return "&#x1f415;";
            case "moneybag":
                return "&#x1f4b0;";
            default:
                throw new Exception(name);
        }
    }

    private string CodeBlockMatchAction(Match match)
    {
        var target = match.Groups[1].Value;
        return "<code>" + target.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;").Replace(" ", "&nbsp;") + "</code>";
    }

    public string ChannelName => channelName;
}
