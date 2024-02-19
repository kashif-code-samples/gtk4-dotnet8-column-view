using ColumnView.App;

var application = Gtk.Application.New("org.kashif-code-samples.columnview.sample", Gio.ApplicationFlags.FlagsNone);
application.OnActivate += (sender, args) =>
{
    var window = new ColumnViewWindow
    {
        Application = application
    };
    window.Show();
};
return application.RunWithSynchronizationContext(null);