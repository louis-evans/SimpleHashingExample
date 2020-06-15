using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter password");
            var pswd = Console.ReadLine();
            byte[] passBytes;
            byte[] hashBytes;
            string hashed;

            foreach(var sha in new HashAlgorithm[] { SHA1.Create(), SHA256.Create(), SHA384.Create(), SHA512.Create() })
            {
                using(sha)
                {
                    var timer = new Stopwatch();
                    timer.Start();

                    passBytes = Encoding.UTF8.GetBytes(pswd);
                    hashBytes = sha.ComputeHash(passBytes);

                    foreach(var i in Enumerable.Range(1, 100000))
                    {
                        hashBytes = sha.ComputeHash(hashBytes);
                    }

                    hashed = string.Concat(hashBytes.Select(b => b.ToString("x2")));

                    timer.Stop();

                    Console.WriteLine(hashed + " in " + TimeSpan.FromTicks(timer.ElapsedTicks).TotalMilliseconds + " milliseconds");
                }
            }
        }
    }
}
