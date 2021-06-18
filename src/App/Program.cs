using Fclp;
using System;

namespace FHIRValidate.App {
    class Program {
        public static CommandArgs Params { get; set; }

        static void Main(string[ ] args) {
            var p = new FluentCommandLineParser<CommandArgs>();

            p.Setup(arg => arg.Directory)
                .As('d', "Directory")
                .Required();

            var arg = p.Parse(args);
            if (arg.HasErrors == true) {
                string err = "Error in arguments";
                Console.WriteLine(err);
                throw new Exception(err);
            }

            Params = p.Object;

            FHIRValidate.Dev.Validate v = new Dev.Validate(Params.Directory);

            v.Execute();
        }

    }

    public class CommandArgs {
        public string Directory { get; set; }
    }
}
