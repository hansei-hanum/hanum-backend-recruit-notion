
using Notion.Client;

namespace Hanum.Recruit.Helpers;

public static class NotionRichText {
    public static RichTextText ToRichText(this string content) {
        return new RichTextText {
            Text = new Text {
                Content = content
            }
        };
    }

    public static ParagraphBlock ToParagraphBlock(this string content) {
        return new ParagraphBlock {
            Paragraph = new ParagraphBlock.Info {
                RichText = [
                    new RichTextText {
                        Text = new Text {
                            Content = content
                        }
                    }
                ]
            }
        };
    }
}