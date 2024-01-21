using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuickCopypaster;
public static class TextSplitUtils
{
    const bool showDebugMessages = false;

    private static char[][] prioritizedSeparators = new[] {
        // Ближе к началу - приоритетнее
        new[] 
        {
            '.',
            ';',
            ':',
            '!',
            '?',
            '\n'
        },
        new[]
        {
            ' ', ',',
        }
    };

    public static string[] SplitWithMaxFragmentSize(string text,
        int maxFragmentSize, int minFragmentSize)
    {
        if (showDebugMessages)
            Console.WriteLine("Min fragment size: " + minFragmentSize);

        var parts = new List<string>();

        // Start and end indexes of the current fragment
        int fragStart = 0;
        int fragEnd = 0;

        // Each fragment satisfies the following conditions
        // (in order of decreasing the priority of the conditions)
        // 1. minSize, maxSize
        // 2. The last element of the text is the separator
        // with the highest priority
        
        // Todo: dont split micro words away (less than 3 symbols)

        while (fragStart < text.Length - maxFragmentSize)
        {
            fragEnd = fragStart + maxFragmentSize;

            // Find separator with the highest priority
            int bestSeparatorIndex = fragEnd;
            while (fragEnd >= fragStart + minFragmentSize)
            {
                char curSeparator = text[fragEnd];
                char bestSeparator = text[bestSeparatorIndex];
                int curSeparatorPriority = GetSeparatorPriority(curSeparator);
                int bestSeparatorPriority = GetSeparatorPriority(bestSeparator);
                if (curSeparatorPriority > bestSeparatorPriority)
                {
                    bestSeparatorIndex = fragEnd;
                }
                fragEnd--;
            }
            fragEnd = bestSeparatorIndex;

            string fragment = text.Substring(fragStart, fragEnd - fragStart + 1);
            parts.Add(fragment);

            if (showDebugMessages)
                Console.WriteLine($"Adding fragment (size: {fragment.Length}): "
                    + fragment);

            fragStart = fragEnd + 1;
        }
        if (fragStart < text.Length)
        {
            parts.Add(text.Substring(fragStart));
        }
        return parts.ToArray();
    }

    private static int GetSeparatorPriority(char separator)
    {
        for (int i = 0; i < prioritizedSeparators.Length; i++)
        {
            if (prioritizedSeparators[i].Contains(separator))
            {
                int res = prioritizedSeparators.Length - i;
                return res;
            }
        }
        return -1;
    }
}
