using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Tools
{
    public static List<string> GetTextFlexiableContent(Text text, string content)
    {
        Font myFont = text.font;
        myFont.RequestCharactersInTexture(content, text.fontSize, text.fontStyle);

        List<string> valuePerModule = new List<string>();
        int contentLength = content.Length;
        string matchString = "\n";
        int startIndex = 0;
        while (startIndex < contentLength)
        {
            int nextModuleIndex = content.IndexOf(matchString, startIndex);
            if (nextModuleIndex < 0)
                nextModuleIndex = contentLength;
            valuePerModule.Add(content.Substring(startIndex, nextModuleIndex - startIndex));
            startIndex = nextModuleIndex + 1;
        }
        return valuePerModule;
    }
    public static List<string> GetTextMiddleCenterContent(Text text, List<string> content)
    {
        float maxSizeX = text.GetComponent<RectTransform>().rect.width;
        Font myFont = text.font;
        float height = myFont.lineHeight * text.lineSpacing;
        int textSize = text.fontSize;
        int maxLength = 0;
        CharacterInfo characterInfo;
        int strCount = content.Count;
        List<int> totalLengths = new List<int>();
        for(int i = 0; i < strCount; i++)
        {
            myFont.RequestCharactersInTexture(content[i], text.fontSize, text.fontStyle);
            char[] charArr = content[i].ToCharArray();
            int totalCharWidth = 0;
            foreach (char ch in charArr)
            {
                myFont.GetCharacterInfo(ch, out characterInfo, textSize);
                totalCharWidth += characterInfo.advance;
            }
            totalLengths.Add(totalCharWidth);
            if (totalCharWidth > maxLength)
                maxLength = totalCharWidth;
        }
        myFont.GetCharacterInfo(' ', out characterInfo, textSize);
        int spaceLength = characterInfo.advance;
        maxLength += spaceLength * 4;
        for (int i = 0; i < strCount; i++)
        {
            float offsetWidth = maxLength - totalLengths[i];
            int addSpaceCount = Mathf.CeilToInt(offsetWidth / spaceLength);
            int insertIndex = content[i].IndexOf("$");
            for (int j = 0; j < addSpaceCount; j++)
                if (j < 4+(j-4)/2)
                    content[i] = content[i].Insert(insertIndex, " ");
                else
                    content[i] = content[i].Insert(0, " ");
        }
        return content;
    }
}
public class Range
{
    private int min;
    private int max;
    public int Min { get { return min; } }
    public int Max { get { return max; } }
    public Range(int min,int max)
    {
        this.min = min;
        this.max = max;
    }
    public int RandomIncludeMax()
    {
        return Random.Range(min, max + 1);
    }
    public int RandomExculdeMax()
    {
        return Random.Range(min, max);
    }
}
