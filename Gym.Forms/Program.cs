using Microsoft.EntityFrameworkCore;
using Gym.Data.Context;
using Gym.Forms.Forms;

namespace Gym.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var db = new AppDbContext();
        db.Database.Migrate();

        Application.Run(new MainForm());
    }
}
