using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace SimpleHashingExample
{
    class Program
    {
        private const int HashCount = 100000;

        static void Main(string[] args)
        {
            var algorithms = new HashAlgorithm[] { 
                MD5.Create(),
                SHA1.Create(),
                SHA256.Create(),
                SHA384.Create(), 
                SHA512.Create(),
            };

            Console.WriteLine("Enter a password...");

            var pswd = Console.ReadLine();
        
            Console.WriteLine("-------------------------------------------------------------------------------------------");

            foreach(var sha in algorithms)
            {
                byte[] passBytes;
                byte[] hashBytes;
                string hashed;

                using(sha)
                {
                    Console.WriteLine("Hashing \"{0}\" {1} time(s) using {2}", pswd, HashCount, sha.GetType().FullName.Split(".").Last().Replace("+Implementation", ""));

                    var timer = new Stopwatch();
                    timer.Start();

                    passBytes = Encoding.UTF8.GetBytes(pswd);
                    hashBytes = sha.ComputeHash(passBytes);

                    foreach(var i in Enumerable.Range(1, HashCount))
                    {
                        hashBytes = sha.ComputeHash(hashBytes);
                    }

                    hashed = string.Concat(hashBytes.Select(b => b.ToString("x2")));

                    timer.Stop();

                    Console.WriteLine("{0} in {1} milliseconds", hashed, TimeSpan.FromTicks(timer.ElapsedTicks).TotalMilliseconds);
                    Console.WriteLine("-------------------------------------------------------------------------------------------");
                }
            }
        }
    }
}
