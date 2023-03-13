using Producer;

string input = string.Empty;
string routingKey = string.Empty;
do
{
    input = string.Empty;
    Console.WriteLine("Enter message");
    input = Console.ReadLine()!;
    Console.WriteLine("Enter routing key");
    routingKey = Console.ReadLine()!;
    Sender sender = new();
    sender.SendMessage(input, routingKey);
    Console.WriteLine($"Sending message {input}");
}while(!(string.IsNullOrEmpty(input)));