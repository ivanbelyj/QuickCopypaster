using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCopypaster;
public class InteractiveApp
{
    private const string defaultFileName = "input.txt";
    private const int defaultMaxFragmentSize = 256;

    private FragmentsManager? fragmentsManager;

    public void Start()
    {
        Console.WriteLine("Press Ctrl+C to exit");
        bool shouldExit = false;
        while (!shouldExit) {
            shouldExit = ProcessInput();
        }
    }

    private string ReadMultilineText()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Console.WriteLine("Press Ctrl+Z and Enter to end console input");
        string? input;
        while ((input = Console.ReadLine()) != null)
        {
            stringBuilder.AppendLine(input);
        }
        return stringBuilder.ToString();
    }

    /// <returns>true - should exit</returns>
    public bool ProcessInput()
    {
        Console.WriteLine($"Enter text (or nothing to use data from " +
            $"{defaultFileName})");
        string? enteredText = ReadMultilineText();

        string? text = null;
        if (string.IsNullOrEmpty(enteredText))
        {

            text = File.Exists(defaultFileName)
                ? File.ReadAllText(defaultFileName) : null;
        }
        if (text == null)
        {
            text = enteredText;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("No text entered");
            return false;
        }

        Console.WriteLine("Input fragment maximum size (integer positive number) " +
            $"or press Enter to use {defaultMaxFragmentSize}");
        string? inputStr = Console.ReadLine();
        int maxFragmentSize;
        if (string.IsNullOrEmpty(inputStr))
        {
            maxFragmentSize = defaultMaxFragmentSize;
            Console.WriteLine($"Using {maxFragmentSize} max size");
        }
        else
        {
            try
            {
                maxFragmentSize = int.Parse(inputStr ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Integer number expected");
                return false;
            }
        }

        fragmentsManager = new FragmentsManager(text, maxFragmentSize);

        Console.WriteLine("Use arrow keys to move between the separated parts " +
            "of the text (the current fragment will be copied to the clipboard " +
            "automatically) or press Esc to stop moving");

        fragmentsManager.HandleFirstFragment();

        bool isExit = false;
        while (!isExit)
        {
            isExit = ProcessMainAppInput();
        }

        Console.WriteLine("Do you want to copypaste another text? Press y to "
            + "start over");
        return Console.ReadKey(true).Key == ConsoleKey.Y ? false : true;
    }

    public bool ProcessMainAppInput()
    {
        if (fragmentsManager == null)
            throw new InvalidOperationException("Not initialized to process " +
                "main app input");

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
            case ConsoleKey.UpArrow:
            case ConsoleKey.A:
                fragmentsManager.Prev();
                break;
            case ConsoleKey.RightArrow:
            case ConsoleKey.DownArrow:
            case ConsoleKey.D:
                fragmentsManager.Next();
                break;

            case ConsoleKey.Escape:
                Console.WriteLine("Stop copying mode");
                return true;
        }

        return false;
    }
}
