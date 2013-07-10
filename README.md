# ChatSharp

A C# library for chatting on an IRC (Internet Relay Protocol) network.

Supports a lot of RFC 1459 and a little of 2812. Should be sufficient for most of your IRC bot-making needs.

## Example

Here's an example of using this library:

```csharp
var client = new IrcClient("irc.freenode.net", new IrcUser("ChatSharp", "ChatSharp"));
client.ConnectionComplete += (s, e) => client.Join("#botwar");
client.ChannelMessageRecieved += (s, e) =>
{
    var channel = client.Channels[e.PrivateMessage.Source];
    // Respond to a few commands
    if (e.PrivateMessage.Message == ".list")
        channel.SendMessage(string.Join(", ", channel.Users.Select(u => u.Nick)));
    else if (e.PrivateMessage.Message.StartsWith(".ban "))
    {
        if (!channel.Operators.Contains(client.User))
        {
            channel.SendMessage("I'm not an op here!");
            return;
        }
        var target = e.PrivateMessage.Message.Substring(5);
        client.WhoIs(target, whois => channel.Ban("*!*@" + whois.User.Hostname));
    }
};
client.Connect();

while (true) ; // Do nothing
```

## Compiling

To compile ChatSharp on Windows, add `C:\Windows\Microsoft.NET\Framework\v4.0.30319` to your PATH. Then, from the root
of the ChatSharp repository, run this:

    msbuild

On Linux and Mac, install Mono, then run this from the root of the CraftSharp repository:

    xbuild

You can also compile it with Visual Studio/SharpDevelop/MonoDevelop/etc.

## Getting Help

If you find a bug or wish to request a feature,
[create a GitHub issue](https://github.com/SirCmpwn/ChatSharp/issues/new). You may also create an issue to simply ask a
question.

## Development

I only add to this project when I need something more from it. As a result, it tends to only support the things I need
it to. Fortunately, for this particular project, I need a nice featureset and it supports quite a lot of things.
However, if you need it to do something else, it's probably best for you to implement it yourself. The code isn't that
scary and I'm sure you can do it. You can create a GitHub issue asking for your feature, but it may be done months
later, or never. Feel free to submit a pull request with your changes once you've added them - your contributions are
appriciated.

## Licensing

ChatSharp uses the permissive [MIT license](http://www.opensource.org/licenses/mit-license.php/).

In a nutshell:

* You are not restricted on usage of ChatSharp; commercial, private, etc, all fine.
* The developers are not liable for what you do with it.
* ChatSharp is provided "as is" with no warranty.
