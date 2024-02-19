namespace ColumnView.App;

using Gtk;
using static Gtk.SignalListItemFactory;

public class UserData : GObject.Object
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Sales { get; set; }
    public int ProgressPercentage { get; set; }

    public UserData(int id, string name, decimal sales, int progressPercentage)
        : base(true, Array.Empty<GObject.ConstructArgument>())
    {
        Id = id;
        Name = name;
        Sales = sales;
        ProgressPercentage = progressPercentage;
    }

    public static UserData[] SampleUserData()
    {
        return
        [
            new UserData(1, "Leanne Graham", 50_000, 25),
            new UserData(2, "Ervin Howell", 35_000, 50),
            new UserData(3, "Patricia Lebsack", 40_000, 40),
            new UserData(4, "Chelsey Dietrich", 65_000, 65),
            new UserData(5, "Clementine Bauch", 20_000, 75),
        ];
    }
}

public class ColumnViewWindow : Window
{
    public ColumnViewWindow()
        : base()
    {
        Title = "Gtk::ColumnView (Gio::ListStore)";
        SetDefaultSize(400, 400);

        var model = Gio.ListStore.New(UserData.GetGType());
        foreach (var userData in UserData.SampleUserData())
        {
            model.Append(userData);
        }

        var selectionModel = SingleSelection.New(model);
        selectionModel.Autoselect = false;
        selectionModel.CanUnselect = true;

        var columnView = ColumnView.New(selectionModel);
        columnView.AddCssClass("data-table");

        var scrolledWindow = ScrolledWindow.New();
        scrolledWindow.Child = columnView;

        Child = scrolledWindow;

        // Id Column
        var listItemFactory = SignalListItemFactory.New();
        listItemFactory.OnSetup += (_, args) => OnSetupLabel(args, Align.End);
        listItemFactory.OnBind += (_, args) => OnBindText(args, (ud) => ud.Id.ToString());

        var idColumn = ColumnViewColumn.New(nameof(UserData.Id), listItemFactory);
        columnView.AppendColumn(idColumn);

        // Name Column
        listItemFactory = SignalListItemFactory.New();
        listItemFactory.OnSetup += (_, args) => OnSetupLabel(args, Align.Start);
        listItemFactory.OnBind += (_, args) => OnBindText(args, (ud) => ud.Name);

        var nameColumn = ColumnViewColumn.New(nameof(UserData.Name), listItemFactory);
        columnView.AppendColumn(nameColumn);

        // Sales Column
        listItemFactory = SignalListItemFactory.New();
        listItemFactory.OnSetup += (_, args) => OnSetupLabel(args, Align.End);
        listItemFactory.OnBind += (_, args) => OnBindText(args, (ud) => $"{ud.Sales:C}");

        var salesColumn = ColumnViewColumn.New(nameof(UserData.Sales), listItemFactory);
        columnView.AppendColumn(salesColumn);

        // Percentage Column
        listItemFactory = SignalListItemFactory.New();
        listItemFactory.OnSetup += OnSetupProgress;
        listItemFactory.OnBind += OnBindProgress;

        var percentageColumn = ColumnViewColumn.New(nameof(UserData.ProgressPercentage), listItemFactory);
        columnView.AppendColumn(percentageColumn);
    }

    private void OnSetupLabel(SetupSignalArgs args, Align align)
    {
        if (args.Object is not ListItem listItem)
        {
            return;
        }

        var label = Label.New(null);
        label.Halign = align;
        listItem.Child = label;
    }

    private void OnBindText(BindSignalArgs args, Func<UserData, string> getText)
    {
        if (args.Object is not ListItem listItem)
        {
            return;
        }

        if (listItem.Child is not Label label) return;
        if (listItem.Item is not UserData userData) return;

        label.SetText(getText(userData));
    }

    private void OnSetupProgress(SignalListItemFactory sender, SetupSignalArgs args)
    {
        if (args.Object is not ListItem listItem)
        {
            return;
        }

        var progressBar = ProgressBar.New();
        progressBar.SetShowText(true);
        listItem.Child = progressBar;
    }

    private void OnBindProgress(SignalListItemFactory sender, BindSignalArgs args)
    {
        if (args.Object is not ListItem listItem)
        {
            return;
        }

        if (listItem.Child is not ProgressBar progressBar) return;
        if (listItem.Item is not UserData userData) return;

        progressBar.SetFraction(userData.ProgressPercentage * 0.01);
    }
}
