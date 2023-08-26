Imports System
Imports Discord
Imports Discord.Commands
Imports Discord.WebSocket
Imports System.Reflection
Imports System.Threading.Tasks

Public Module EchoBot
    Private WithEvents _client As DiscordSocketClient
    Private WithEvents _commands As CommandService
    Private WithEvents _commandHandler As CommandHandler

    Public Sub Main()
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        _client = New DiscordSocketClient()
        _commands = New CommandService()

        AddHandler _client.Log, AddressOf LogAsync
        AddHandler _client.Ready, AddressOf ReadyAsync

        Await _client.LoginAsync(TokenType.Bot, "YOUR_BOT_TOKEN")
        Await _client.StartAsync()

        _commandHandler = New CommandHandler(_client, _commands)
        Await _commandHandler.InstallCommandsAsync()

        Await Task.Delay(-1)
    End Function

    Private Function LogAsync(logMessage As LogMessage) As Task
        Console.WriteLine(logMessage)
        Return Task.CompletedTask
    End Function

    Private Function ReadyAsync() As Task
        Console.WriteLine("Bot is connected and ready!")
        Return Task.CompletedTask
    End Function

    Private Async Function HandleCommandAsync(message As SocketMessage) As Task
        Dim userMessage = TryCast(message, SocketUserMessage)
        Dim context = New SocketCommandContext(_client, userMessage)

        If userMessage.Author.IsBot Then Return

        Dim argPos = 0
        If userMessage.HasStringPrefix("!", argPos) Then
            Dim result = Await _commands.ExecuteAsync(context, argPos, Nothing)
            If Not result.IsSuccess Then
                Console.WriteLine(result.ErrorReason)
            End If
        End If
    End Function

    Private Async Function OnMessageReceived(message As SocketMessage) As Task Handles _client.MessageReceived
        Await HandleCommandAsync(message)
    End Function

    Public Class EchoModule
        Inherits ModuleBase(Of SocketCommandContext)
        
        <Command("echo")>
        Public Async Function EchoAsync(<Remainder> ByVal text As String) As Task
            Await ReplyAsync(text)
        End Function
    End Class
End Module
