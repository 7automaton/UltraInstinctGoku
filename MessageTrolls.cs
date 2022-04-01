using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using ImageMagick;
using System.Net;

namespace UltraInstinctGoku
{
    internal class MessageTrolls
    {
        List<GokuRule> _rules;
        Random _random;
        //WebClient _webClient;

        /// <summary>
        /// When this class is instantiated, the Goku rules are
        /// read from JSON and deserialized into a persistent C# object.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the list is empty.</exception>
        internal MessageTrolls()
        {
            _random = new Random();
            // _webClient = new WebClient();
            /// Read the rules JSON and convert it.
            using (StreamReader r = new StreamReader("gokurules.json"))
            {
                string json = r.ReadToEnd();
                _rules = JsonConvert.DeserializeObject<List<GokuRule>>(json)
                    ?? throw new ArgumentNullException("_rules", "The list of Goku rules list is empty.");
            }
        }

        /// <summary>
        /// Every time a message is received in a server,
        /// this function is fired.
        /// </summary>
        /// <param name="message">The message that was sent in chat.</param>
        /// <returns>Task.CompletedTask</returns>
        internal Task MessageReceived(SocketMessage message)
        {
            /// Prevent the bot from epically trolling itself infinitely,
            /// or responding to bots like Mudae
            if (message.Author.IsBot) return Task.CompletedTask;

            /// Aborts the bot if no list was found, or the list is empty.
            if ((_rules != null) && (!_rules.Any()))
                throw new NullReferenceException("No list object was found, or it is empty.");

            /// Now we scan the message for every keyword until we find one.
            List<GokuRule> rulesBroken = new List<GokuRule>();
            foreach(var rule in _rules)
            {
                bool cont = false;
                    //Console.WriteLine(string.Concat(rule.keywords));
                foreach (string keyword in rule.keywords)
                {
                    if (!cont)
                    {
                        if (message.Content.ToLower().Contains(keyword))
                        {
                            rulesBroken.Add(rule);
                            /// If a rule is found, skip trying to detect the same rules again.
                            cont = true;
                            Console.WriteLine(message.Content);
                            Console.WriteLine(keyword);
                            Console.WriteLine("Rule broken: " + keyword + "\n");
                        }
                    }
                }
            }

            /// Only do the rule sorting and parsing if a rule has been broken.
            if (rulesBroken.Any())
            {
                /// TODO add broken rule tracker

                /// Take 2 random rules from the rulesBroken list to display in the top field,
                /// then sort them in order of rule number.
                /// https://stackoverflow.com/questions/48087/select-n-random-elements-from-a-listt-in-c-sharp
                List<GokuRule> randomRulesBroken = rulesBroken.OrderBy(x => _random.Next()).Take(2).ToList();
                randomRulesBroken = ReorderGokuRules(randomRulesBroken);
                foreach (GokuRule rule in randomRulesBroken)
                {
                    Console.Write(rule.rulename);
                }
                Console.WriteLine("\n");

                /// This generates the header for the embed to use.
                bool plural = (rulesBroken.Count > 1 ? true : false);
                string header = String.Format("⚠️ {0}RULE{1} BROKEN", 
                    (plural ? String.Format("{0} ", rulesBroken.Count) : ""),
                    (plural ? "S" : "")
                );

                /// This generates the field for the embed to use.
                string field = "";
                for (var i = 0; i < (randomRulesBroken.Count >= 2 ? 2 : 1); i++)
                    field += String.Format("RULE {0}: {1}\n", randomRulesBroken[i].rulenumber, randomRulesBroken[i].rulename);
                if (rulesBroken.Count > 2)
                    field += String.Format("+ {0} more 💀", rulesBroken.Count - 2);

                /// If there's a Goku rule, then post its Gif.
                /// TODO Also, mark Gif progress.
                /// Generate an embed for the tenor image.
                EmbedBuilder embed = new EmbedBuilder();
                embed.AddField(header, field)
                /// If multiple rules are broken, pick one random image to display.
                .WithImageUrl(randomRulesBroken[0].imagelink)
                    .WithColor(new Color(254, 251, 255));

                // ==> FULL SEND ==>
                message.Channel.SendMessageAsync(embed: embed.Build());
                //message.Channel.SendMessageAsync();
            }


            return Task.CompletedTask;
        }

        /// <summary>
        /// Scans a message string.
        /// </summary>
        /// <param name="message">The text to scan, in string form.</param>
        /// <returns>A list of rule numbers broken that were found in the string.</returns>
        internal static List<int> ScanMessage(string message)
        {
            return new List<int>();
        }

        /// <summary>
        /// Resizes and saves GIF images from the internet to a channel.
        /// </summary>
        /// <param name="imageLink">A valid link to a .gif from the internet.</param>
        /// <returns>A resized local path to the resized .gif.</returns>
        //internal static string ResizeAndCacheGif(GokuRule rule)
        //{

        //    _webClient.DownloadFile(rule.imagelink, );
        //    var newWidth = 100;
        //    using (var collection = new MagickImageCollection(new FileInfo(@"C:\test.gif")))
        //    {
        //        collection.Coalesce();
        //        foreach (var image in collection)
        //        {
        //            image.Resize(newWidth, 0);
        //        }
        //        collection.Write(@"c:\resized.gif");
        //    }
        //    return "-1";
        //}

        /// <summary>
        /// Reorders GokuRule lists from lowest to highest rule.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static List<GokuRule> ReorderGokuRules(List<GokuRule> list)
        {
            // list.OrderBy(o => o.rulenumber);
            list.Sort();
            return list;
        }
    }

    /// <summary>
    /// Sortable data structure for Goku rules.
    /// </summary>
    public class GokuRule : IComparable<GokuRule>
    {
        public int rulenumber;
        public string rulename;
        public List<string> keywords;
        public string imagelink;

        public int CompareTo(GokuRule? other)
        {
            if (other == null) return 0;
            if (other.rulenumber > rulenumber) return -1; else return 1;
        }
    }
}
