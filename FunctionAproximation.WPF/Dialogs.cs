using Microsoft.Win32;

namespace FunctionAproximation.WPF;

public static class Dialogs
{
    public static string? GetFilePath(string title, string initiaDirectory)
    {
        var openFileDialog = new OpenFileDialog()
        {
            InitialDirectory = initiaDirectory,
            Title = title,
            Filter = "Text files|*.txt"
        };

        return openFileDialog.ShowDialog() == true
            ? openFileDialog.FileName
            : null;
    }
}
