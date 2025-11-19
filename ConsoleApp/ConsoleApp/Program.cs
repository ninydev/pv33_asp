// See https://aka.ms/new-console-template for more information

using ConsoleApp;
using ConsoleApp.Commands;
using ConsoleApp.Fight;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

// 1. Создаем "Коллекцию сервисов" (это чертеж нашего контейнера)
var services = new ServiceCollection();

// 2. РЕГИСТРАЦИЯ (Настройка)
// Учим контейнер, что такое Dog
services.AddTransient<NeoDog>();
        
// Регистрируем команды. Обрати внимание, интерфейс один (ICommand), а реализации разные.
// Контейнер запомнит их все.
services.AddTransient<ICommand, VoiceCommand>();
services.AddTransient<ICommand, SeatCommand>();

// 3. СБОРКА (Build)
// На этом этапе создается тот самый "Склад готовых объектов" (IServiceProvider)
using var serviceProvider = services.BuildServiceProvider();

// 4. ИСПОЛЬЗОВАНИЕ (Resolve)
// Мы просим только Собаку. 
// Контейнер сам создаст BarkCommand, JumpCommand, соберет их в массив и засунет в Собаку.
var myDog = serviceProvider.GetRequiredService<NeoDog>();
        
myDog.ShowOff();



// Fight barbarian = new Fight();
// barbarian.ChangeLeft(new Axe());
// barbarian.ChangeRight(new Lazer());
//
// barbarian.DoLeft();
// barbarian.DoRight();



// Dog Richard = new Dog("Rishard");
//
// Richard.AddCommand("Seet", new SeatCommand());
// Richard.AddCommand("Voice", new VoiceCommand());
//
// Richard.ExecuteCommand("Seet");
// Richard.ExecuteCommand("Voice");

