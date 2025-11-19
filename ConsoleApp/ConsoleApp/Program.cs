// See https://aka.ms/new-console-template for more information

using ConsoleApp;
using ConsoleApp.Commands;

Console.WriteLine("Hello, World!");

Dog Richard = new Dog("Rishard");

Richard.AddCommand("Seet", new SeatCommand());
Richard.AddCommand("Voice", new VoiceCommand());

Richard.ExecuteCommand("Seet");
Richard.ExecuteCommand("Voice");

