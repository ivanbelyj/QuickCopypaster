using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCopypaster;
public class FragmentsManager
{
    private string[] fragments;

    private int currentFragment = 0;

    public FragmentsManager(string text, int maxFragmentSize,
        int minFillingPercent = 70)
    {
        fragments = TextSplitUtils.SplitWithMaxFragmentSize(text, maxFragmentSize,
            (int)((float)minFillingPercent / 100 * maxFragmentSize));
    }

    public void Prev()
    {
        currentFragment--;
        if (currentFragment < 0)
        {
            currentFragment = 0;
            if (fragments.Length > 1)
                return;
        }
        HandleFragment();
    }

    public void Next() {
        currentFragment++;
        if (currentFragment >= fragments.Length)
        {
            currentFragment = fragments.Length - 1;
            if (fragments.Length > 1)
                return;
        }
        HandleFragment();
    }

    public void HandleFirstFragment()
    {
        currentFragment = 0;
        HandleFragment();
    }

    private void HandleFragment()
    {
        Console.WriteLine($"\n{currentFragment + 1}/{fragments.Length}");
        string currentPart = fragments[currentFragment];
        TextCopy.ClipboardService.SetText(currentPart);
        Console.WriteLine("\n" + currentPart + "\n");
    }
}
