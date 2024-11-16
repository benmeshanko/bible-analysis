namespace bible_parser;

public static class FileScraper
{
    // Parses each file in the directory
    public static List<Book> ParseDirectory(string directorypath)
    {
        List<Book> books = [];
        string[] filenames = Directory.GetFiles(directorypath);
        foreach(var filepath in filenames)
        {
            books.Add(ParseFile(filepath));
        }
        return books;
    }

    public static Book ParseFile(string filepath)
    {
        Book book = new([]);
        Chapter chapter = new([]);
        int currChapter = 1;

        StreamReader sr = new(filepath);
        while (!sr.EndOfStream)
        {
            string? line = sr.ReadLine();
            if (line == null) break;

            // [colossians 1:15	He is the image of the invisible God, the firstborn of all creation.]
            string bookName = line[..line.IndexOf(' ')];
            line = line[(line.IndexOf(' ') + 1)..];

            int chapterNum = int.Parse(line[..line.IndexOf(':')]);
            line = line[(line.IndexOf(':') + 1)..];

            int verseNum = int.Parse(line[..line.IndexOf('\t')]);
            string verseText = line[(line.IndexOf('\t') + 1)..];

            Verse verse = new(bookName, chapterNum, verseNum, verseText);

            if (currChapter != chapterNum)
            {
                // We've read all the verses for a chapter. Create a new one
                book.Chapters.Add(chapter);
                chapter = new([]);
                currChapter = chapterNum;
            }
            
            // Add verse to chapter
            chapter.Verses.Add(verse);
        }
        return book;
    }
}
