using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace SimpleHashingExample
{
    class Program
    {
        private static string Separator => new String('-', 100);

        static void Main(string[] args)
        {
            var pswd = PromptPassword();

            Console.WriteLine(Separator);

            var hashAlgorithm = PromptAlgorithm();

            Console.WriteLine(Separator);

            var hashCount = PromptHashCount();

            Console.WriteLine(Separator);

            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine("Hashing \"{0}\" {1} time(s) using {2}", pswd, hashCount, FormatAlgorithmName(hashAlgorithm));

            var hashed = HashPassword(pswd, hashCount, hashAlgorithm);           

            timer.Stop();

            Console.WriteLine(hashed);
            Console.WriteLine("in {0} milliseconds", TimeSpan.FromTicks(timer.ElapsedTicks).TotalMilliseconds);
            Console.WriteLine(Separator);
        }

        private static string HashPassword(string password, Int16 hashCount, HashAlgorithm hashAlgorithm)
        {
            using(hashAlgorithm)
            {
                var passBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = hashAlgorithm.ComputeHash(passBytes);

                if(hashCount > 1)
                {
                    hashBytes = Enumerable
                                    .Range(1, hashCount - 1) // -1 because it's already been hashed once
                                    .Aggregate(hashBytes, (bytes, i) => hashAlgorithm.ComputeHash(bytes));
                }

                return string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }
        }

        private static HashAlgorithm PromptAlgorithm()
        {
            var algorithms = new List<Type> 
            { 
                typeof(MD5),
                typeof(SHA1),
                typeof(SHA256),
                typeof(SHA384),
                typeof(SHA512),
            };

            while(true)
            {
                Console.WriteLine($"Enter the option for the algorithm you wish to use...");   

                for(var i = 0; i < algorithms.Count; i++)
                {
                    Console.WriteLine($"{i} - {FormatAlgorithmName(algorithms[i])}");   
                }

                try
                {
                    var optionStr = Console.ReadLine();

                    var option = int.Parse(optionStr);

                    if(option < 0 || option > algorithms.Count - 1) throw new ArgumentOutOfRangeException();

                    return (HashAlgorithm) algorithms[option].GetMethod("Create", new Type[] {}).Invoke(null, null);
                }
                catch
                {
                    Console.WriteLine("Please select a valid option!");   
                }
            }
        }

        private static short PromptHashCount()
        {
            while(true)
            {
                Console.WriteLine($"Enter the number of times to hash the password (Between 1 and {Int16.MaxValue})...");   

                var countStr = Console.ReadLine();

                try
                {
                    var count = Int16.Parse(countStr);

                    if(count < 1) throw new ArgumentOutOfRangeException();

                    return count;
                }
                catch
                {
                    Console.WriteLine("Please enter a valid number!");
                }
            }
        }

        private static string PromptPassword()
        {
            while(true)
            {
                Console.WriteLine("Enter a password...");

                var pswd = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(pswd))
                {
                    Console.WriteLine("Please enter a valid password!");
                }
                else
                {
                    return pswd;
                }
            }
        }

        private static string FormatAlgorithmName(HashAlgorithm hashAlgorithm) => FormatAlgorithmName(hashAlgorithm.GetType());

        private static string FormatAlgorithmName(Type hashAlgorithm) => hashAlgorithm.FullName.Split(".").Last().Replace("+Implementation", "");
    }
}
