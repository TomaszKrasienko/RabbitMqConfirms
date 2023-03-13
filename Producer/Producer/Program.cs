using Producer;

string input = string.Empty;
do
{
    input = string.Empty;
    Console.WriteLine("Enter message");
    input = Console.ReadLine();
    Sender sender = new();
    sender.SendMessage(input);
    Console.WriteLine($"Sending message {input}");
}while(!(string.IsNullOrEmpty(input)));