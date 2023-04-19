using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer
{
    class Program
    {
        static Queue<int> buffer = new Queue<int>();//Opretter en Queue til at holde produceret ting
        static object obj = new object();//Laver et objekt som vi bruger tíl at låse adgang til bufferen
        static int maxBufferSize = 10;//Max størrelse på bufferen

        static void Produce()
        {
            while (true)
            {
                lock (obj)//Låser objektet
                {
                    while (buffer.Count >= maxBufferSize)//Hvis bufferen er fyldt skal den vente med at producere mere
                    {
                        Console.WriteLine("Buffer is full, producer is waiting...");//Giver brugeren besked om bufferen er fuld
                        Monitor.Wait(obj);//Venter med at producere indtil den får besked på at fortsætte igen
                    }
                    int item = GenerateItem();//Kalder en metode som generer et nyt element
                    buffer.Enqueue(item);//Tilføjer elementet til bufferen
                    Console.WriteLine($"Produced item: {item}");//giver brugeren besked om der er blevet produceret et element
                    Monitor.Pulse(obj);//Notiferer tråden som venter om at der er en ændring på det låste objekts status
                }
                Thread.Sleep(1000);//Pauser tråden i 1000ms
            }
        }
        static void Consume()
        {
            while (true)
            {
                lock (obj)//Låser objektet
                {
                    while (buffer.Count == 0)//Hvis bufferen er tom skal den vente med at consume mere
                    {
                        Console.WriteLine("Buffer is empty, consumer is waiting...");//Giver brugeren besked om at bufferen er tom 
                        Monitor.Wait(obj);//Venter med at consume indtil den får besked om at fortsætte igen
                    }
                    int item = buffer.Dequeue();//fjerner elementer fra bufferen
                    Console.WriteLine($"Consumed item: {item}");//Giver brugeren besked om der er blevet consumet et element
                    Monitor.Pulse(obj);//Notiferer tråden som venter om at der er en ændring på det låste objekts status
                }
                Thread.Sleep(1000);//pauser tråden i 1000ms
            }
        }
        static int GenerateItem()
        {
            return new Random().Next(1, 101);//Generere et tilfælidt tal mellem 1 og 100 og returnerer det
        }
        static void Main(string[] args)
        {
            Thread producer = new Thread(Produce);//Laver en tråd for at producere et element
            Thread consumer = new Thread(Consume);//Laver en tråd for at consume et element
            producer.Start();//Starter producerings tråden
            consumer.Start();//Starter consumerings tråden
        }
    }
}
