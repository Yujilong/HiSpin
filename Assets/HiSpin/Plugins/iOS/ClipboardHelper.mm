#import "ClipboardHelper.h"

@implementation Clipboard

- (void) objc_copyTextToClipboard : (NSString*)text
{
     UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
     pasteboard.string = text;
}
@end

extern "C" {
    static Clipboard *iosClipboard;

    void _copyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList];

        if (iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }

        [iosClipboard objc_copyTextToClipboard: text];
    }
}