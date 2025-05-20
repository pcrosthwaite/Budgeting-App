using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System;

//
// Summary:
//     Contains a collection of static string functions not found in the .NET Framework
public static class StringFunctions
{
    //
    // Summary:
    //     String processing functions that use the primitive data type string. Effective
    //     for small-to-medium strings.
    public class StringProcessing
    {
        //
        // Summary:
        //     Returns a string representation of an array. Each element is written on a new
        //     line.
        public static string ArrayToString(IList array)
        {
            if (array == null || array.Count == 0)
            {
                return string.Empty;
            }

            string text = string.Empty;
            for (int i = 0; i < array.Count; i++)
            {
                text += array[i].ToString();
                if (i != array.Count - 1)
                {
                    text += Environment.NewLine;
                }
            }

            return text;
        }

        //
        // Summary:
        //     Returns a string representation of an array. Each element is separated by the
        //     specified string.
        //
        // Parameters:
        //   array:
        //     The collection of objects.
        //
        //   separator:
        //     The string sequence to separate each element in the collection
        public static string ArrayToString(IList array, string separator)
        {
            if (array == null || array.Count == 0)
            {
                return string.Empty;
            }

            string text = string.Empty;
            for (int i = 0; i < array.Count; i++)
            {
                text += array[i].ToString();
                if (i != array.Count - 1)
                {
                    text += separator;
                }
            }

            return text;
        }
    }

    //
    // Summary:
    //     String processing functions that use the StringBuilder class. Effective for large
    //     strings.
    public class StringBuilderProcessing
    {
        //
        // Summary:
        //     Returns a string representation of an array. Each element is written on a new
        //     line.
        public static string ArrayToString(IList array)
        {
            if (array == null || array.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder(array.Count * 2);
            for (int i = 0; i < array.Count; i++)
            {
                stringBuilder.Append(array[i].ToString());
                if (i != array.Count - 1)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Returns a string representation of an array. Each element is separated by the
        //     specified string.
        //
        // Parameters:
        //   array:
        //     The collection of objects.
        //
        //   separator:
        //     The string sequence to separate each element in the collection
        public static string ArrayToString(IList array, string separator)
        {
            if (array == null || array.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder(array.Count * 2);
            for (int i = 0; i < array.Count; i++)
            {
                stringBuilder.Append(array[i].ToString());
                if (i != array.Count - 1)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }
    }

    //
    // Summary:
    //     Function to return text with each word capitalised eg. hello world = Hello World
    //
    //
    // Parameters:
    //   text:
    //
    //   separator:
    public static string ToProperCase(string text, string separator = " ")
    {
        if (!text.HasValue())
        {
            return string.Empty;
        }

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return string.Join(separator, from word in text.Split(new string[1] { separator }, StringSplitOptions.None)
                                      select textInfo.ToTitleCase(word.ToLower()));
    }

    //
    // Summary:
    //     Returns a string with characters in reverse order.
    public static string Reverse(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        char[] array = input.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }

    //
    // Summary:
    //     Returns a string with a given separator inserted after every character.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The separator to insert.
    public static string InsertSeparator(this string input, string separator)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        char[] collection = separator.ToCharArray();
        for (int i = 1; i < list.Count; i += 1 + separator.Length)
        {
            if (i != list.Count)
            {
                list.InsertRange(i, collection);
            }
        }

        return new string(list.ToArray());
    }

    //
    // Summary:
    //     Return the plural form of a string based on the number
    //
    // Parameters:
    //   input:
    //
    //   number:
    public static string GetPlural(this string singular, int? number)
    {
        if (!singular.HasValue() || !number.HasValue)
        {
            return singular;
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            { "child", "children" },
            { "person", "people" },
            { "day", "days" }
        };
        if (number.GetValueOrDefault() == 1)
        {
            return singular;
        }

        if (dictionary.ContainsKey(singular))
        {
            return dictionary[singular];
        }

        if (singular.EndsWith("y"))
        {
            return singular.Substring(0, singular.Length - 1) + "ies";
        }

        return singular + "s";
    }

    //
    // Summary:
    //     Function to mask a string of text
    //
    // Parameters:
    //   origString:
    //
    //   lastDigits:
    //     The number of end characters to show. Default is 4
    //
    //   prefixDigits:
    //     The number of start characters to show. Default is none
    public static string MaskString(string origString, int lastDigits = 4, int? prefixDigits = null)
    {
        origString = origString.Trim();
        if (!origString.HasValue())
        {
            return origString;
        }

        int num = prefixDigits.GetValueOrDefault();
        int count = origString.Length - num - lastDigits;
        if (origString.Length <= num + lastDigits)
        {
            num = Math.Max(origString.Length - lastDigits, 0);
        }

        string text = new string('*', count);
        string obj = ((num > 0) ? origString.Substring(0, num) : "");
        string text2 = ((origString.Length >= lastDigits) ? origString.Substring(origString.Length - lastDigits) : "");
        return obj + text + text2;
    }

    //
    // Summary:
    //     Returns a string with a given separator inserted after a specified interval of
    //     characters.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The separator to insert.
    //
    //   interval:
    //     The number of characters between separators.
    public static string InsertSeparator(this string input, string separator, int interval)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        char[] collection = separator.ToCharArray();
        for (int i = interval; i < list.Count; i += interval + separator.Length)
        {
            if (i != list.Count)
            {
                list.InsertRange(i, collection);
            }
        }

        return new string(list.ToArray());
    }

    //
    // Summary:
    //     Returns a string with any vowel character removed.
    public static string RemoveVowels(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        for (int num = list.Count - 1; num >= 0; num--)
        {
            if (list[num] == 'a' || list[num] == 'A' || list[num] == 'e' || list[num] == 'E' || list[num] == 'i' || list[num] == 'I' || list[num] == 'o' || list[num] == 'O' || list[num] == 'u' || list[num] == 'U')
            {
                list.RemoveAt(num);
            }
        }

        return new string(list.ToArray());
    }

    //
    // Summary:
    //     Returns a string with only the vowel characters.
    public static string KeepVowels(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        for (int num = list.Count - 1; num >= 0; num--)
        {
            if (list[num] != 'a' && list[num] != 'A' && list[num] != 'e' && list[num] != 'E' && list[num] != 'i' && list[num] != 'I' && list[num] != 'o' && list[num] != 'O' && list[num] != 'u' && list[num] != 'U')
            {
                list.RemoveAt(num);
            }
        }

        return new string(list.ToArray());
    }

    //
    // Summary:
    //     Returns a string with alternated letter casing (upper/lower). First character
    //     of the string stays the same.
    public static string AlternateCases(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (input.Length == 1)
        {
            return input;
        }

        char[] array = input.ToCharArray();
        bool flag = !char.IsUpper(array[0]);
        for (int i = 1; i < array.Length; i++)
        {
            if (flag)
            {
                array[i] = char.ToUpper(array[i]);
            }
            else
            {
                array[i] = char.ToLower(array[i]);
            }

            flag = !flag;
        }

        return new string(array);
    }

    //
    // Summary:
    //     Returns a string with the opposite letter casing for each character.
    public static string SwapCases(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        char[] array = input.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (char.IsUpper(array[i]))
            {
                array[i] = char.ToLower(array[i]);
            }
            else
            {
                array[i] = char.ToUpper(array[i]);
            }
        }

        return new string(array);
    }

    //
    // Summary:
    //     Capitalises the first character in a string.
    public static string Capitalize(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (input.Length == 1)
        {
            return input.ToUpper();
        }

        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }

    //
    // Summary:
    //     Returns the initials of each word in a string. Words must be separated with spaces.
    //
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   capitalizeInitials:
    //     True to capitalise each initial in the output string.
    //
    //   preserveSpaces:
    //     True to preserver the spaces between initials in the output string.
    //
    //   includePeriod:
    //     True to include a '.' after each initial
    public static string GetInitials(this string input, bool capitalizeInitials, bool preserveSpaces, bool includePeriod)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string[] array = input.Split(' ');
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Length > 0)
            {
                if (capitalizeInitials)
                {
                    array[i] = char.ToUpper(array[i][0]).ToString();
                }
                else
                {
                    array[i] = array[i][0].ToString();
                }

                if (includePeriod)
                {
                    array[i] += ".";
                }
            }
        }

        if (preserveSpaces)
        {
            return string.Join(" ", array);
        }

        return string.Join("", array);
    }

    //
    // Summary:
    //     Returns the initials of each word in a string. Words are separated according
    //     to the specified string sequence.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The string sequence that separates words.
    //
    //   capitalizeInitials:
    //     True to capitalise each initial in the output string.
    //
    //   preserveSeparator:
    //     True to preserver the spaces between initials in the output string.
    public static string GetInitials(this string input, string separator, bool capitalizeInitials, bool preserveSeparator, bool includePeriod)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string[] array = input.Split(separator.ToCharArray());
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Length > 0)
            {
                if (capitalizeInitials)
                {
                    array[i] = char.ToUpper(array[i][0]).ToString();
                }
                else
                {
                    array[i] = array[i][0].ToString();
                }

                if (includePeriod)
                {
                    array[i] += ".";
                }
            }
        }

        if (preserveSeparator)
        {
            return string.Join(separator, array);
        }

        return string.Join("", array);
    }

    //
    // Summary:
    //     Returns a string with each word's first character capitalised. Words are separated
    //     according to the specified string sequence
    //
    //     If no seperator is provided, a space character is used
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The string sequence that separates words.
    public static string GetTitle(this string input, string separator = " ")
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string[] array = input.Split(separator.ToCharArray());
        for (int i = 0; i < array.Length; i++)
        {
            string arg = null;
            if (array[i].Length > 0)
            {
                int num = 0;
                if (array[i].Length > 1 && !array[i][num].ToString().IsLetters())
                {
                    arg = array[i][num].ToString();
                    num++;
                }

                array[i] = $"{arg}{char.ToUpper(array[i][num])}{array[i].Substring(num + 1).ToLower()}";
            }
        }

        return string.Join(separator, array);
    }

    //
    // Summary:
    //     Returns a string with each word's first character capitalised. Words are separated
    //     according to the specified string sequence.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The string sequence that separates words.
    public static string FormatABN(string abnString)
    {
        if (abnString.Length != 11)
        {
            return abnString;
        }

        string text = abnString.Substring(0, 2);
        string text2 = abnString.Substring(2, 3);
        string text3 = abnString.Substring(5, 3);
        string text4 = abnString.Substring(8, 3);
        return text + " " + text2 + " " + text3 + " " + text4;
    }

    public static string FormatACN(string acnString)
    {
        if (!acnString.HasValue())
        {
            return acnString;
        }

        string text = acnString.Replace(" ", "").Replace("-", "").Trim();
        if (text.Length != 9)
        {
            return acnString;
        }

        string text2 = text.Substring(0, 3);
        string text3 = text.Substring(3, 3);
        string text4 = text.Substring(6, 3);
        return text2 + " " + text3 + " " + text4;
    }

    //
    // Summary:
    //     Returns a segment of a string, marked by the start and end index (exclusive).
    //
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   start:
    //     The start index position.
    //
    //   end:
    //     The end index position. (exclusive)
    public static string SubstringEnd(this string input, int start, int end)
    {
        if (string.IsNullOrEmpty(input) || start == end)
        {
            return string.Empty;
        }

        if (start == 0 && end == input.Length)
        {
            return input;
        }

        if (start < 0)
        {
            throw new IndexOutOfRangeException("start index cannot be less than zero.");
        }

        if (start > input.Length)
        {
            throw new IndexOutOfRangeException("start index cannot be greater than the length of the string.");
        }

        if (end < 0)
        {
            throw new IndexOutOfRangeException("end index cannot be less than zero.");
        }

        if (end > input.Length)
        {
            throw new IndexOutOfRangeException("end index cannot be greater than the length of the string.");
        }

        if (start > end)
        {
            throw new IndexOutOfRangeException("start index cannot be greater than the end index.");
        }

        return input.Substring(start, end - start);
    }

    //
    // Summary:
    //     Returns a truncated string
    //
    // Parameters:
    //   str:
    //     The string to truncate
    //
    //   maxLength:
    //     The maximum length the string can be
    //
    //   addEllipses:
    //     Indicates if ... should be added to the end of the string. Default is false
    public static string Truncate(this string str, int maxLength, bool addEllipses = false)
    {
        if (!str.HasValue())
        {
            return str;
        }

        int num = ((str.Length < maxLength) ? str.Length : maxLength);
        return str[..(addEllipses ? (num - "...".Length) : num)] + (addEllipses ? "..." : null);
    }

    //
    // Summary:
    //     Returns the character in a string at a given index counting from the right.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   index:
    //     The starting position from the right. (Index 0 = last character)
    public static char CharRight(this string input, int index)
    {
        if (string.IsNullOrEmpty(input))
        {
            return '\0';
        }

        if (input.Length - index - 1 >= input.Length)
        {
            throw new IndexOutOfRangeException("Index cannot be less than zero.");
        }

        if (input.Length - index - 1 < 0)
        {
            throw new IndexOutOfRangeException("Index cannot be larger than the length of the string");
        }

        return input[input.Length - index - 1];
    }

    //
    // Summary:
    //     Returns the character at a position given by the startingIndex plus the given
    //     count.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   startingIndex:
    //     The starting index position.
    //
    //   countIndex:
    //     The number of characters to count from the starting position.
    public static char CharMid(this string input, int startingIndex, int count)
    {
        if (string.IsNullOrEmpty(input))
        {
            return '\0';
        }

        if (startingIndex < 0)
        {
            throw new IndexOutOfRangeException("startingIndex cannot be less than zero.");
        }

        if (startingIndex >= input.Length)
        {
            throw new IndexOutOfRangeException("startingIndex cannot be greater than the length of the string.");
        }

        if (startingIndex + count < 0)
        {
            throw new IndexOutOfRangeException("startingIndex + count cannot be less than zero.");
        }

        if (startingIndex + count >= input.Length)
        {
            throw new IndexOutOfRangeException("startingIndex + count cannot be greater than the length of the string.");
        }

        return input[startingIndex + count];
    }

    //
    // Summary:
    //     Returns the total number of times a given sequence appears in a string.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   sequence:
    //     The string sequence to count.
    //
    //   ignoreCase:
    //     True, to ignore the difference in case between the sequence and the original
    //     string.
    public static int CountString(this string input, string sequence, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(sequence))
        {
            return 0;
        }

        int num = 0;
        for (int i = 0; i < input.Length && i + sequence.Length <= input.Length; i++)
        {
            if (string.Compare(input.Substring(i, sequence.Length), sequence, ignoreCase) == 0)
            {
                num++;
            }
        }

        return num;
    }

    //
    // Summary:
    //     Returns an array of every index where a sequence is found on the specified string.
    //     Note: Overlaps will be counted.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   sequence:
    //     The string sequence to seek.
    //
    //   ignoreCase:
    //     True, to ignore the difference in case between the sequence and the original
    //     string.
    public static int[] IndexOfAll(this string input, string sequence, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new int[0];
        }

        List<int> list = new List<int>();
        for (int i = 0; i < input.Length && i + sequence.Length <= input.Length; i++)
        {
            if (string.Compare(input.Substring(i, sequence.Length), sequence, ignoreCase) == 0)
            {
                list.Add(i);
            }
        }

        int[] result = list.ToArray();
        list.Clear();
        return result;
    }

    //
    // Summary:
    //     Returns an array of every index where a sequence is found on the specified string.
    //     Note: Overlaps will be counted.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   sequence:
    //     The string sequence to seek.
    //
    //   startIndex:
    //     Index from which to start seeking.
    //
    //   ignoreCase:
    //     True, to ignore the difference in case between the sequence and the original
    //     string.
    public static int[] IndexOfAll(this string input, string sequence, int startIndex, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new int[0];
        }

        List<int> list = new List<int>();
        for (int i = startIndex; i < input.Length && i + sequence.Length <= input.Length; i++)
        {
            if (string.Compare(input.Substring(i, sequence.Length), sequence, ignoreCase) == 0)
            {
                list.Add(i);
            }
        }

        int[] result = list.ToArray();
        list.Clear();
        return result;
    }

    //
    // Summary:
    //     Returns whether the letter casing in a string is alternating.
    public static bool IsAlternateCases(this string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length == 1)
        {
            return false;
        }

        bool flag = char.IsUpper(input[0]);
        for (int i = 1; i < input.Length; i++)
        {
            if (flag)
            {
                if (char.IsUpper(input[i]))
                {
                    return false;
                }
            }
            else if (char.IsLower(input[i]))
            {
                return false;
            }

            flag = !flag;
        }

        return true;
    }

    //
    // Summary:
    //     Returns true if the first character in a string is upper case.
    public static bool IsCapitalized(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        return char.IsUpper(input[0]);
    }

    //
    // Summary:
    //     Returns whether a string is in all lower case.
    public static bool IsLowerCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsLower(input[i]) && char.IsLetter(input[i]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is in all upper case.
    public static bool IsUpperCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsUpper(input[i]) && char.IsLetter(input[i]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string contains any vowel letters
    public static bool HasVowels(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == 'a' || input[i] == 'A' || input[i] == 'e' || input[i] == 'E' || input[i] == 'i' || input[i] == 'I' || input[i] == 'o' || input[i] == 'O' || input[i] == 'u' || input[i] == 'U')
            {
                return true;
            }
        }

        return false;
    }

    //
    // Summary:
    //     Returns whether a string is all empty spaces
    public static bool IsSpaces(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            return input.Replace(" ", "").Length == 0;
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is composed of only a single character.
    public static bool IsRepeatedChar(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            return input.Replace(input[0].ToString(), "").Length == 0;
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is composed of only numeric characters.
    public static bool IsNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsNumber(input[i]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string contains any numberic characters.
    public static bool HasNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsNumber(input[i]))
            {
                return true;
            }
        }

        return false;
    }

    //
    // Summary:
    //     Returns whether a string is composed of only letter and number characters.
    public static bool IsAlphaNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsLetter(input[i]) && !char.IsNumber(input[i]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is composed of all letter characters.
    public static bool IsLetters(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsLetter(input[i]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is formatted like a title, i.e. the first character
    //     of each word is capitalised. Words must be separated by spaces.
    public static bool IsTitle(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        string[] array = input.Split(' ');
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Length > 0 && !char.IsUpper(array[i][0]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is formatted like a title, i.e. the first character
    //     of each word is capitalised. Words are separated according to the specified string
    //     sequence.
    //
    // Parameters:
    //   input:
    //     The original string.
    //
    //   separator:
    //     The string sequence that separates words.
    public static bool IsTitle(this string input, string separator)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        string[] array = input.Split(separator.ToCharArray());
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Length > 0 && !char.IsUpper(array[i][0]))
            {
                return false;
            }
        }

        return true;
    }

    //
    // Summary:
    //     Returns whether a string is in a valid email address format.
    public static bool IsEmailAddress(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (input.IndexOf("@", StringComparison.Ordinal) != -1 && input.Length >= 5 && input.LastIndexOf(".", StringComparison.Ordinal) > input.IndexOf("@", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    public static string GetUserFromEmailAddress(this string input)
    {
        if (input.IsEmailAddress())
        {
            return input[..input.IndexOf("@", StringComparison.Ordinal)];
        }

        return input;
    }

    public static string ValueNotNullOrEmpty(this string current, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }

        return current;
    }

    public static bool IsNull(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNotNull(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static bool HasValue(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    //
    // Summary:
    //     Returns a maximum number of characters from a string. If Packages.Helpers.StartingPosition
    //     parameter is End, then the last X characters are returned
    //
    // Parameters:
    //   str:
    //
    //   maxLength:
    //
    //   startingPosition:
    //     Set the starting position.
    public static string MaxLength(this string str, int maxLength, StartingPosition startingPosition = StartingPosition.Start)
    {
        if (str == null)
        {
            return null;
        }

        maxLength = Math.Min(str.Length, maxLength);
        if (startingPosition == StartingPosition.Start)
        {
            return str.SubstringEnd(0, maxLength);
        }

        int start = str.Length - maxLength;
        return str.SubstringEnd(start, str.Length);
    }

    public static string UrlCombine(this string url1, string url2)
    {
        if (url1.Length == 0)
        {
            return url2;
        }

        if (url2.Length == 0)
        {
            return url1;
        }

        url1 = url1.TrimEnd('/', '\\');
        url2 = url2.TrimStart('/', '\\');
        return $"{url1}/{url2}";
    }

    //
    // Summary:
    //     Returns a string with any vowel character removed.
    public static string RemoveNonNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        for (int num = list.Count - 1; num >= 0; num--)
        {
            if (!char.IsNumber(list[num]) && list[num] != '.')
            {
                list.RemoveAt(num);
            }
        }

        return new string(list.ToArray());
    }

    //
    // Summary:
    //     Returns a string with any vowel character removed.
    public static string RemoveNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        List<char> list = new List<char>(input.ToCharArray());
        for (int num = list.Count - 1; num >= 0; num--)
        {
            if (char.IsNumber(list[num]))
            {
                list.RemoveAt(num);
            }
        }

        return new string(list.ToArray());
    }

    public static string StripHtml(this string input)
    {
        return new Regex("</?.+?>").Replace(input, " ");
    }

    //
    // Summary:
    //     Joins the elements of a sequence into a single string. Any null or empty elements
    //     will be omitted from the result.
    //
    //     Each element must be convertible to a string or null will be returned.
    //
    //     Examples: JoinValidStrings(".", "string1", null, 23); or JoinValidStrings(".",
    //     new[] { "string1", null, 23 });
    //
    // Parameters:
    //   separator:
    //
    //   values:
    public static string JoinValidStrings(string separator, params object[] values)
    {
        IEnumerable<object> source = values.SelectMany((object v) => (!(v is Array source2)) ? new object[1] { v } : source2.Cast<object>());
        if (source.Any((object v) => !CanConvertToString(v)))
        {
            return null;
        }

        IEnumerable<string> values2 = from x in source
                                      where x != null && !string.IsNullOrEmpty(x.ToString())
                                      select x into v
                                      select v.ToString();
        return string.Join(separator, values2);
    }

    public static string JoinValidStrings<T>(string separator, IEnumerable<T> values)
    {
        return JoinValidStrings(separator, values.Cast<object>().ToArray());
    }

    private static bool CanConvertToString(object value)
    {
        try
        {
            value?.ToString();
            return true;
        }
        catch
        {
            return false;
        }
    }
}