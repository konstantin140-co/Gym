using Gym.Data.Context;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class VisitStatisticsReportForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly DateTimePicker _dtpFrom = new() { Width = 140, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker _dtpTo = new() { Width = 140, Format = DateTimePickerFormat.Short };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
    private readonly Label _lblTotal = new() { AutoSize = true, Font = UiTheme.BodyFont, Tag = "secondary" };

    public VisitStatisticsReportForm()
    {
        SetActiveNavKey("visit-stats");
        NavigationService.NavigateTo(this, "visit-stats");

        var header = new PageHeaderControl();
        header.SetTitle("Статистика посещений", "Количество визитов за период");
        header.ConfigureAction("", false);
        header.HideSearch();

        _dtpFrom.Value = DateTime.Today.AddDays(-30);
        _dtpTo.Value = DateTime.Today;

        var filter = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8) };
        filter.Controls.Add(new Label { Text = "С:", AutoSize = true, Margin = new Padding(0, 8, 4, 0), Tag = "secondary" });
        filter.Controls.Add(_dtpFrom);
        filter.Controls.Add(new Label { Text = "По:", AutoSize = true, Margin = new Padding(12, 8, 4, 0), Tag = "secondary" });
        filter.Controls.Add(_dtpTo);
        var btn = new Button { Text = "Показать", Width = 100 };
        UiTheme.StylePrimaryButton(btn);
        btn.Click += (_, _) => LoadData();
        filter.Controls.Add(btn);

        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Тип занятия", Name = "Activity", Width = 200 },
            new DataGridViewTextBoxColumn { HeaderText = "Кол-во визитов", Name = "Count", Width = 120 });

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 32 };
        footer.Controls.Add(_lblTotal);
        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        card.Controls.Add(_grid);

        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(filter);
        ContentPanel.Controls.Add(header);

        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
        DataGridViewSortHelper.Attach(_grid);
    }

    private void LoadData()
    {
        var from = _dtpFrom.Value.Date;
        var to = _dtpTo.Value.Date.AddDays(1).AddTicks(-1);
        var stats = _db.Visits.AsEnumerable()
            .Where(v => v.DateTime >= from && v.DateTime <= to)
            .GroupBy(v => v.ActivityType)
            .Select(g => new { Activity = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        _grid.Rows.Clear();
        var total = 0;
        foreach (var row in stats)
        {
            _grid.Rows.Add(row.Activity, row.Count);
            total += row.Count;
        }
        _lblTotal.Text = $"  Всего визитов за период: {total}";
    }
}
