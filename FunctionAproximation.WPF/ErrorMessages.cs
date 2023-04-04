using System.Windows;

namespace FunctionAproximation.WPF;

public static class ErrorMessages
{
    public static void TargetFilePathEqualsToSourceFilePath() 
        => MessageBox.Show("Target file path can not be equal to the source file path",
                           "Invalid target file path",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);

    public static void OnError(string message)
     => MessageBox.Show(message,
                        "Something went wrong :(",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
}
