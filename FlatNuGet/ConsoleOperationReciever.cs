using FlatNuGet.Core;
using System.Runtime.CompilerServices;

namespace FlatNuGet
{
    public class ConsoleOperationReceiver
    {
        internal static int OperationCount = 0;
        static void PrintPkgRef(PackageReference pkg)
        {

            Console.Write($"{pkg.Include}/");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{pkg.Version}");
            Console.ResetColor();
        }
        static object _Lock = new object();
        public static void Receive(Operation operation)
        {
            lock (_Lock)
            {
                OperationCount++;
                switch (operation.OperationType)
                {
                    case OperationType.Common:
                        Console.WriteLine($"{operation.Message}");
                        break;
                    case OperationType.Download:
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Download");
                            Console.ResetColor();
                            Console.Write("]");
                            var pkg = operation.Message as PackageReference;
                            if (pkg != null)
                            {
                                PrintPkgRef(pkg);
                                Console.WriteLine("");
                            }
                            else
                            {
                                Console.WriteLine($"{operation.Message}");
                            }
                        }
                        break;
                    case OperationType.CacheHit:
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Hit Cache");
                            Console.ResetColor();
                            Console.Write("]");
                            var pkg = operation.Message as PackageReference;
                            if (pkg != null)
                            {
                                PrintPkgRef(pkg);
                                Console.WriteLine("");
                            }
                            else
                            {
                                Console.WriteLine($"{operation.Message}");
                            }
                        }
                        break;
                    case OperationType.Extract:
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Extract");
                            Console.ResetColor();
                            Console.Write("]");
                            var combineData = operation.Message as CombinedData;
                            if (combineData != null)
                            {
                                var pref = combineData.L as PackageReference;
                                if (pref != null)
                                {
                                    PrintPkgRef(pref);
                                }
                                else
                                {
                                    Console.Write(combineData.L);
                                }
                                Console.Write(">");
                                Console.Write(combineData.R);
                                Console.WriteLine("");
                            }
                            else
                            {
                                Console.WriteLine($"{operation.Message}");
                            }
                        }
                        break;
                    case OperationType.Error:
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Error");
                            Console.ResetColor();
                            Console.Write("]");
                            {
                                Console.WriteLine($"{operation.Message}");
                            }
                        }
                        break;
                    case OperationType.Warning:
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("Warn");
                            Console.ResetColor();
                            Console.Write("]");
                            {
                                Console.WriteLine($"{operation.Message}");
                            }
                        }
                        break;
                    default:
                        {
                            Console.WriteLine($"{operation.Message}");

                        }
                        break;
                }
                Console.ResetColor();

                OperationCount--;
            }
        }
    }
}