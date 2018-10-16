using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    /// <summary>
    ///  Provides information on how to use commands within this bot.
    /// </summary>
    [Group("help")]
    [Name("Help")]
    [Summary("Provides tools for getting command help text")]
    public class Help : ModuleBase<SocketCommandContext>
    {

        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public Help(IServiceProvider prov, CommandService comm)
        {
            _commands = comm;
            _provider = prov;
        }

        [Command]
        [Name("help")]
        [Summary("Provides a list of all commands and a short summary of them")]
        [Remarks("Can be used with the -v flag for more verbose output: `help -v help`")]
        public async Task HelpAsync(params string[] args)
        {
            EmbedBuilder output = new EmbedBuilder();

            bool verbose = false;
            string module = "";
            for(int i = 0; i < args.Length; i++)
            {
                //only check flags at the beginning
                if (!args[i].StartsWith("-"))
                {
                    module = string.Join(" ", args.Skip(i));
                    break;
                }

                if (args[i].Equals("-v"))
                {
                    verbose = true;
                }
            }

            if (module == "")
            {
                foreach (var mod in _commands.Modules.Where(m => m.Parent == null).OrderBy(m => m.Name))
                {
                    AddHelp(mod, ref output, verbose);
                }

                output.Footer = new EmbedFooterBuilder
                {
                    Text = "Use 'help <module>' to get help with a module."
                };
            }
            else
            {
                var mod = _commands.Modules.FirstOrDefault(m => m.Name.ToLower() == module.ToLower());
                if (mod == null) { await ReplyAsync("No module could be found with that name."); return; }

                output.Title = mod.Name;
                output.Description = $"{mod.Summary}\n" +
                (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
                (mod.Aliases.Any() ? $"Prefix(es): {string.Join(",", mod.Aliases)}\n" : "") +
                (mod.Submodules.Any() ? $"Submodules: {string.Join(", ", mod.Submodules.Select(x => x.Name))}\n" : "") + " ";
                AddCommands(mod, ref output, verbose);
            }

            await ReplyAsync("", embed: output.Build());
        }

        private void AddHelp(ModuleInfo module, ref EmbedBuilder builder, bool verbose)
        {
            builder.AddField(f =>
            {
                f.WithName($"**{module.Name}**");
                f.WithValue($"{module.Summary}\n"
                    + (verbose && !string.IsNullOrEmpty(module.Remarks) ? $"{module.Remarks}\n" : "")
                    + (module.Commands.Any() ? $"Commands: {string.Join(", ", module.Commands.Select(x => x.Name))}\n" : "")
                    + (module.Submodules.Any() ? $"Submodules: {string.Join(", ", module.Submodules.Select(x => x.Name))}" : "")
                );
            });
        }

        private void AddCommands(ModuleInfo module, ref EmbedBuilder builder, bool verbose)
        {
            foreach(var command in module.Commands.OrderBy(c => c.Name))
            {
                command.CheckPreconditionsAsync(Context, _provider).GetAwaiter().GetResult();
                AddCommand(command, ref builder, verbose);
            }
        }

        private void AddCommand(CommandInfo command, ref EmbedBuilder builder, bool verbose)
        {
            builder.AddField(f =>
            {
                f.WithName($"**{command.Name}**");
                f.WithValue($"{command.Summary}\n"
                    + (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "")
                    + (verbose && command.Parameters.Any() ? $"{GetParameterSummaries(command)}" : "")
                    + (command.Aliases.Count >= 2 ? $"Aliases: {string.Join(", ", command.Aliases.Select(x => $"`+{x}`"))}\n" : "")
                    + $"Usage: `+{GetPrefix(command)}{GetParametersInUsage(command)}`");
            });
        }

        private string GetParameterSummaries(CommandInfo command)
        {
            if (!command.Parameters.Any()) return "";

            StringBuilder output = new StringBuilder();
            foreach (var param in command.Parameters)
            {
                if (param.Summary != null)
                {
                    output.Append($"`{param.Name}`: {param.Summary}\n");
                }
            }
            return output.ToString();
        }

        private string GetParametersInUsage(CommandInfo command)
        {
            if (!command.Parameters.Any()) return "";

            StringBuilder output = new StringBuilder();
            foreach (var param in command.Parameters)
            {
                if (param.IsOptional)
                    output.Append($"[{param.Name} = {param.DefaultValue}] ");
                else if (param.IsMultiple)
                    output.Append($"|{param.Name}| ");
                else if (param.IsRemainder)
                    output.Append($"...{param.Name} ");
                else
                    output.Append($"<{param.Name}> ");
            }
            return output.ToString();
        }

        private string GetPrefix(CommandInfo command)
        {
            var output = $"{command.Aliases.FirstOrDefault()} ";
            return output;
        }

        private string GetPrefix(ModuleInfo module)
        {
            string output = "";
            if (module.Parent != null) output = $"{GetPrefix(module.Parent)}{output}";
            if (module.Aliases.Any())
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            return output;
        }
    }
}
