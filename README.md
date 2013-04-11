# ChatSharp

A C# library for chatting on an IRC (Internet Relay Protocol) network.

Supports most of RFC 1459 and a little of 2812.

ChatSharp is still a young project. You should expect many things not to work, some features to be missing, and some
things to change later on.

## Example

Here's an example of using this library:

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

## Compiling

To compile ChatSharp on Windows, add `C:\Windows\Microsoft.NET\Framework\v4.0.30319` to your PATH. Then, from the root of
the ChatSharp repository, run this:

    msbuild

On Linux and Mac, install Mono, then run this from the root of the CraftSharp repository:

	xbuild

## Getting Help

If you find a bug or wish to request a feature, [create a GitHub issue](https://github.com/SirCmpwn/ChatSharp/issues/new).
You may also create an issue to simply ask a question.

## Licensing

ChatSharp uses the permissive [MIT license](http://www.opensource.org/licenses/mit-license.php/).

In a nutshell:

* You are not restricted on usage of ChatSharp; commercial, private, etc, all fine.
* The developers are not liable for what you do with it.
* ChatSharp is provided "as is" with no warranty.