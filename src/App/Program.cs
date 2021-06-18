using Fclp;
using System;

namespace FHIRValidate.App {
    class Program {
        public static CommandArgs Params { get; set; }

        static void Main(string[ ] args) {
            Console.WriteLine("Hello World!");
            var p = new FluentCommandLineParser<CommandArgs>();

            p.Setup(arg => arg.Directory)
                .As('d', "Directory")
                .Required();

            Params = p.Object;
            // Params.Directory = "/Users/mthomas/Development/FHIRValidate/TestData";

             
            FHIRValidate.Dev.Validate v = new Dev.Validate(Params.Directory);

            v.Execute();
        }

    }

    public class CommandArgs {
        public string Directory { get; set; }
    }
}
