# BrawrdonBot.NET 🤖
A .NET Core powered Twitter bot!

## What is this?
BrawrdonBot is a simple Twitter bot I created to teach myself about using public APIs. It posts messages that people have sent me from my website onto Twitter to publicly archive them.

## Using BrawrdonBot.NET
These are instructions for using this project as a basis for creating your own Twitter bot. This project isn't a library so you'll have to make changes to the code directly if you want to adapt it.

### Prerequisites
* .NET Core 2.1 SDK
* A Twitter developer account / API keys and tokens

### Downloading
`git clone https://github.com/Brawrdon/BrawrdonBot.NET.git`

### How it works
There are two projects, *Server* and *BrawrdonBot*. *Server* is the main class. It contains a HTTP listener that takes (RESTful-ish) requests and parses them. It attempts to process the request, returning a HTTP error response code and reason if it fails. It currently only accepts `POST` requests to `twitter/post/brawrdonbot/` that contain JSON with the element `message`.

*BrawrdonBot* has methods that generate the required information to make send requests to Twitter. It currently can post tweets and update its description to show if the server is online or offline.

### Setting API Keys
The public repository masks the API keys for security reasons, or else you all could Tweet as me! Place your API keys into the variables found in the file BrawrdonBot/API.cs.

```
public static class Api
{
  public const string ConsumerKey = "";
  public const string ConsumerKeySecret = "";
  public const string OauthToken = "";
  public const string OauthTokenSecret = "";
}
```

### Setting up Server.cs
You can change the server's base URL from the line `_listener.Prefixes.Add("http://localhost:15101/")`. You can find more information about HTTP listener's prefixes from the [.NET Docs](https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=netcore-2.1).

### Changing URL parsing

### Running the bot
The projects main method is in Server.cs. Running the main method will allow requests to be sent to your server.
