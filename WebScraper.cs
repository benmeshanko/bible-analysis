using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bible_parser;

public static class WebScraper
{
    public static string baseUrl = "https://biblehub.com/{book}/{chapter}-{verse}.htm";

    public static void OutputBookToText(string filepath, Book book)
    {
        StreamWriter sw = new(filepath);
        foreach (var chapter in book.Chapters)
        {
            foreach (var verse in chapter.Verses)
            {
                sw.WriteLine($"{verse.Book} {verse.Chapter}:{verse.Number}\t{verse.Text}");
            }
        }

        sw.Close();
    }

    public static List<string> ParseBooksList(string filepath)
    {
        List<string> books = new();
        StreamReader sr = new(filepath);
        while (!sr.EndOfStream)
        {
            string? line = sr.ReadLine();
            if (line == null) break;
            books.Add(line.Replace(' ', '_'));
        }
        return books;
    }

    public static void ParseWebBible(List<string> bookNames)
    {
        HtmlWeb webLoader = new();

        foreach (string bookName in bookNames)
        {
            Book book = new([]);

            int chapterDx = 1;
            while (true)
            {
                int verseDx = 1;
                Chapter chapter = new([]);
                while (true)
                {
                    string url = baseUrl
                    .Replace("{book}", bookName)
                    .Replace("{chapter}", chapterDx.ToString())
                    .Replace("{verse}", verseDx.ToString());

                    var webContents = webLoader.Load(url).ParsedText;
                    if (webContents.Length < 2500)
                    {
                        // Just skip for now - some of the pages are blank, etc
                        verseDx++;
                        continue;
                    }
                    var parsedText = webContents[2500..8000];

                    // Check if "English Standard Version" appears in text
                    int esvIndex = parsedText.IndexOf("English Standard Version") + 41;
                    if (esvIndex != 40)
                    {
                        // Replace HTML escape characters with text values
                        string verseText = parsedText[esvIndex..];
                        verseText = verseText[..verseText.IndexOf('<')]
                            .Replace("&#8212;", "-")
                            .Replace("&#8216;", "\'")
                            .Replace("&#8217;", "\'")
                            .Replace("&#8220;", "\"")
                            .Replace("&#8221;", "\"");
                        chapter.Verses.Add(new(bookName, chapterDx, verseDx, verseText));
                    }
                    else
                    {
                        break;
                    }
                    verseDx++;
                }

                // If Verse #1 didn't exist for this chapter, we are out of chapters! Move to the next book.
                if (verseDx == 1) break;

                book.Chapters.Add(chapter);
                chapterDx++;
            }

            // Write the contents of the book to a text file
            OutputBookToText(bookName + ".txt", book);
        }
    }
}
