using Microsoft.Extensions.Options;
using Xunit;

public class MailerTests
{
    private static Mailer Make(string host, int port) => new(Options.Create(new MailSettings { Host = host, Port = port, From = "a@b.c" }));

    [Fact] public void From_FromSettings() => Assert.Equal("a@b.c", Make("h", 1).From());
    [Theory] [InlineData("smtp", 25, true)] [InlineData("", 25, false)] [InlineData("smtp", 0, false)]
    public void IsConfigured(string host, int port, bool expected) => Assert.Equal(expected, Make(host, port).IsConfigured());
    [Fact] public void Describe_Configured() => Assert.Equal("smtp:25", Make("smtp", 25).Describe());
    [Fact] public void Describe_Missing() => Assert.Equal("not configured", Make("", 0).Describe());
}
