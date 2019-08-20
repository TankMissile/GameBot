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
                var lines = File.ReadAllLines(path);
                if(lines.Count() == 0)
                {
                    await ReplyAsync("There are no items in your to-do list!");
                    return;
                }

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
                await ReplyAsync("You haven't started a to-do list!");
            }
        }

        [Command("remove")]
        [Alias("done", "delete", "minus")]
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
            await AddTodoSimpleAsync(todo);
        }

        [Command("clear")]
        [Summary("Clears all todo's")]
        async Task ClearTodosAsync()
        {
            await ReplyAsync("Clearing Todos for user " + Context.User.Username + ". Here's what I deleted:");
            await TodoListAsync();
            File.WriteAllLines(GetUserTodoPath(Context.User), new List<string>());
        }


        [Command, Name("todo")]
        [Priority(-1)]
        [Summary("Adds a to-do to the user's list")]
        async Task AddTodoSimpleAsync([Remainder] string todo)
        {
            int id = await AppendTodoToFile(todo);
            if(id == -1)
            {
                await ReplyAsync(RNG.Error.GetRandomErrorMessage("There was a problem adding your to-do!"));
                return;
            }
            await ReplyAsync("To-do added for " + Context.User.Username + " with id " + id);
        }

        string GetUserTodoPath(Discord.WebSocket.SocketUser user)
        {
            return GameBot.GetPath("Todo_Lists/" + user.Username + "_todo.txt");
        }

        async Task<int> AppendTodoToFile(string line)
        {
            bool newLine = File.ReadAllText(GetUserTodoPath(Context.User)).Length > 0;
            var file = File.AppendText(GetUserTodoPath(Context.User));
            try
            {
                await file.WriteAsync((newLine ? "\n" : "") + line);
                await file.FlushAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                file.Close();
                return -1;
            }

            file.Close();

            return File.ReadAllLines(GetUserTodoPath(Context.User)).Count();
        }
    }
}
