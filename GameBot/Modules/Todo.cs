using Discord.Commands;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GameBot.Modules
{
    [Group("todo")]
    [Name("To-Do")]
    [Summary("A to-do list for users to keep track of what they need... to do")]
    public class Todo : ModuleBase<SocketCommandContext>
    {
        [Command, Name("todo")]
        [Alias("list")]
        [Summary("Displays the user's to-do list")]
        async Task TodoListAsync()
        {
            string path = GetUserTodoPath(Context.User);

            if (File.Exists(path))
            {
                var lines = await File.ReadAllLinesAsync(path);
                StringBuilder msgBuilder = new StringBuilder("To-do list for ").Append(Context.User.Username).Append(":\n```");
                for(int i = 0; i < lines.Length; i++)
                {
                    msgBuilder.Append((i + 1).ToString()).Append(". ");
                    msgBuilder.Append(lines[i]).Append("\n");
                }
                msgBuilder.Append("```");

                await ReplyAsync(msgBuilder.ToString());
            }
            else
            {
                await ReplyAsync("I don't have a to-do list for you.  Start one with `+todo add My task`");
            }
        }

        [Command("remove")]
        [Alias("done", "delete")]
        [Summary("Removes an item from the user's to-do list")]
        async Task RemoveTodoAsync(params int[] ids)
        {
            var lines = File.ReadAllLines(GetUserTodoPath(Context.User)).ToList();
            var sorted = (
                    from element in ids
                    where element <= lines.Count() && element > 0
                    orderby element descending
                    select element-1
                ).Distinct();

            var removed = new List<string>();
            foreach (var id in sorted)
            {
                string msg = String.Format("~~{0}. {1}~~", id+1, lines.ElementAt(id));
                removed.Insert(0, msg);
                lines.RemoveAt(id);
            }
            File.WriteAllText(GetUserTodoPath(Context.User), String.Join("\n", lines));

            await ReplyAsync(String.Join("\n", removed));
        }

        [Command("add")]
        [Summary("Adds a to-do to the user's list")]
        async Task AddTodoAsync([Remainder] string todo)
        {
            var file = File.AppendText(GetUserTodoPath(Context.User));
            await file.WriteAsync("\n" + todo);
            await file.FlushAsync();
            file.Close();

            await ReplyAsync("To-do added for " + Context.User.Username + " with id " + (File.ReadLines(GetUserTodoPath(Context.User)).Count()));
        }


        [Command, Name("todo")]
        [Priority(-1)]
        [Summary("Adds a to-do to the user's list")]
        async Task AddTodoSimpleAsync([Remainder] string todo)
        {
            var file = File.AppendText(GetUserTodoPath(Context.User));
            await file.WriteAsync("\n" + todo);
            await file.FlushAsync();
            file.Close();

            await ReplyAsync("To-do added for " + Context.User.Username + " with id " + (File.ReadLines(GetUserTodoPath(Context.User)).Count()));
        }

        string GetUserTodoPath(Discord.WebSocket.SocketUser user)
        {
            return GameBot.GetPath("Todo_Lists/" + user.Username + "_todo.txt");
        }
    }
}
