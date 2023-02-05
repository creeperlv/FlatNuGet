using FlatNuGet.Core;
using LibCLCC.NET.IO;
using NuGet.Protocol.Plugins;
using System.Linq;

namespace FlatNuGet
{
    public class Arguments
    {
        public string MainParameter;
        public Dictionary<Parameter, List<string>> Options = new Dictionary<Parameter, List<string>>();
        public static Arguments Resolve(string[] args, List<Parameter> parameters)
        {
            Arguments arguments = new Arguments();
            Parameter CurrentP = null;
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item.StartsWith("-") || item.StartsWith("/"))
                {
                    //switches.
                    item = item.Trim().TrimStart('-').TrimStart('/').ToUpper();
                    bool Hit = false;
                    foreach (var _p in parameters)
                    {
                        foreach (var str in _p.Variants)
                        {
                            if (str.ToUpper() == item)
                            {
                                Hit = true;
                                CurrentP = _p;
                                if (arguments.Options.ContainsKey(_p))
                                {
                                }
                                else
                                {
                                    arguments.Options.Add(_p, new List<string>());
                                }
                            }
                            if (Hit) break;
                        }
                        if (Hit) break;
                    }
                }
                else
                {
                    if (CurrentP != null)
                    {
                        arguments.Options[CurrentP].Add(item);
                    }
                    else
                        arguments.MainParameter = item;
                }
            }
            return arguments;
        }
    }
    public class Parameter : IEquatable<Parameter>
    {
        public List<string> Variants;

        public bool Equals(Parameter other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }
    }
    internal class Program
    {
        static void ShowVersion(Type t)
        {
            var dll = t.Assembly.GetName();
            Console.Write(dll.Name);
            Console.Write(":");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(dll.Version);
            Console.ResetColor();
            Console.WriteLine();
        }
        static void PrintVersion()
        {
            Console.WriteLine("FlatNuGet");
            ShowVersion(typeof(FlatNGCore));
            ShowVersion(typeof(PathUtilities));
            ShowVersion(typeof(NuGet.Packaging.PackageReference));
        }
        static void Main(string[] args)
        {

            Parameter Operation = new Parameter { Variants = new List<string> { "O", "OP", "OPERATION" } };
            Parameter ver = new Parameter { Variants = new List<string> { "V", "VER", "VERSION" } };
            var _args = Arguments.Resolve(args, new List<Parameter> { Operation, ver });
            if (_args.Options.ContainsKey(ver))
            {
                PrintVersion();
            }
            if (_args.MainParameter == null) return;
            OperationRouter.actions += ConsoleOperationReciever.Receive;
            FlatNGCore flatNGCore = new FlatNGCore(new FileInfo(_args.MainParameter));
            if (_args.Options.ContainsKey(Operation))
            {
                if (_args.Options[Operation].Count > 0)
                {
                    for (int i = 0; i < _args.Options[Operation].Count; i++)
                    {
                        var item = _args.Options[Operation][i];

                        switch (item.ToLower())
                        {
                            case "init":
                                {
                                    flatNGCore.InitIndexFile();
                                }
                                break;
                            case "update":
                                {
                                    flatNGCore.Update();
                                }
                                break;
                            case "forceupdate":
                                {
                                    flatNGCore.ForceUpdate();
                                }
                                break;
                            case "add":
                                {
                                    i++;
                                    var description = _args.Options[Operation][i];
                                    flatNGCore.Add(description);
                                }
                                break;
                            case "remove":
                                {
                                    i++;
                                    var description = _args.Options[Operation][i];
                                    flatNGCore.Remove(description);
                                }
                                break;
                            case "filter":
                            case "f":
                                {
                                    i++;
                                    var description = _args.Options[Operation][i];
                                    flatNGCore.Filter(description);
                                }
                                break;
                            case "track":
                            case "t":
                                {
                                    i++;
                                    var description = _args.Options[Operation][i];
                                    flatNGCore.Track(description);
                                }
                                break;
                            default:
                                {
                                    Console.WriteLine("Unkown operation:" + item + ".");
                                }
                                break;
                        }
                    }
                }
            }
            ///
            while (ConsoleOperationReciever.OperationCount > 0)
            {
                //Wait for all console output.
            }
        }
    }
}